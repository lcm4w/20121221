using System;
using TourWriter.Forms;
using System.ComponentModel;

namespace TourWriter.Services
{
    class LicenseService
    {
        internal static void CheckLicense()
        {
            var licenseThread = new BackgroundWorker();
            licenseThread.DoWork += LicenseThreadDoWork;
            licenseThread.RunWorkerCompleted += LicenseThreadRunWorkerCompleted;
            licenseThread.RunWorkerAsync();
        }

        static void LicenseThreadDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var license = new Info.License();
                license.LoadFromDatabase();
                e.Result = license;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ErrorHelper.IsServerConnectionError(ex)) e.Result = null;
                else throw;
            }
        }

        static void LicenseThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null) return;

            var license = (Info.License) e.Result;
            var notify = false;
            var delay = 0;
            ValidateLicense(license.EndDate, ref notify, ref delay);
            if (!notify) return;

            _lastNotifiedDate = DateTime.Now;
            var form = new LicenseExpiredForm(license.EndDate, delay);
            App.MainForm.Invoke((System.Windows.Forms.MethodInvoker)(() => form.ShowDialog(App.MainForm)));
        }

        private static DateTime _lastNotifiedDate = DateTime.MinValue;
        private static void ValidateLicense(DateTime expDate, ref bool notify, ref int delay)
        {
            var daysExpired = (DateTime.Now - expDate.Date).Days;

            if (daysExpired <= 0) 
            {
                delay = 0;
                notify = false;
            }
            else if (daysExpired <= 15) 
            {
                delay = 4;
                notify = _lastNotifiedDate.AddHours(6) < DateTime.Now;
            }
            else if (daysExpired <= 30)
            {
                delay = 8;
                notify = _lastNotifiedDate.AddHours(3) < DateTime.Now;
            }
            else
            {
                delay = 16;
                notify = true;
            }
        }
    }
}
