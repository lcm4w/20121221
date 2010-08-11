using System;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Services;

namespace TourWriter
{
    internal static class ApplicationEntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                // catch unhandled exceptions with global handlers				
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;

                // set application styles
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // start applicaiton
                var application = new ApplicationStarter();
                application.Run(args);
                
            }
            catch (Exception ex)
            {
                ErrorHelper.HandleError(ex);
            }
        }

        #region Global exception handling
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorHelper.HandleError(e.ExceptionObject);
        }

        private delegate void SafeApplicationThreadException(object sender, ThreadExceptionEventArgs e);
       
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            SafeApplication_ThreadException(sender, e);
        }
        
        private static void SafeApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (App.MainForm != null && App.MainForm.InvokeRequired)
            {
                App.MainForm.Invoke(
                    new SafeApplicationThreadException(SafeApplication_ThreadException),
                    new[] {sender, e});
            }
            else
            {
                ErrorHelper.HandleError(e.Exception);
            }
        }

        #endregion
    }
}