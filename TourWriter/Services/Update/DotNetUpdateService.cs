using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace TourWriter.Services.Update
{
    class DotNetUpdateService
    {
        static readonly Version UpdateVersion = new Version("3.5.30729.01");
        const string RemoteFile = "http://go.microsoft.com/fwlink/?LinkId=118077";
        static readonly string LocalFile = Path.Combine(App.Path_TempFolder, "dotnetfx35setup.exe");

        internal static void GetDotNet()
        {
            var ts = new ThreadStart(
                delegate
                {
                    if (DotNetUpdateRequired() && !File.Exists(LocalFile))
                    {
                        try { new WebClient().DownloadFile(new Uri(RemoteFile), LocalFile); }
                        catch (WebException ex) { ErrorHelper.SendEmail(ex, true); }
                    }
                });
            new Thread(ts) { Name = "DotNET_UpdateCheck" }.Start();
        }

        internal static void RunDotNet()
        {
            const string msg = 
@"TourWriter requires update to Microsoft .NET Framework, please click yes to run update now.

This update is required for the upcoming Groups functionality.
After starting the update, you can leave it to run (approx 10 min).

Start update now?
";

            if (DotNetUpdateRequired() && File.Exists(LocalFile))
                if (App.AskYesNo(msg))
                    Process.Start(LocalFile);
        }

        private static bool DotNetUpdateRequired()
        {
            var s = App.GetDotNetVersion();
            if (!s.Contains(".")) s += ".0";
            return new Version(s) < UpdateVersion;
        }
    }
}
