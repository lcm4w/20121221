using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using TourWriter.Global;
using TourWriter.Properties;
using System.Linq;
using System.Collections.Specialized;
using TourWriter.Services.Update;

namespace TourWriter.Forms
{
    internal partial class UpdateForm : Form
    {
        private const string MinUpdateableVersion = "2009.9.29"; // post ReportServices v10
        private UiState currentState;
        private readonly WebClient webClient;
        private ApplicationUpdateService.AppUpdateResponse updateReponse;
        internal enum UiState { Start, Check, None, Update, FullInstall, Download, Manual, Install, Error }
        private Exception _error;


        internal UpdateForm()
        {
            InitializeComponent();

            chkNoUpdates.Visible = false;
            chkNoUpdates.Checked = !Services.Update.ApplicationUpdateService.UpdateChecksVisible;
            webClient = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload) };
            webClient.DownloadProgressChanged += DownloadProgressChanged;
        }

        internal void Run()
        {
            SetUI(UiState.Start);
            SetVisible();
        }

        internal void RunUpdate()
        {
            try
            {
                UpdateCheck();
                if (IsUpdateAvailable)
                {
                    if (IsFullSetupRequired)
                    {
                        SetUI(UiState.FullInstall);
                        chkNoUpdates.Checked = false;
                    }
                    else
                    {
                        if (CheckForValidLocalUpdate(updateReponse))
                            SetUI(UiState.Install);
                        else SetUI(UiState.Update);
                    }
                    SetVisible();
                }
                else Close();
            }
            catch (Exception ex) { HandleError(ex); }
        }

        internal void RunInstall()
        {
            try
            {
                UpdateCheck();
                if (IsUpdateAvailable)
                {
                    if (IsFullSetupRequired)
                    {
                        SetUI(UiState.FullInstall);
                        chkNoUpdates.Checked = false;
                    }
                    else
                    {
                        if (CheckForValidLocalUpdate(updateReponse))
                            SetUI(UiState.Install);
                        else
                        {
                            DownloadFile();
                            var verified = CheckForValidLocalUpdate(updateReponse);
                            if (!verified) 
                                Close();
                            else 
                                SetUI(UiState.Install);
                        }
                    }
                    if (chkNoUpdates.Checked) Close();
                    else SetVisible();
                }
                else Close();
            }
            catch (Exception ex) { HandleError(ex); }
        }

        private void HandleError(Exception ex)
        {
            _error = ex;
            if (Visible) SetUI(UiState.Error);
            if (!Services.ErrorHelper.IsWebServerConnectionError(ex) && !Services.ErrorHelper.IsServerConnectionError(ex) 
                && !(ex is System.Data.SqlClient.SqlException && ex.ToString().ToLower().Contains("login failed for user")))
                Services.ErrorHelper.SendEmail(ex, true);
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (currentState)
                {
                    case UiState.Start:
                        UpdateCheckAsync();
                        break;
                    case UiState.Update:
                        DownloadFileAsync();
                        break;
                    case UiState.Install:
                        CloseApplicationForUpdate();
                        break;
                    case UiState.FullInstall:
                        DownloadToBrowser(updateReponse.Uri);
                        DialogResult = DialogResult.Cancel;
                        break;
                    case UiState.Manual:
                        DownloadToBrowser(updateReponse.Uri);
                        DialogResult = DialogResult.Cancel;
                        break;
                }
            }
            catch (Exception ex) { HandleError(ex); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (webClient != null)
            {
                webClient.CancelAsync();
                webClient.Dispose();
            }
            DialogResult = DialogResult.Cancel;
        }

        private void chkNoUpdates_CheckedChanged(object sender, EventArgs e)
        {
            Services.Update.ApplicationUpdateService.UpdateChecksVisible = !chkNoUpdates.Checked;
            //chkNoUpdates.ForeColor = chkNoUpdates.Checked ? Color.Red : Color.Black;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Updater_FormClosed(object sender, FormClosedEventArgs e)
        {
            CleanUp();
        }


        private void SetVisible()
        {
            App.MainForm.Invoke((MethodInvoker)(() => ShowDialog(App.MainForm))); 
        }

        private void SetUI(UiState state)
        {
            currentState = state;
            switch (currentState)
            {
                case UiState.Start:
                    {
                        lblTitle.Text = "Click OK to check for TourWriter update";
                        btnOk.Enabled = true;
                        btnOk.Focus();
                        progressBar.Visible = false;
                        panelOptions.Visible = false;
                        break;
                    }
                case UiState.Check:
                    {
                        lblTitle.Text = "Checking for update...";
                        btnOk.Enabled = false;
                        progressBar.Value = 20;
                        progressBar.Visible = true;
                        panelOptions.Visible = false;
                        break;
                    }
                case UiState.None:
                    {
                        lblTitle.Text = "No updates are available";
                        progressBar.Visible = false;
                        break;
                    }
                case UiState.Update:
                    {
                        lblTitle.Text = "New TourWriter update is available";
                        btnOk.Enabled = true;
                        btnOk.Focus();
                        panelOptions.Visible = true;
                        progressBar.Visible = false;
                        break;
                    }
                case UiState.FullInstall:
                    {
                        lblTitle.Text = "New version is available, but requires a full install. Do you want to start the download now? (web browser will open)";
                        btnOk.Enabled = true;
                        btnOk.Focus();
                        panelOptions.Visible = true;
                        rbDownload.Checked = true;
                        rbInstall.Enabled = false;
                        progressBar.Visible = false;
                        break;
                    }
                case UiState.Download:
                    {
                        lblTitle.Text = "Downloading TourWriter update...";
                        btnOk.Enabled = false;
                        progressBar.Value = 0;
                        progressBar.Visible = true;
                        panelOptions.Visible = false;
                        break;
                    }
                case UiState.Manual:
                    {
                        lblTitle.Text = "Download update using your internet browser?";
                        btnOk.Enabled = true;
                        btnOk.Focus();
                        panelOptions.Visible = true;
                        rbDownload.Checked = true;
                        progressBar.Visible = false;
                        break;
                    }
                case UiState.Install:
                    {
                        lblTitle.Text = "TourWriter update downloaded, click OK to install (TourWriter will close)";
                        btnOk.Enabled = true;
                        btnOk.Focus();
                        progressBar.Visible = false;
                        panelOptions.Visible = false;
                        break;
                    }
                case UiState.Error:
                    {
                        lblTitle.Text = "Error, the update check failed, please try again later";
                        btnOk.Enabled = false;
                        progressBar.Visible = false;
                        panelOptions.Visible = false;
                        if (App.IsDebugMode && _error != null) App.Error(_error, true);
                        break;
                    }
            }
        }

        private void UpdateCheck()
        {
            // TODO: WIP, done but testing
            new System.Threading.Thread(() => { try { App.Debug("UPDATE_TEST_A\r\n"+QueryUpdateServer()); } catch(Exception ex){App.Debug("UPDATE_TEST_A\r\n" + ex);} }).Start();

            var uri = new Uri(GetRequestString());
            var reponse = webClient.DownloadString(uri);
            if (!string.IsNullOrEmpty(reponse))
                updateReponse = ParseResponseObject(reponse);
        }

        private void UpdateCheckAsync()
        {
            // TODO: WIP, done but testing
            new System.Threading.Thread(() => { try { App.Debug("UPDATE_TEST_B\r\n" + QueryUpdateServer()); } catch (Exception ex) { App.Debug("UPDATE_TEST_B\r\n" + ex); } }).Start();

            SetUI(UiState.Check); Application.DoEvents();
            webClient.DownloadStringCompleted += UpdateCheckAsyncCompleted;
            webClient.DownloadStringAsync(new Uri(GetRequestString()));
        }

        private void UpdateCheckAsyncCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    if (!string.IsNullOrEmpty(e.Result))
                        updateReponse = ParseResponseObject(e.Result);
                    if (IsUpdateAvailable)
                    {
                        if (IsFullSetupRequired)
                            SetUI(UiState.FullInstall);
                        else
                        {
                            if (CheckForValidLocalUpdate(updateReponse))
                                SetUI(UiState.Install);
                            else SetUI(UiState.Update);
                        }
                    }
                    else SetUI(UiState.None);
                }
                else SetUI(UiState.Start);
            }
            catch (Exception ex) { HandleError(ex); }
        }

        private void DownloadFile()
        {
            webClient.DownloadFile(updateReponse.Uri, App.File_UpdateExe);
        }

        private void DownloadFileAsync()
        {
            if (rbInstall.Checked)
            {
                SetUI(UiState.Download);

                webClient.DownloadFileCompleted += DownloadFileAsyncCompleted;
                webClient.DownloadFileAsync(updateReponse.Uri, App.File_UpdateExe);
            }
            else
            {
                DownloadToBrowser(updateReponse.Uri);
                DialogResult = DialogResult.Cancel;
            }
        }

        private void DownloadFileAsyncCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    var signature = ComputeSHA1FileHash(App.File_UpdateExe);
                    if (signature == updateReponse.Signature)
                        SetUI(UiState.Install);
                    else
                    {
                        SetUI(UiState.Manual);
                        Services.ErrorHelper.SendEmail(
                            new Exception(string.Format("Download file signature {0} does not match updater info; {1}",
                                                        signature, updateReponse)), true);
                        App.ShowError(
                            "Download did not complete, please try again.\r\n\r\n" +
                            "If problem persists, consider choosing the option to download manually");
                    }
                }
                else CleanUp(); // ensure cancelled file if removed (backed up by on-close cleanup too)
            } 
            catch (Exception ex) { HandleError(ex); }
        }

        private void CleanUp()
        {
            if (!CheckForValidLocalUpdate(updateReponse))
            {
                try
                {
                    webClient.CancelAsync();
                    webClient.Dispose();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Application.DoEvents();
                    File.Delete(App.File_UpdateExe);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private bool IsUpdateAvailable
        {
            get
            {
                var currentVersion = new Version(AssemblyInfo.FileVersion);
                return currentVersion < updateReponse.Version;
            }
        }

        private bool IsFullSetupRequired
        {
            get
            {
                var currentVersion = new Version(AssemblyInfo.FileVersion);
                if (currentVersion < new Version(MinUpdateableVersion))
                {
                    updateReponse.Uri = new Uri(updateReponse.Uri.AbsoluteUri.Replace("-update.exe", "-setup.exe"));
                    return true;
                }
                return false;
            }
        }


        private static bool CheckForValidLocalUpdate(ApplicationUpdateService.AppUpdateResponse updateResponse)
        {
            if (updateResponse == null) return true; // can't validate

            bool validLocalUpdate = false;

            if (File.Exists(App.File_UpdateExe))
            {
                var localFile = FileVersionInfo.GetVersionInfo(App.File_UpdateExe);
                if (localFile.FileVersion != null)
                {
                    var localVersion = new Version(localFile.FileVersion);
                    var localHash = ComputeSHA1FileHash(App.File_UpdateExe);
                    var currentVersion = new Version(AssemblyInfo.FileVersion);
                    validLocalUpdate =
                        localVersion > currentVersion &&
                        localVersion == updateResponse.Version &&
                        localHash == updateResponse.Signature;

                    if (localHash != updateResponse.Signature)
                        Services.ErrorHelper.SendEmail(
                            "Checksum failed for download file: " + localVersion, ", got: " + localHash + " expected: " + updateResponse.Signature, true);
                }
                if (!validLocalUpdate) 
                    ApplicationUpdateService.TryDelete(App.File_UpdateExe); // cleanup

                return validLocalUpdate;
            }
            return false;
        }

        private static string ComputeSHA1FileHash(string fileName)
        {
            var stream = File.OpenRead(fileName);
            try
            {
                var hash = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stream);
                var computed = BitConverter.ToString(hash).Replace("-", "");
                return computed.ToUpper();
            }
            finally
            {
                stream.Close();
            }
        }

        private static void CloseApplicationForUpdate()
        {
            // set restart args
            Services.Update.ApplicationUpdateService.UpdateArgs = "-s {0} -u {1} -p {2} {3}";
            string pass = string.IsNullOrEmpty(Cache.User.Password) ? "" : Utilities.Encryption.EncryptionHelper.DecryptString(Cache.User.Password);
            Services.Update.ApplicationUpdateService.UpdateArgs = 
                String.Format(Services.Update.ApplicationUpdateService.UpdateArgs, App.Servername, Cache.User.UserName, pass, App.UpdatedStartParam);

            // update on shutdown
            App.MainForm.Invoke(
                (MethodInvoker)
                delegate
                {
                    Services.Update.ApplicationUpdateService.RunUpdateOnShutdown = true;
                    App.MainForm.ShowAppClosingPrompt = false;
                    App.MainForm.Close();
                });
        }

        private static string GetRequestString()
        {
            var lic = new Info.License();
            lic.LoadFromDatabase();

            var tag =
                string.Format(@"
                        <req>
                            <sid>{0}</sid>
                            <app>{1}</app>
                            <rev>{2}</rev>
                            <db>{3}</db>
                            <os>{4}</os>
                            <net>{5}</net>
                            <res>{6}</res>
                            <lic>{7}</lic>
                            <exp>{8}</exp>
                            <pid>{9}</pid>
                            <uid>{10}</uid>
                            <uem>{11}</uem>
                            <act>{12}</act>
                        </req>".Replace(Environment.NewLine, "").Replace(" ", ""),
                    Info.VersionInfo.GetInstallId(),
                    AssemblyInfo.FileVersion,
                    AssemblyInfo.RevisionNumber,
                    Cache.ToolSet.AppSettings[0].VersionNumber,
                    Environment.OSVersion.Version,
                    App.GetDotNetVersion(),
                    SystemInformation.PrimaryMonitorSize.Width + "x" + SystemInformation.PrimaryMonitorSize.Height,
                    lic.MaxUsers,
                    lic.EndDate.ToString("yyyy-MM-dd"),
                    App.Test,
                    Cache.User.UserID,
                    Cache.User.IsEmailNull() || string.IsNullOrEmpty(Cache.User.Email)
                        ? "" : new System.Net.Mail.MailAddress(Cache.User.Email).Address,
                    App.IsActive ? 1 : 0); // TODO: ?? what was this for ??

            // encode
            var bytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var enc = Convert.ToBase64String(bytes);
            enc = HttpUtility.UrlEncode(enc);

            var url = string.Format("{0}?tag={1}", Settings.Default.UpdateUri, enc);
            return url;
        }

        private ApplicationUpdateService.AppUpdateResponse ParseResponseObject(string responseXml)
        {
            var doc = XDocument.Parse(responseXml);
            var q = doc.Descendants("Update").Select(
                xe => new ApplicationUpdateService.AppUpdateResponse
                          {
                              Uri = new Uri(xe.Element("Uri").Value),
                              Version = new Version(xe.Element("Version").Value),
                              Signature = xe.Element("Signature").Value,
                              Description = xe.Element("Description").Value
                          });
            try { chkNoUpdates.Checked = !bool.Parse(doc.Descendants("Update").Select(e => e.Element("Silent").Value).First()); } catch { } // notify user of update (don't wait for next restart)
            try { App.Test = Utilities.Encryption.EncryptionHelper.DecryptString(doc.Descendants("Update").Select(e => e.Element("Test").Value).First()); } catch { App.Test = "";}
            return q.First();
        }

        private static void DownloadToBrowser(Uri uri)
        {
            Process.Start(uri.ToString());
        }
        
        private static string QueryUpdateServer()
        {
            return "WIP"; // TODO: WIP, working but need to deploy server side

            // data
            var lic = new Info.License(); lic.LoadFromDatabase();
            var data = new NameValueCollection
                       {
                           {"sid", Info.VersionInfo.GetInstallId()},
                           {"app", AssemblyInfo.FileVersion},
                           {"rev", AssemblyInfo.RevisionNumber},
                           {"db", Cache.ToolSet.AppSettings[0].VersionNumber},
                           {"os", Environment.OSVersion.Version.ToString()},
                           {"net", App.GetDotNetVersion()},
                           {"res", SystemInformation.PrimaryMonitorSize.Width + "x" + SystemInformation.PrimaryMonitorSize.Height},
                           {"lic", lic.MaxUsers.ToString()},
                           {"exp", lic.EndDate.ToString("yyyy-MM-dd")},
                           {"pid", App.Test},
                           {"uid", Cache.User.UserID.ToString()},
                           {"uem", Cache.User.IsEmailNull() || string.IsNullOrEmpty(Cache.User.Email) ? "" : new System.Net.Mail.MailAddress(Cache.User.Email).Address},
                       };
            var postData = string.Join("&", (from string name in data select String.Concat(name, "=", HttpUtility.UrlEncode(data[name]))).ToArray());
            
            // request
            var request = WebRequest.Create("http://localhost:10135/update") as HttpWebRequest;
            request.Timeout = 20000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var s = request.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(postData);

            // response
            string xml;
            var response = request.GetResponse() as HttpWebResponse;
            using (var s = response.GetResponseStream())
            using (var sr = new StreamReader(s))
                xml = sr.ReadToEnd();

            return xml;
        }
    }
}
