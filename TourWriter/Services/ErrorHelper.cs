using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Utilities.Xml;

namespace TourWriter.Services
{
    /// <summary>
    /// Provides helper actions to take with errors that occur in the applicaiton
    /// </summary>
    class ErrorHelper
    {
        internal static void HandleError(object exceptionObject)
        {
            if (exceptionObject is Exception)
            {
                var ex = exceptionObject as Exception;

                if (IsServerConnectionError(ex))
                    App.ShowServerConnectionError();

                else if (IsFileAccessError(ex))
                    App.ShowError(ex.Message);

                else if (IsValidationError(ex))
                    App.ShowError(ex.Message);

                else
                    App.Error("An error occured: " + ex.Message, ex);
            }
            else
            {
                App.Error("An unknown error occurred.", new Exception());
            }
        }

        internal static bool IsServerConnectionError(Exception ex)
        {
            if (ex is System.Data.SqlClient.SqlException &&
                (ex.Message.ToLower().Contains("error: 0") ||       // An existing connection was forcibly closed by the remote host...
                 ex.Message.ToLower().Contains("error: 26") ||      // Error Locating Server/Instance Specified...
                 ex.Message.ToLower().Contains("error: 40") ||      // Could not open a connection to SQL Server...
                 ex.Message.ToLower().Contains("timeout expired"))) // System.Data.SqlClient.SqlException: Timeout expired.
            {
                return true;
            }
            if (ex is InvalidOperationException &&
                ex.Message.ToLower().Contains("timeout period elapsed"))
            {
                return true;
            }
            return false;
        }

        internal static bool IsFileAccessError(Exception ex)
        {
            if (ex is System.IO.IOException &&
                ex.Message.ToLower().Contains("process cannot access the file"))
            {
                return true;
            }
            if (ex is UnauthorizedAccessException &&
                ex.Message.ToLower().Contains("access to the path") &&
                ex.Message.ToLower().Contains("is denied"))
            {
                return true;
            }
            if (ex is System.Runtime.InteropServices.COMException &&
                ex.Message.ToLower().Contains("file could not be found"))
            {
                return true;
            }
            if (ex is System.Runtime.InteropServices.COMException &&
                ex.Message.ToLower().Contains("document name or path is not valid"))
            {
                return true;
            }
            if (ex is System.ComponentModel.Win32Exception &&
                ex.Message.ToLower().Contains("no application is associated with the specified file for this operation"))
            {
                return true;
            }
            if (ex is System.ComponentModel.Win32Exception &&
                ex.Message.ToLower().Contains("network path was not found"))
            {
                return true;
            }
            return false;
        }

        internal static bool IsValidationError(Exception ex)
        {
            if (ex is FormatException &&
                ex.Message.Contains("The specified string is not in the form required for an e-mail address"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends an error message email to default support address, on a background thread
        /// </summary>
        /// <param name="ex">The error</param>
        /// <param name="isSilentError">Suppress any errors that occur</param>
        internal static void SendEmail(Exception ex, bool isSilentError)
        {
            ThreadStart ts = () => Send(ex.Message, ex.ToString(), isSilentError, null);
            SendOnThread(ts);
            
        }

        /// <summary>
        /// Sends an error message email to default support address, on a background thread
        /// </summary>
        /// <param name="ex">The error</param>
        /// <param name="isSilentError">Suppress any errors that occur</param>
        /// <param name="attachments">Attachments to include with email</param>
        internal static void SendEmail(Exception ex, bool isSilentError, params Attachment[] attachments)
        {
            ThreadStart ts = () => Send(ex.Message, ex.ToString(), isSilentError, attachments);
            SendOnThread(ts);

        }

        /// <summary>
        /// Sends an error message email to default support address, on a background thread
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="detail">The error details</param>
        /// <param name="isSilentError">If further errors should be suppressed</param>
        internal static void SendEmail(string message, string detail, bool isSilentError)
        {
            ThreadStart ts = () => Send(message, detail, isSilentError, null);
            SendOnThread(ts);
        }

        /// <summary>
        /// Sends an error message email to default support address, on a background thread
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="detail">The error details</param>
        /// <param name="isSilentError">If further errors should be suppressed</param>
        /// <param name="attachments">Attachments to include with email</param>
        internal static void SendEmail(string message, string detail, bool isSilentError, params Attachment[] attachments)
        {
            ThreadStart ts = () => Send(message, detail, isSilentError, attachments);
            SendOnThread(ts);
        }

        private static void SendOnThread(ThreadStart ts)
        {
            var t = new Thread(ts) { Name = "ErrorHelper_Send" };
            t.Start();
        }

        private static void Send(string message, string detail, bool isSilentError, params Attachment[] attachments)
        {
            try
            {
                // Set some default values.
                const string emailTo = "app@tourwriter.com";
                string emailFrom;
                string installId;
                string appVersion;
                string revision;
                string dbVersion;
                string connection;

                try { emailFrom = Cache.User.Email; }
                catch { emailFrom = "client.app@tourwriter.com"; }
                try { installId = Cache.ToolSet.AppSettings[0].InstallID.ToString(); }
                catch { installId = "[failed to load]";  }
                try { revision = AssemblyInfo.RevisionNumber; }
                catch { revision = "[failed to load]"; }
                try { appVersion = AssemblyInfo.FileVersion; }
                catch { appVersion = "[failed to load]"; }
                try { dbVersion = Cache.ToolSet.AppSettings[0].VersionNumber; }
                catch { dbVersion = "[failed to load]"; }
                try { connection = String.Format("{0}\\{1}", App.Servername, Cache.User.UserName); }
                catch { connection = "[failed to load]"; }

                var errorMsg = new ErrorMessage
                                   {
                                       // tourwriter
                                       ErrorType = ("ClientApp" + (isSilentError ? ".Silent" : "")),
                                       TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                       UtcTime = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                                       InstallId = installId,
                                       AppVersion = appVersion,
                                       Revision = revision,
                                       DbVersion = dbVersion,
                                       Connection = connection,

                                       // system
                                       ApplicationName = Application.ProductName,
                                       ComputerName = SystemInformation.ComputerName,
                                       UserName = SystemInformation.UserName,
                                       OsVersion = Environment.OSVersion.VersionString,
                                       NetVersion = App.GetDotNetVersion().ToString(),
                                       Culture = CultureInfo.CurrentCulture.Name,
                                       Resolution = SystemInformation.PrimaryMonitorSize.ToString(),
                                       ApplicationUpTime = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(),

                                       // message
                                       Message = message,
                                       Detail = detail
                                   };

                var emailMsg = new EmailMessage
                                   {
                                       _To = emailTo,
                                       _From = emailFrom,
                                       Subject = "TW_ERROR_REPORT",
                                       Body = XmlHelper.ObjectToXML(typeof (ErrorMessage), errorMsg),
                                       Priority = MailPriority.High
                                   };

                if (attachments != null)
                    foreach (var attachment in attachments)
                        emailMsg.Attachments.Add(attachment);

                emailMsg.Send();
            }
            catch (Exception)
            {
                if (!isSilentError)
                {
                    App.ShowError(
                        "Failed to send error message email to TourWriter support." +
                        "Please send the message manually to 'support@tourwriter.com'.");
                }
            }
        }
    }
}
