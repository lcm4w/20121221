using System;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Forms;
using TourWriter.Info;

namespace TourWriter.Services
{
    class LicenseService
    {
        public const string LicenseExpiredMessage =
            "Your TourWriter License expired on {0}.\r\nPlease contact TourWriter to renew your license";
        public const string LicenseOverrunMessage =
            "Your TourWriter License ({0} users) has been exceeded ({1} minute period). Data is readonly.";

        private static License _license;
        private static Guid _sessionId = Guid.Empty;
        private static int _sessionIndex;
        private static int _sessionCount;
        private const int SessionTimeout = 60; // minutes
        private static DateTime _lastLicenseRefresh = DateTime.MinValue;
        private static DateTime _lastNotifiedDate = DateTime.MinValue;
        internal static bool ForceReadOnlyMode;


        internal static void CheckAsync()
        {
            ThreadPool.QueueUserWorkItem(
                state =>
                    {
                        try
                        {
                            var loadLicense = _license == null;
                            var loadSession = DateTime.Now > _lastLicenseRefresh.AddMinutes(SessionTimeout);

                            if (loadLicense) LoadLicense();
                            if (loadSession) LoadSession();
                            ValidateLicense();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            if (!ErrorHelper.IsServerConnectionError(ex)) throw;
                        }
                    });
        }

        internal static void QuitAsync()
        {
            var computerName = SystemInformation.ComputerName + "\\" + SystemInformation.UserName;
            ThreadPool.QueueUserWorkItem(
                state =>
                    {
                        try { UserSession.Quit(computerName); }  catch { }
                    });
        }


        private static void LoadLicense()
        {
            _license = new License();
            _license.LoadFromDatabase();
        }

        private static void LoadSession()
        {
            var user = Global.Cache.User;
            if (user == null || user.UserName == App.AdminUserName) return; // don't log admin

            var computerName = SystemInformation.ComputerName + "\\" + SystemInformation.UserName;
            UserSession.AddOrUpdate(user.UserID, computerName, SessionTimeout, ref _sessionId, ref _sessionIndex, ref _sessionCount);
            _lastLicenseRefresh = DateTime.Now;
        }

        private static void ValidateLicense()
        {
            if (_license == null) return;

            var notify = false;
            var delay = 0;

            ValidateDates(_license.EndDate, ref notify, ref delay);
            var isExpired = notify;

            if (!notify) ValidateConnections(_license, ref notify, ref delay);
            var isMaxed = !isExpired && notify;

            if (!notify) return; // all good

            //_license = null; // force recheck in case new license gets loaded
            _lastNotifiedDate = DateTime.Now;
            var msg = isExpired
                          ? string.Format(LicenseExpiredMessage, _license.EndDate.ToShortDateString())
                          : string.Format(LicenseOverrunMessage, _license.MaxUsers, SessionTimeout);

            var form = new LicenseExpiredForm(delay, msg);
            App.MainForm.Invoke((MethodInvoker) (() => form.ShowDialog(App.MainForm)));
        }

        private static void ValidateDates(DateTime expDate, ref bool notify, ref int delay)
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
                delay = 12;
                notify = _lastNotifiedDate.AddHours(3) < DateTime.Now;
            }
            else
            {
                delay = 60;
                notify = true;
                if (daysExpired > 60) ForceReadOnlyMode = true;
            }
        }

        private static void ValidateConnections(License license, ref bool notify, ref int delay)
        {
            if (App.SkipFloatingLicenseCheck) return;

            if (_sessionIndex > license.MaxUsers) // this will notify exceeded users
            //if (_sessionCount > license.MaxUsers) // this will notify all users
            {
                delay = 0;
                notify = true;
                ForceReadOnlyMode = true;
            }
        }
    }
}
