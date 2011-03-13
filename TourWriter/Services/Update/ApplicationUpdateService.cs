using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TourWriter.Services.Update
{
    /// <summary>
    /// Service to handle new application version check, download, and installation.
    /// </summary>
    static class ApplicationUpdateService
    {
        /// <summary>
        /// Update args to hold application startup arguments.
        /// </summary>
        internal static string UpdateArgs;
        internal static bool UpdateChecksVisible; // = true;
        private static System.Threading.Timer timer;
        private static int timerCount;
        private const int timerInterval = 600000; // 10min
        private const int appTimerMultiple = 6; // 60min (every 6th dbTimerInterval)
        internal static bool RunUpdateOnShutdown;
        private static bool updateProcessRunning;

        internal static void StartUpdateMonitor()
        {
            if (timer == null)
                timer = new System.Threading.Timer(UpdateTimerFired, null, 0, timerInterval);

            // run templates update (if there is anything new to update)
            TemplatesUpdateService.UpdateTemplatesAsync();
        }

        private static void UpdateTimerFired(object state)
        {
            // disables database updates ****************
            if (App.DisableDatabaseUpdateScripts) return;
            // ******************************************

            if (updateProcessRunning) return;
            try
            {
                updateProcessRunning = true;
                bool doDbForcedUpdate;
                if (timerCount == 0)
                {
                    DatabaseUpdateService.RunDatabaseUpdate();
                    doDbForcedUpdate = false;
                }
                else
                {
                    try { doDbForcedUpdate = DatabaseUpdateService.RunVersionCheck(); }
                    catch { doDbForcedUpdate = false; }
                }

                if (timerCount % appTimerMultiple == 0)
                {
                    var updater = new Forms.UpdateForm();
                    if (doDbForcedUpdate) // (timerCount == 0 || doDbForcedUpdate) // TW just opened, or db forced update
                    {
                        updater.RunUpdate(); // visible
                    }
                    else
                    {
                        updater.RunInstall(); // silent
                    }
                }
            }
            finally
            {
                timerCount++;
                updateProcessRunning = false;
            }
        }

        internal static void RunUpdate()
        {
            new Forms.UpdateForm().Run();
        }

        internal static bool CheckForLocalUpdate()
        {
            if (File.Exists(App.File_UpdateExe))
            {
                var updateInfo = FileVersionInfo.GetVersionInfo(App.File_UpdateExe);

                bool isValid = updateInfo.FileVersion != null;
                if (isValid)
                {
                    var updateVersion = new Version(updateInfo.FileVersion);
                    var currentVersion = new Version(AssemblyInfo.FileVersion);

                    isValid = updateVersion > currentVersion;
                }

                if (!isValid)
                {
                    TryDelete(App.File_UpdateExe); // cleanup
                }
                return isValid;
            }
            return false;
        }

        internal static void TryDelete(string file)
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Application.DoEvents();
                File.Delete(file);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
        
        internal class AppUpdateResponse
        {
            internal Uri Uri { get; set; }
            internal Version Version { get; set; }
            internal String Signature { get; set; }
            internal String Description { get; set; }
            public override string ToString()
            {
                return string.Format("Uri:{0} Version:{1} Signature:{2}", Uri, Version, Signature);
            }
        }
    }
}