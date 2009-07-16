
using System.IO;
using TourWriter.Global;

namespace TourWriter.Services
{
    /// <summary>
    /// Class to manage storage of files outside of the TourWriter database. It encourages user to always
    /// store files within a common external files path.
    /// </summary>
    class ExternalFilesHelper
    {
        /// <summary>
        /// Converts a relative path (external files path) to an absolute file path.
        /// </summary>
        /// <param name="filePath">the file path to check</param>
        /// <returns>Absolute file path.</returns>
        internal static string ConvertToAbsolutePath(string filePath)
        {
            // could use [DllImport("shlwapi.dll")] but may be slow if looking up network paths?

            // Is it a network path?
            if (string.IsNullOrEmpty(filePath) || filePath.StartsWith("\\\\"))
                return filePath;

            // Is it already a full path?
            if (filePath.Length > 1 && filePath[1] == ':' && filePath[2] == '\\')
                return filePath;

            // Treat as a subpath, so prepend the common external path.
            return 
                Cache.ToolSet.AppSettings[0].ExternalFilesPath.TrimEnd('\\') +
                Path.DirectorySeparatorChar +
                filePath.TrimStart('\\');
        }

        /// <summary>
        /// Converts an absolute file path to a path relative to the common external files path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Reletive file path.</returns>
        internal static string ConvertToRelativePath(string filePath)
        {
            string externalPath = Cache.ToolSet.AppSettings[0].ExternalFilesPath;

            if (externalPath.Length > 0 && filePath.StartsWith(externalPath))
                return filePath.Replace(externalPath, "");

            return filePath;
        }
        
        /// <summary>
        /// Gets the TourWriterData folder.
        /// </summary>
        /// <returns>TourWriterData folder path</returns>
        internal static string GetTourWriterDataFolder()
        {
            return Cache.ToolSet.AppSettings[0].ExternalFilesPath.TrimEnd(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the default email folder path (format ..\Email\Sent\yyyy-MM).
        /// </summary>
        /// <returns>Email folder path</returns>
        internal static string GetEmailFolder()
        {
            return
                GetTourWriterDataFolder() + Path.DirectorySeparatorChar +
                "Email" + Path.DirectorySeparatorChar +
                "Sent" + Path.DirectorySeparatorChar +
                System.DateTime.Now.ToString("yyyy-MM");
        }

        /// <summary>
        /// Gets the default external template folder path.
        /// </summary>
        /// <returns>Template folder path</returns>
        internal static string GetTemplateFolder()
        {
            return
                GetTourWriterDataFolder() + Path.DirectorySeparatorChar +
                "Templates";
        }
    }
}