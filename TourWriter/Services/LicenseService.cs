using System;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Forms;
using System.ComponentModel;
using TourWriter.Info;

namespace TourWriter.Services
{
    class LicenseService
    {
        public const string LicenseExpiredMessage =
            "Your TourWriter License expired on {0}.\r\nPlease contact TourWriter to renew your license";
        public const string LicenseOverrunMessage =
            "Your TourWriter License ({0} users) has been exceeded ({1} online) for this session (last {2} minutes).";

        private static Info.License _license;
        private static Guid _sessionId = Guid.Empty;
        private static int _sessionIndex;
        private static int _sessionCount;
        private const int SessionTimeout = 30; // minutes
        private static DateTime _lastLicenseRefresh = DateTime.MinValue;
        private static DateTime _lastNotifiedDate = DateTime.MinValue;


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
            _license = new Info.License();
            _license.LoadFromDatabase();
        }

        private static void LoadSession()
        {
            var computerName = SystemInformation.ComputerName + "\\" + SystemInformation.UserName;

            try
            {
                UserSession.AddOrUpdate(Global.Cache.User.UserID, computerName, SessionTimeout,
                    ref _sessionId, ref _sessionIndex, ref _sessionCount);
                _lastLicenseRefresh = DateTime.Now;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.ToString().ToLower().Contains("stored procedure '_Login_AddOrUpdate' doesn't exist".ToLower()))
                    return; // swallow, first run on 2010.05.04 db update will fail as sp does not exist until after update
                throw; 
            }

        }

        private static void ValidateLicense()
        {
            if (_license == null) return;

            var notify = false;
            var delay = 0;

            ValidateDates(_license.EndDate, ref notify, ref delay);
            var isExpired = notify;

            if (!notify) ValidateConnections(_license, ref notify, ref delay);

            if (notify)
            {
                //_license = null; // force recheck in case new license gets loaded
                _lastNotifiedDate = DateTime.Now;
                var msg = isExpired
                              ? string.Format(LicenseExpiredMessage, _license.EndDate.ToShortDateString())
                              : string.Format(LicenseOverrunMessage, _license.MaxUsers, _sessionCount, SessionTimeout);

                var form = new LicenseExpiredForm(delay, msg);
                App.MainForm.Invoke((MethodInvoker) (() => form.ShowDialog(App.MainForm)));
            }
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
                delay = 8;
                notify = _lastNotifiedDate.AddHours(3) < DateTime.Now;
            }
            else
            {
                delay = 16;
                notify = true;
            }
        }

        private static void ValidateConnections(Info.License license, ref bool notify, ref int delay)
        {
            if (_sessionIndex > license.MaxUsers) // this will notify exceeded users
            //if (_sessionCount > license.MaxUsers) // this will notify all users
            {
                delay = 10;
                notify = true;

                // =======================================================================================
                // TODO: override client notifications with this monitoring code instead
                notify = false;
                delay = 0;
                var detail = string.Format("\r\ninsert into x ([computer],[user],[license],[connections],[index],[date]) values ('{0}','{1}',{2},{3},{4},'{5}')\r\n", 
                    SystemInformation.ComputerName + "\\" + SystemInformation.UserName, Global.Cache.User.Email, license.MaxUsers, _sessionCount, _sessionIndex, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                ErrorHelper.SendEmail("Max users exceeded", detail, true);
                // =======================================================================================
            }
        }

        /*

        #region OLD...
        internal static void CheckAsync_OLD()
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
                if (_license == null)
                {
                    var license = new Info.License();
                    license.LoadFromDatabase();
                    e.Result = license;
                }

                var refreshSession = DateTime.Now > _lastLicenseRefresh.AddMinutes(SessionTimeout);
                if (refreshSession)
                {
                    //                    var sql = @"select LicenseID,LicenseFile,(select count(*) from [Login] where LastActiveDate > dateadd(minute, -20, getdate())) [Sessions]
                    //                                from License where LicenseID = (select max(LicenseID) from License)";

                    var computerName = SystemInformation.ComputerName + "\\" + SystemInformation.UserName;
                    
                    // An error occured: The stored procedure '_Login_AddOrUpdate' doesn't exist.
                    UserSession.AddOrUpdate(Global.Cache.User.UserID, computerName, SessionTimeout,
                        ref _sessionId, ref _sessionIndex, ref _sessionCount);
                    _lastLicenseRefresh = DateTime.Now;
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ErrorHelper.IsServerConnectionError(ex)) e.Result = null;
                else throw;
            }
        }

        static void LicenseThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _license = e.Result as Info.License;
            ValidateLicense();
        }
        #endregion

        */
    }
}
