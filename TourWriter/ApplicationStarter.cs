using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using TourWriter.Forms;
using TourWriter.Services.Update;

namespace TourWriter
{
    /// <summary>
    /// Handle base application startup and shutdown tasks (i.e. splash screen, updater).
    /// </summary>
    public class ApplicationStarter : WindowsFormsApplicationBase
    {
        private const bool isSingleInstance = true;
        private string SplashScreenText
        {
            set
            {
                if (SplashScreen != null)
                    ((SplashScreen)SplashScreen).MessageText = value;
            }
        }

        public ApplicationStarter()
        {
            IsSingleInstance = isSingleInstance;
        }

        protected override void OnCreateSplashScreen()
        {
            SplashScreen splash = new SplashScreen();
            splash.VersionText = "Version: " + AssemblyInfo.FileVersion;

            SplashScreen = splash;
            
            MinimumSplashScreenDisplayTime = 2000;
            SplashScreenText = "Loading TourWriter...";
        }

        protected override void OnCreateMainForm()
        {
            if (ApplicationUpdateService.CheckForLocalUpdate())
            {
                SplashScreenText = "Installing update...";
                Thread.Sleep(1000);
                SplashScreenText = "Restarting...";
                Thread.Sleep(1000);

                RunUpdate();
                Application.Exit();
                Environment.Exit(0);
            }
            else
            {
                RunApplication();
            }
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            if (ApplicationUpdateService.RunUpdateOnShutdown)
            {
                RunUpdate();
            }
        }

        private void RunApplication()
        {
            MainForm = new MainForm();
        }

        private static void RunUpdate()
        {
            Application.DoEvents();

            // add 'updating' flag to args
            if (ApplicationUpdateService.UpdateArgs == null) ApplicationUpdateService.UpdateArgs = "";
            if (!ApplicationUpdateService.UpdateArgs.Contains(App.UpdatedStartParam))
                ApplicationUpdateService.UpdateArgs += " " + App.UpdatedStartParam;

            string args = ApplicationUpdateService.UpdateArgs;

            if (!string.IsNullOrEmpty(args))
            {
                Process.Start(App.File_UpdateExe, args);
            }
            else
            {
                Process.Start(App.File_UpdateExe);
            }
        }
    }
}
