using System;
using TourWriter.Info.Services;

namespace TourWriter.Info
{
    public class VersionInfo
    {
        /// <summary>
        /// Loads the database version number from AppSettings table.
        /// </summary>
        public static string GetInstallId()
        {
            object o = SqlHelper.ExecuteScalar(
                ConnectionString.GetConnectionString(),
                System.Data.CommandType.Text,
                "SELECT [InstallID] FROM [AppSettings]");

            return o.ToString();
        }

        /// <summary>
        /// Loads the database version number from AppSettings table.
        /// </summary>
        public static Version GetDatabaseVersion()
        {
            object o = SqlHelper.ExecuteScalar(
                ConnectionString.GetConnectionString(),
                System.Data.CommandType.Text,
                "SELECT [VersionNumber] FROM [AppSettings]");

            return GetVersionFromString(o.ToString());
        }

        /// <summary>
        /// Parses a string version number using the System.Version class,
        /// after adding default minor number if missing. 
        /// </summary>
        /// <param name="versionNumber">String version number.</param>
        /// <returns>Version number in a class.</returns>
        private static Version GetVersionFromString(string versionNumber)
        {
            if (!versionNumber.Contains("."))
                versionNumber += ".0";

            return new Version(versionNumber);
        }
    }
}
