using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Services.Update
{
    /// <summary>
    /// Service to handle database upgrade process.
    /// </summary>
    class DatabaseUpdateService
    {
        const string UpgradeScriptsPath = "TourWriter.Services.Update.DatabaseUpdateScripts.";

        /// <summary>
        /// Checks if database version is newer than application version, prompting user to update applicaiton if required.
        /// </summary>
        internal static bool RunVersionCheck()
        {
            var dbVersion = VersionInfo.GetDatabaseVersion();
            foreach (string resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (resourceName.StartsWith(UpgradeScriptsPath))
                {
                    Version updVersion = GetUpdateScriptVersion(resourceName);
                    if (updVersion == dbVersion) return false; // found update script for current db version
                }
            }
            var accept = App.AskYesNoThreadSafe(
                App.MainForm,
                "IMPORTANT - TourWriter update is required!\r\n\r\n" +
                "Your application is older and may be incompatible with your database version, probably because another user has already updated.\r\n\r\n" +
                "Please click YES to update your TourWriter now...");

            return accept;
        }

        /// <summary>
        /// Checks the current version and starts update if required.
        /// </summary>
        internal static void RunDatabaseUpdate()
        {
            try
            {
                var requiredUpgrades = GetRequiredUpdateScripts(VersionInfo.GetDatabaseVersion());
                if (requiredUpgrades.Count > 0)
                {
                    PerformUpdates(requiredUpgrades);
                    PerformPostUpdateTasks();
                }
            }
            catch (Exception ex)
            {
                if (!ErrorHelper.IsServerConnectionError(ex)) throw;
                // else fail silently
            }
        }

        /// <summary>
        /// Backups up and then updates the database.
        /// </summary>
        /// <param name="updateScripts">The update scripts to run.</param>
        private static void PerformUpdates(IEnumerable<KeyValuePair<Version, string>> updateScripts)
        {
            string currentOperation = "";
            try
            {
                currentOperation = "backup database before update";
                Info.Services.DatabaseHelper.Backup();

                foreach (KeyValuePair<Version, string> update in updateScripts)
                {
                    currentOperation = "run database update script " + update.Key;
                    Info.Services.DatabaseHelper.RunScriptWithTransaction(update.Value);
                    App.Debug(" -- " + currentOperation);
                }
            }
            catch (Exception ex)
            {
                App.Error(string.Format(
                              "Failed to complete update step \"{0}\", this step has been rolled back and update cancelled at that point." , currentOperation), ex);
            }
        }

        /// <summary>
        /// Refreshes UI bound data on the UI thread.
        /// </summary>
        private static void PerformPostUpdateTasks()
        {
            App.MainForm.Invoke( // to protect UI databindings
                (MethodInvoker)
                delegate
                    {
                        try
                        {
                            Cache.RefreshToolSet(false);
                        }
                        catch (Exception ex)
                        {
                            App.Error(
                                "Failed to refresh screen data after database update, please restart TourWriter.",
                                ex);
                        }
                    });
        }

        /// <summary>
        /// Gets a sorted list of upgrade scripts required to upgrade the database from the current version.
        /// Looks for the scripts in the resources list in the TourWriter assembly.
        /// Example resource name: "TourWriter.Services.Upgrade.DatabaseUpgradeScripts.2007.8.8.sql"
        /// </summary>
        /// <param name="currentVersion">Current version to upgrade from.</param>
        /// <returns>List of required upgrade scripts, where key as version number (sorted), 
        /// and value is the script for that version.</returns>
        private static SortedList<Version, string> GetRequiredUpdateScripts(Version currentVersion)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sortedList = new SortedList<Version, string>();

            // Enumerate the resources list, looking for upgrade script files.
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(UpgradeScriptsPath))
                    continue; // not an upgrade script

                if (!Regex.IsMatch(resourceName, @"\d{4}\.\d{2}\.\d{2}\.sql$"))
                    continue; // not valid file name (expects yyyy.MM.dd.sql)

                var scriptVersion = GetUpdateScriptVersion(resourceName);
                if (currentVersion >= scriptVersion)
                    continue; // old script not needed for this upgrade

                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;
                using (var reader = new StreamReader(stream))
                {
                    sortedList.Add(scriptVersion, reader.ReadToEnd());
                }
            }
            return sortedList;
        }

        /// <summary>
        /// Extracts the version number part from a resource file name.
        /// </summary>
        /// <param name="resourceName">The resource file name.</param>
        /// <returns>The version number.</returns>
        private static Version GetUpdateScriptVersion(string resourceName)
        {
            // strip off the namespace
            resourceName = resourceName.Replace(UpgradeScriptsPath, "");

            // strip off the file extension
            resourceName = Path.GetFileNameWithoutExtension(resourceName);

            return new Version(resourceName);
        }
    }
}
