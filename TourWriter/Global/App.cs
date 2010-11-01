using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Data;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using Microsoft.Win32;
using TourWriter.Global;
using TourWriter.Services;

namespace TourWriter
{
    /// <summary>
    /// Global helper methods for running application.
    /// </summary>
    internal class App
    {
        internal const string HelpFilename = "TourWriterHelp.chm";
        internal const string AppsettingFilename = "AppSettings.xml"; // used also in BLLBase	
        internal const string UsersettingFilename = "UserSettings.xml";
        internal const string UserlayoutFilename = "UserLayout.xml";
        internal const string DataErrorCurrencyViolationText = "Concurrency violation";
        internal const string DataErrorPkDeleteConflictText = "The DELETE statement conflicted with the REFERENCE constraint";
        
        internal const int TemplateCategoryBookingEmail = 5;
        internal const string PricingOptionNetMarkupText = "nm";
        internal const string PricingOptionNetGrossText = "ng";
        internal const string PricingOptionGrossCommissionText = "gc";
        internal const string RemoteConnectionName = "(custom server)";
        internal const string AdminUserName = "admin";

        internal static void RefreshMenu(string menuName)
        {
            switch (menuName)
            {
                case "Itinerary":
                    MainForm.InitialiseMenu(MainForm.ItineraryMenu);
                    break;
                case "Supplier":
                    MainForm.InitialiseMenu(MainForm.SupplierMenu);
                    break;
                case "Contact":
                    MainForm.InitialiseMenu(MainForm.ContactMenu);
                    break;
                default:
                    break;
            }
        }

        internal static string Path_TempFolder
        {
            get
            {
                string path = Path.Combine(Path.GetTempPath(), "TourWriter");
                
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        internal static string Path_UserApplicationData
        {
            get
            {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Application.ProductName) + Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        internal static string Path_CommonApplicationData
        {
            get
            {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    Application.ProductName) + Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        internal static string Path_DefaultTemplatesFolder
        {
            get 
            {
                var fileInfo = new FileInfo(File_TempatesUpdateArchive);
                var path = Path.Combine(Path_CommonApplicationData, fileInfo.Name.Replace(fileInfo.Extension, ""));
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        internal static string Path_MyDocumentsDataFolder
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                    Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        internal static string File_HelpPathAndFile = Path.Combine(Application.StartupPath, HelpFilename);
        internal static string File_AppSettingPathAndFile = Path.Combine(Path_CommonApplicationData, AppsettingFilename);
        internal static string File_UserSettingPathAndFile = Path.Combine(Path_UserApplicationData, UsersettingFilename);
        internal static string File_UserLayoutPathAndFile = Path.Combine(Path_UserApplicationData, UserlayoutFilename);
        internal static string File_UpdateExe = Path.Combine(Path_TempFolder, "TourWriterUpdate.exe");
        internal static string File_TempatesUpdateArchive = Path.Combine(Application.StartupPath, "Templates.zip");
        
        internal static Utilities.KeyHook.KeyDefinition DebugKeyCombination =
            new Utilities.KeyHook.KeyDefinition(
                true, false, true, Keys.D, Utilities.KeyHook.KeyState.KeyDown);	// [Shift + Alt + D]

        #region Application variables
        internal static string Servername;
        internal static string UsernameOnDisk = "";
        internal static string PasswordOnDisk = "";
        internal static string TypeOfConnection = "";
        internal static string LANLocation = "";
        internal static string WSLocation = "";
        internal static string UpdatedStartParam = "-updated";

        internal static bool ShowOldReports
        {
            // TODO: Remove this when old reports gone
            get
            {
                return App.IsDebugMode 
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "20D0DC7C-BE6B-4846-83C6-3CC580808959".ToLower() // wldr
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "49AD26FD-6582-493A-B998-B9BA244D082F".ToLower() // sc nz
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "BBE982AB-C8E2-4DB4-8EAC-06801A810540".ToLower() // sc aus
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "FC23968E-28B2-4AE0-AD28-06F6A9AB76A1".ToLower() // btb
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "60DF6FBE-6E29-4AC8-A27D-B4A65CB33B45".ToLower() // 3d
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "8A7E0397-40DC-4389-8A38-9E74A2C32E20".ToLower() // szs
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "7e18060b-4d24-4c6e-a1b9-55f4955f3fc8".ToLower(); // rest
            }
        }
        
        internal static bool ShowNewPublishing
        {
            // TODO: Remove this when new publishing released to all
            get
            {
                return App.IsDebugMode
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "13b8e136-405f-402f-a4bb-3913879be702".ToLower() // dev
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "c34e1d19-1655-467c-a4b6-2cbbe6a19523".ToLower() // glenn
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "9e37e740-6731-4dda-972c-d68baeb25db7".ToLower() // andrea
                       ||
                       Cache.ToolSet.AppSettings.Rows.Count > 0 &&
                       Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() ==
                       "575E7900-BF13-42D1-A661-2242510C3359".ToLower(); // te
            }
        }

        #endregion

        #region Network
        internal static bool CheckHostConnection(string hostName)
        {
            try
            {
                System.Net.Dns.GetHostEntry(hostName);
                return true;
            }
            catch
            {
                return false;
            }
        }


        internal static void StartDefaultMail(string to, string subject, string body)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;

            psi.FileName = String.Format("mailto:{0}?subject={1}&body={2}",
                to,			//System.Web.HttpUtility.HtmlDecode(to),
                subject,	//System.Web.HttpUtility.UrlEncode(subject),
                body);		//System.Web.HttpUtility.UrlEncode(body));

            Process.Start(psi);
        }

        #endregion

        #region Help
        internal static void ShowHelp()
        {
            ShowHelp("");
        }

        internal static void ShowHelp(string defaultPage)
        {
            if (defaultPage == "")
            {
                Help.ShowHelp(MainForm, File_HelpPathAndFile, HelpNavigator.TableOfContents);
            }
            else
            {
                string topicID = String.Format("{0}.htm", defaultPage.Replace(".", "_"));
                Help.ShowHelp(MainForm, File_HelpPathAndFile, HelpNavigator.Topic, topicID);
            }
            if (IsDebugMode)
                ShowInfo("DEBUG INFO: Open help at default page:\r\n\r\n" + defaultPage);
        }

        #endregion

        #region Browser

        internal static void OpenUrl(string url)
        {
            try
            {
                var p = new Process {StartInfo = {FileName = GetDefaultBrowser(), Arguments = url}};
                p.Start();
            }
            catch (Exception ex)
            {
                Error("Failed to start default web browser on this computer", ex, false);
            }
        }

        internal static string GetDefaultBrowser()
        {
            string browser;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                //trim off quotes
                browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                if (!browser.EndsWith("exe"))
                {
                    //get rid of everything after the ".exe"
                    browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                }
            }
            finally
            {
                if (key != null) key.Close();
            }
            return browser;
        }

        #endregion

        #region Dialogs
        /// <summary>
        /// PromptSaveFile - opens the Save File dialog box to get a filename.
        /// </summary>
        /// <param name="defaultFilename">Used for the dialog filter as a description</param>
        /// <param name="fileDescription">Used for the dialog filter as a description</param>
        /// <param name="fileExtension">Used by the dialog filter for the file extension to save to.</param>
        /// <param name="promptOverwrite">Ask the user whether to overwrite the file if it already exists.</param>
        /// <returns>Filename to save to</returns>
        internal static string PromptSaveFile(string defaultFilename, string fileDescription, string fileExtension, bool promptOverwrite)
        {
            string filterTemplate = "{0} (*{1})|*{1}|All files (*.*)|*.*";

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = string.Format(
                CultureInfo.CurrentCulture, filterTemplate, fileDescription, fileExtension);

            if (defaultFilename != "")
                dlg.FileName = defaultFilename;

            dlg.FilterIndex = 0;
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = fileExtension;
            dlg.OverwritePrompt = promptOverwrite;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            else return "";
        }

        internal static string PromptSaveFile(string defaultFilename, string fileDescription, string fileExtension)
        {
            return PromptSaveFile(defaultFilename, fileDescription, fileExtension, true);
        }

        internal static string PromptChooseDir(string startPath)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (startPath != "")
                dlg.SelectedPath = startPath;

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.SelectedPath;

            return "";
        }

        /// <summary>
        /// Uses open file dialog to choose a single file (ensureCommonFilesPath prompts user to keep within the common external files path). 
        /// Returns part file path if file is within external files path, or full path if not, or null if cancelled.
        /// </summary>
        internal static string SelectExternalFile(bool ensureCommonFilesPath, string dialogTitle, string fileTypeFilter, int filterIndex)
        {
            string[] result = PerformOpenExternalFileDialog(ensureCommonFilesPath, dialogTitle, "", fileTypeFilter, filterIndex, false);
            return (result != null && result.Length > 0) ? result[0] : null;
        }

        /// <summary>
        /// Uses open file dialog to choose a single file (ensureCommonFilesPath prompts user to keep within the common external files path). 
        /// Returns part file path if file is within external files path, or full path if not, or null if cancelled.
        /// </summary>
        internal static string SelectExternalFile(bool ensureCommonFilesPath, string dialogTitle, string filename, string fileTypeFilter, int filterIndex)
        {
            string[] result = PerformOpenExternalFileDialog(ensureCommonFilesPath, dialogTitle, filename, fileTypeFilter, filterIndex, false);
            return (result != null && result.Length > 0) ? result[0] : null;
        }

        /// <summary>
        /// Uses open file dialog to choose multiple files (ensureCommonFilesPath prompts user to keep within the common external files path). 
        /// Returns part file path if file is within external files path, or full path if not, or null if cancelled.
        /// </summary>
        internal static string[] SelectExternalFiles(bool ensureCommonFilesPath, string dialogTitle, string fileTypeFilter, int filterIndex)
        {
            return PerformOpenExternalFileDialog(ensureCommonFilesPath, dialogTitle, "", fileTypeFilter, filterIndex, true);
        }

        private static string _lastDir = "";
        private static string[] PerformOpenExternalFileDialog(bool ensureCommonFilesPath, string dialogTitle, string filename, string fileTypeFilter, int filterIndex, bool multiSelect)
        {
            if (ensureCommonFilesPath && !_lastDir.StartsWith(Cache.ToolSet.AppSettings[0].ExternalFilesPath))
                _lastDir = Cache.ToolSet.AppSettings[0].ExternalFilesPath;

            var dlg = new OpenFileDialog
                          {
                              Title = dialogTitle,
                              Filter = fileTypeFilter,
                              FilterIndex = filterIndex,
                              RestoreDirectory = false,
                              Multiselect = multiSelect
                          };
            if (filename != "") dlg.FileName = filename;
            dlg.InitialDirectory = _lastDir;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _lastDir = Path.GetDirectoryName(dlg.FileNames[0]);

                // Return any file.
                if (!ensureCommonFilesPath)
                    return dlg.FileNames;

                // Return file from within the common external files path.
                var fileNotInCommonPath = false;
                var result = new string[dlg.FileNames.Length];

                for (var i = 0; i < dlg.FileNames.Length; i++)
                {
                    if (Cache.ToolSet.AppSettings[0].ExternalFilesPath != ""
                        && dlg.FileNames[i].StartsWith(Cache.ToolSet.AppSettings[0].ExternalFilesPath))
                    {
                        // Remove common part of file path.
                        result[i] = dlg.FileNames[i].Replace(Cache.ToolSet.AppSettings[0].ExternalFilesPath, "");
                    }
                    else
                    {
                        result[i] = dlg.FileNames[i];
                        fileNotInCommonPath = true;
                    }
                }

                if (fileNotInCommonPath)
                {
                    const string msg = "Warning: you have selected a file that is outside of your external files location (Tools->Options->Administration).\r\n" +
                                       " It is strongly recommended that you move this file into the external files location, before adding it here.\r\n\r\n" +
                                       "Click OK to continue anyway, or Cancel to canel this operation";

                    if (MessageBox.Show(msg, MessageCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                        == DialogResult.Cancel)
                    {
                        return null;
                    }
                }
                return result;
            }
            return null;
        }

        #endregion

        #region File and IO
        /// <summary>
        /// Returns the path of the current systems temp folder.
        /// </summary>
        internal static string TempFolder
        {
            get
            {
                return Path.GetTempPath();
            }
        }

        internal static bool IsValidFileName(string fileName)
        {
            try
            {
                FileInfo f = new FileInfo(fileName);
                if (f.Name == "")
                    throw new Exception();

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static string GetFileName(string filePathAndName)
        {
            String[] s = filePathAndName.Split('\\');
            int index = s.GetUpperBound(0);
            return s[index];
        }

        internal static string StripInvalidFileNameChars(string fileName, string replacement)
        {
            string pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
            return Regex.Replace(fileName, pattern, replacement);
        }

        internal static string StripInvalidFileNameChars(string fileName)
        {
            return StripInvalidFileNameChars(fileName, " ");
        }
        #endregion

        #region System

        internal static string GetDotNetVersion()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            if (key == null) return null;

            var versions = key.GetSubKeyNames();
            if (versions.Length == 0) return "";

            var latest = key.OpenSubKey(versions[versions.Length - 1]);
            if (latest == null) return "";

            if (latest.GetValue("Version") != null) 
                return latest.GetValue("Version").ToString();

            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4
            return Regex.Match(latest.Name, @"(?<=\\v)(\d+\.?[\d\.]*)").Value;
        }

        internal static void EnsureInstallId()
        {
            // update InstallID if database using default id.
            if (Cache.ToolSet.AppSettings.Count > 0 &&
                (Cache.ToolSet.AppSettings[0].IsInstallIDNull() || 
                 Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "11111111-1111-1111-1111-111111111111".ToLower()))
            {
                Cache.ToolSet.AppSettings[0].InstallID = Guid.NewGuid();
                Cache.SaveToolSet();
            }
        }

        #endregion

        #region Application
        internal static Forms.MainForm MainForm;
        internal static string MessageCaption
        {
            get { return GetResourceString("MessageCaption"); }
        }

        internal static DateTime LastActive = DateTime.Now;
        internal static bool IsActive
        {
            get { return (DateTime.Now - LastActive).Hours < 1; }
        }

        private static bool isDebugMode;
        internal static bool IsDebugMode
        {
            // debug combination Shift + Alt + D

            get { return isDebugMode; }

            set { isDebugMode = value; }
        }

        private static string _x;
        private static string _y;
        internal static string Test
        {
            get
            {
                if (_y == null && !string.IsNullOrEmpty(_x))
                {
                    _y = "";
                    try
                    {
                        int i;
                        var o = Info.Services.DatabaseHelper.ExecuteScalar(_x);
                        if (o != null && int.TryParse(o.ToString(), out i)) _y = i.ToString();
                    }
                    catch { _y = "error"; }
                }
                return _y;
            }
            set { _x = value; if (string.IsNullOrEmpty(_x)) _y = null; }
        }

        private static string ListLoadedAssemblies()
        {
            var count = 1;
            var strbuf = new System.Text.StringBuilder();
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                string name = assembly.FullName;
                strbuf.Append(String.Format("({0}) {1}\r\n", count++, name.Substring(0, name.IndexOf(','))));
            }
            return strbuf.ToString();
        }

        internal static void SetCultureInfo()
        {
            // var cultureInfo = new CultureInfo(1066); // Vietnamese Dong (VND)
            // http://stackoverflow.com/questions/1071273/currency-formatting/1071302#1071302
            if (Cache.ToolSet.AppSettings[0].IsCurrencyCodeNull() || string.IsNullOrEmpty(Cache.ToolSet.AppSettings[0].CurrencyCode.Trim())) return;
            var cultureInfo = App.GetCultureInfo(Cache.ToolSet.AppSettings[0].CurrencyCode);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        /// <summary>
        /// Use in place of 'buggy' System.ComponentModel.Component.DesignMode
        /// </summary>
        internal static bool IsInDesignMode
        {
            get
            {
                return (Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }
        
        #endregion

        #region User settings

        public static bool EnableGroups
        {
            get
            {
                return Properties.Settings.Default.EnableGroups;
            }
        }

        #endregion

        #region Resource files

        internal static string GetResourceString(string key)
        {
            System.Resources.ResourceManager res = new System.Resources.ResourceManager("TourWriter.Messages",
                                                                                        System.Reflection.Assembly.GetExecutingAssembly());

            return res.GetString(key);
        }

        #endregion

        #region Regular expressions
        /// <summary>
        /// Validate a list of RFC 2822 formated email addresses, where each is separated by a comma or semicolon. 
        /// </summary>
        /// <param name="emailAddresses"></param>
        /// <returns>True if valid email address(es), otherwise false.</returns>
        internal static bool ValidateEmailAddresses(string emailAddresses)
        {
            bool isValid = true;

            // split multiple addresses on ',' and ';'
            string testString = emailAddresses.Replace(",", ";");

            foreach (string email in testString.Split(';'))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                        email.Trim(),

                        // accepts RFC 2822 email addresses in the form: 'blah@blah.com' OR 'Blah <blah@blah.com>'.
                        @"^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$"

                        //@"^(([^()<>@,;:\\\"".[\]\ ]+)|(\""[^\r\""]+\""))((\.[^()<>@,;:\\\"".[\]\ ]+)|(\.\""[^\r\""]+\""))*@((([a-zA-Z0-9\-]+\.)*([a-zA-Z0-9][a-zA-Z0-9\-]+)*[a-zA-Z0-9]+\.[a-zA-Z]{2,})|(\[(25[0-4]|2[0-4]\d|1\d{2}|[1-9]\d?)(\.(25[0-4]|2[0-4]\d|1\d{2}|[1-9]\d|\d)){2}(\.(25[0-4]|2[0-4]\d|1\d{2}|[1-9]\d?)))\])$"
                        ))
                    isValid = false;
            }
            return isValid;
        }

        #endregion

        #region Form helper methods

        /// <summary>
        /// Forces a data bound control to end edits
        /// </summary>
        /// <param name="control">The control with bindings</param>
        /// <param name="propertyName">The binding property of the control</param>
        internal static void BindingsForceEndEdit(Control control, string propertyName)
        {
            /* Get the binding – could have manually created and added the binding as well */
            Binding adoBinding = control.DataBindings[propertyName];

            adoBinding.FormattingEnabled = true;
            adoBinding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            /* Force EndEdit for ADO.NET */
            adoBinding.BindingComplete += delegate(object binding, BindingCompleteEventArgs args)
            {
                if ((args.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate) &&
                    (args.BindingCompleteState == BindingCompleteState.Success))
                {
                    DataRowView drv = (args.Binding.BindingManagerBase.Current as DataRowView);

                    /* Force ADO.NET to commit the value */
                    if (null != drv)
                    {
                        drv.EndEdit();
                    }
                }
            };
        }

        /// <summary>
        /// Clean up bound objects so they will be garbage-collected and the container (Form) released.
        /// </summary>
        internal static void ClearBindings(Control control)
        {
            Binding[] bindings = new Binding[control.DataBindings.Count];
            control.DataBindings.CopyTo(bindings, 0);
            control.DataBindings.Clear();
            if (control is UltraGrid)
                (control as UltraGrid).DisplayLayout.ValueLists.Clear();

            foreach (Binding binding in bindings)
            {
                System.ComponentModel.TypeDescriptor.Refresh(binding.DataSource);
            }
            foreach (Control c in control.Controls)
            {
                ClearBindings(c);
            }
        }

        /// <summary>
        /// Commit open edits in all UltraGrid controls in the container control.
        /// </summary>
        /// <param name="container">The container control containing UltraGrid controls.</param>
        internal static void CommitGridEdits(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is UltraGrid)
                {
                    GridHelper.HandleInvalidGridEdits(c as UltraGrid, true);
                    (c as UltraGrid).UpdateData();
                }
                else
                {
                    CommitGridEdits(c);
                }
            }
        }

        /// <summary>
        /// Creates a unique value in format "Test Value (2)" where "Test Value" is the value to test for
        /// </summary>
        /// <param name="testRows">Rows to search</param>
        /// <param name="testCol">Column to search</param>
        /// <param name="testValue">Value to search for</param>
        /// <returns></returns>		
        internal static string CreateUniqueNameValue(RowsCollection testRows, string testCol, string testValue)
        {
            bool testValueExists = false;
            ArrayList existingIntValues = new ArrayList();

            foreach (UltraGridRow row in testRows)
            {
                string rowValue = row.Cells[testCol].Value.ToString();

                // look for format "abc (123)"
                if (rowValue.StartsWith(testValue + " (") && rowValue.EndsWith(")"))
                {
                    try
                    {
                        int start = (testValue + " (").Length;
                        existingIntValues.Add(int.Parse(rowValue.Substring(start, rowValue.Length - start - 1)));
                    }
                    catch { }
                }
                else if (rowValue == testValue)
                    testValueExists = true;
            }

            if (!testValueExists)
                return testValue;
            else
            {
                int i = 2;
                while (true)
                {
                    if (!existingIntValues.Contains(i))
                        return testValue + " (" + i + ")";
                    i++;
                }
            }
        }

        internal static string CreateUniqueNameValue(TreeNodesCollection testRows, string testValue)
        {
            bool testValueExists = false;
            ArrayList existingIntValues = new ArrayList();

            foreach (UltraTreeNode row in testRows)
            {
                string rowValue = row.Text;

                // look for format "abc (123)"
                if (rowValue.StartsWith(testValue + " (") && rowValue.EndsWith(")"))
                {
                    try
                    {
                        int start = (testValue + " (").Length;
                        existingIntValues.Add(int.Parse(rowValue.Substring(start, rowValue.Length - start - 1)));
                    }
                    catch { }
                }
                else if (rowValue == testValue)
                    testValueExists = true;
            }

            if (!testValueExists)
                return testValue;
            else
            {
                int i = 2;
                while (true)
                {
                    if (!existingIntValues.Contains(i))
                        return testValue + " (" + i + ")";
                    i++;
                }
            }
        }

        internal static void Tree_SynchroniseChildNodes(UltraTreeNode node)
        {
            foreach (UltraTreeNode childNode in node.Nodes)
            {
                childNode.CheckedState = node.CheckedState;
                Tree_SynchroniseChildNodes(childNode);
            }
        }

        internal static void Tree_SynchroniseParentNodes(UltraTreeNode node)
        {
            if (node.Parent != null)
            {
                //bool syncParent = true;

                if (node.CheckedState == CheckState.Checked)
                {	// check
                    node.Parent.CheckedState = CheckState.Checked;
                }
                //				else
                //				{	// uncheck
                //					foreach(UltraTreeNode n in node.Parent.Nodes)
                //					{
                //						if(node != n && n.CheckedState == System.Windows.Forms.CheckState.Checked)
                //						{
                //							syncParent = false;
                //							return;
                //						}
                //					}
                //				}
                // recurs.
                //if(syncParent)
                Tree_SynchroniseParentNodes(node.Parent);
            }
        }


        internal static System.Drawing.Rectangle DragDrop_SetDragStartPosition(int X, int Y)
        {
            // Remember the point where the mouse down occurred. The DragSize indicates
            // the size that the mouse can move before a drag event should be started.
            System.Drawing.Size dragSize = SystemInformation.DragSize;

            // Create a rectangle using the DragSize, with the mouse position being
            // at the center of the rectangle.
            return new System.Drawing.Rectangle(
                new System.Drawing.Point(X - (dragSize.Width / 2), Y - (dragSize.Height / 2)),
                dragSize);
        }


        /// <summary>
        /// Determines whether a key press is valid input for a number key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the key press event data.</param>
        /// <returns>
        /// 	<c>true</c> if the key press is allowed for number, otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsKeyPressAllowedForNumber(KeyPressEventArgs e)
        {
            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if (keyInput.Equals(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                // Decimal separator is OK
            }
            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            else if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            {
                // Let the edit control handle control and alt key combinations
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion

        #region String format masks
        /// <summary>
        /// Gets the local short date format string, eg. dd/MM/yyyy
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortDateFormat()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        /// <summary>
        /// Gets the local short time format string, eg. H:mm tt
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortTimeFormat()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
        }

        /// <summary>
        /// Gets the local short time (24 hour) format string, eg. H:mm
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortTime24HrFormat()
        {
            return GetLocalShortTimeFormat().Replace("h", "H").Replace(" tt", "");
        }

        /// <summary>
        /// Gets the local short date mask string, eg. dd/mm/yyyy
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortDateMask()
        {
            EditorWithMask editorWithMask = new EditorWithMask();
            return editorWithMask.CalcDefaultDateMask(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the local short time mask string, eg. hh:mm tt
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortTimeMask()
        {
            EditorWithMask editorWithMask = new EditorWithMask();
            return editorWithMask.CalcDefaultTimeMask(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the local short time (24 hour) mask string, eg. hh:mm
        /// </summary>
        /// <returns></returns>
        internal static string GetLocalShortTime24HrMask()
        {
            return GetLocalShortTimeMask().Replace(" tt", "");
        }
        
        /// <summary>
        /// Gets the CultureInfo for a given Currency Code string. Eg: string.Format(GetCultureInfo("VND", "{0:C}", value);
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns>Matching CultureInfo, or default</returns>
        internal static CultureInfo GetCultureInfo(string currencyCode)
        {
            if (!string.IsNullOrEmpty(currencyCode))
            {
                var cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures).
                    Where(cc => new RegionInfo(cc.LCID).ISOCurrencySymbol == currencyCode).FirstOrDefault();

                if (cultureInfo == null) ShowError("Currency Code not valid: " + currencyCode + ". Check that it is a valid ISO 4217 code.");

                return cultureInfo;
            }
            return Thread.CurrentThread.CurrentCulture;
        }

        #endregion

        #region DataSet helper methods
        internal static bool DataSet_CheckForErrors(DataSet ds)
        {
            if (ds.HasErrors)
            {
                Forms.DataErrorViewer dv = new Forms.DataErrorViewer(ds);
                if (dv.HasErrors)
                {
                    dv.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        internal static bool DataSet_AskSaveDeleteConstraints(DataSet ds)
        {
            if (ds.HasChanges())
            {
                if (AskYesNo(
                    "Cannot delete record that is used historically (i.e. service used in booking).\r\n\r\n" +
                    "Do you want to make the data hidden instead?"))
                {
                    return true;
                }
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.GetChanges() != null)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr.RowState == DataRowState.Modified &&
                                    dr.Table.Columns.Contains("IsDeleted") &&
                                    dr["IsDeleted"] != DBNull.Value &&
                                        (bool)dr["IsDeleted"])
                                {
                                    dr.RejectChanges();
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static void PrepareDataTableForExport(DataTable table)
        {
            for (int i = table.Columns.Count - 1; i >= 0; i--)
            {
                if (table.Columns[i].DataType == typeof(Decimal))
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[table.Columns[i]] != DBNull.Value)
                            row[table.Columns[i]] = Decimal.Round((Decimal)row[table.Columns[i]], 2);
                    }
                }
                else if (table.Columns[i].DataType == typeof(DateTime))
                {
                    string newName = table.Columns[i].ColumnName;
                    string oldName = table.Columns[i].ColumnName + "_OLD";
                    table.Columns[i].ColumnName = oldName;
                    table.Columns.Add(newName, typeof (String));

                    foreach (DataRow row in table.Rows)
                    {
                        if (row[oldName] != DBNull.Value)
                            row[newName] = ((DateTime)row[oldName]).ToShortDateString();
                    }

                    table.Columns.Remove(oldName);
                }
            }
        }

        internal static string DataRowsToCsv(DataRow[] rows, string column)
        {
            string values = String.Empty;
            
            for (int i = 0; i < rows.Length; i++)
            {
                values += rows[i][column];

                if (i < rows.Length - 1)
                    values += ",";
            }

            return values;
        }
        #endregion

        #region Debugging helper methods
        private static Forms.DataSourceViewer dataSourceViewer;

        internal static void ViewDataSet(DataSet ds)
        {
            try
            {
                // open existing
                dataSourceViewer.Show();
            }
            catch
            {
                // open new
                dataSourceViewer = null;
                dataSourceViewer = new Forms.DataSourceViewer();
                dataSourceViewer.Show();
            }

            // set datasource
            dataSourceViewer.DataSourceToView = ds;
        }

        internal static void WriteXmlToDesktop(DataSet ds, string filename)
        {
            if (ds == null) ds = new DataSet();
            string s = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ds.WriteXml(Path.Combine(s, filename), XmlWriteMode.DiffGram);
        }

        #endregion

        #region Notification messages and Error handling
        internal static void Error(Exception ex)
        {
            Error(ex, true);
        }

        internal static void Error(Exception ex, bool sendSupportEmail)
        {
            //Services.Logger.Current.Error(ex);
            var f = new Forms.MessageForm(ex) {SendSupportEmail = sendSupportEmail};
            f.ShowDialog();
        }

        internal static void Error(string message, Exception ex)
        {
            Error(message, ex, true);
        }

        internal static void Error(string message, Exception ex, bool sendSupportEmail)
        {
            //Services.Logger.Current.Error(message, ex);
            var f = new Forms.MessageForm(message, ex) { SendSupportEmail = sendSupportEmail };
            f.ShowDialog();
        }

        internal static void Debug(string message)
        {
            if (message == "")
                message = "<Debug message was empty>";

            System.Diagnostics.Debug.WriteLine("[TW] " + message);
        }

        /// <summary>
        /// Warns user if file not in external file path. Returns True if add this file anyway, False if cancelled.
        /// </summary>
        internal static bool WarnFileNotInExternalFilesPath()
        {
            string msg =
                "Warning: you have selected a file that is outside of your external files location " +
                "(Tools->Options->Administration). It is strongly recommended that you first move " +
                "this file into the external files location, before adding this link./r/n" +
                "Do you want to continue to add this file anyway?";

            return MessageBox.Show(msg, MessageCaption, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }


        internal static void ShowInfo(string text)
        {
            MessageBox.Show(text, MessageCaption, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        internal static void ShowInfoThreadSafe(Form parent, string text)
        {
            parent.Invoke(
                (MethodInvoker)
                delegate
                {
                    MessageBox.Show(parent, text, MessageCaption, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                });
        }

        internal static void ShowWarning(string text)
        {
            MessageBox.Show(
                text,
                MessageCaption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        internal static void ShowError(string text)
        {
            MessageBox.Show(
                text,
                MessageCaption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        internal static void ShowNotImplimented()
        {
            MessageBox.Show(GetResourceString("ShowNotImplimented"), MessageCaption);
        }

        internal static void ShowFileNotFound(string fileName)
        {
            MessageBox.Show(GetResourceString("ShowFileNotFound").Trim() + ": " + fileName, MessageCaption);
        }

        internal static bool AskYesNo(string msg)
        {
            return MessageBox.Show(
                       msg, MessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        /// <summary>
        /// Invokes call to message box on same thread as 'parent'.
        /// </summary>
        /// <param name="parent">The parent form.</param>
        /// <param name="msg">The message.</param>
        /// <returns></returns>
        internal static bool AskYesNoThreadSafe(Form parent, string msg)
        {
            bool result = false;
            parent.Invoke(
                (MethodInvoker)
                delegate
                {
                    result = MessageBox.Show(
                                 parent, msg, MessageCaption,
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes;
                });
            return result;
        }

        internal static bool AskDeleteRow()
        {
            return AskDelete(GetResourceString("AskDeleteRow"));
        }

        internal static bool AskDeleteRows(int rowCount)
        {
            return AskDelete(String.Format(
                "Are you sure you want to delete {0} items?", rowCount));
        }

        internal static bool AskDelete(string msg)
        {
            return
                MessageBox.Show(
                    msg,
                    MessageCaption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        internal static bool AskCreateRow()
        {
            return
                MessageBox.Show(GetResourceString("AskCreateRow"), MessageCaption,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        internal static bool ShowCheckPermission(AppPermissions.Permissions permission)//int permissionID)
        {
            if (AppPermissions.UserHasPermission(permission))
                return true;
            
            MessageBox.Show(GetResourceString("ShowPermissionDenied"));
            return false;
        }

        internal static void ShowServerConnectionError()
        {
            ShowWarning("Unable to connect to database server.\r\n\r\nProbable cause: network connection or server is busy or not working.");
        }
        #endregion
    }
}
