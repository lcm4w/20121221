using System;
using Ionic.Zip;
using System.IO;
using System.Threading;

namespace TourWriter.Services.Update
{
    class TemplatesUpdateService
    {
        internal static void UpdateTemplatesAsync()
        {
            ThreadPool.QueueUserWorkItem(state => UpdateTemplates());
        }

        internal static void UpdateTemplates()
        {
            var sourceArchive = App.File_TempatesUpdateArchive;
            if (!File.Exists(sourceArchive)) return;
            var targetDir = App.Path_DefaultTemplatesFolder;
            var backupDir = targetDir + "-backup";

            try
            {
                // backup
                if (Directory.Exists(backupDir)) Directory.Delete(backupDir,true);
                Directory.Move(targetDir, backupDir);

                // update
                ExtractArchive(sourceArchive, App.Path_CommonApplicationData);
                App.Debug("Tempates archive decompressed to " + targetDir);

                // tidy
                ApplicationUpdateService.TryDelete(sourceArchive);
                if (Directory.Exists(backupDir)) Directory.Delete(backupDir,true);
            }
            catch (Exception ex)
            {
                ErrorHelper.SendEmail(ex, true);
                if (!Directory.Exists(targetDir) && Directory.Exists(backupDir)) Directory.Move(backupDir, targetDir);
            }
        }

        public static void ExtractArchive(string archive, string directory)
        {
            using (var zip = new ZipFile(archive))
            {
                zip.ExtractAll(directory, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
