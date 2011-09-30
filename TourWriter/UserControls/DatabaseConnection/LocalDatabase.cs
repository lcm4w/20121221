using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class LocalDatabase : BaseUserControl, IConnectionControl
    {
        public LocalDatabase()
        {
            InitializeComponent();
        }
        
        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "OK";
            NextButton.Enabled = true;
            NextControl = null;
            PrevControl = new ChooseDatabase();

            NextButton.Click += delegate { SetServer(); }; // set server on Next/OK click

            // show the current local connection
            var conn = ConnectionInfo.DbConnections.Where(x => x.Type == "local").FirstOrDefault();
            if (conn != null) cmbSearch.Text = conn.Name;
        }

        private void SetServer()
        {
            if (string.IsNullOrEmpty(cmbSearch.Text.Trim())) return;

            var name = cmbSearch.Text.Trim();

            // TODO: allow multi local db's?
            var conn = ConnectionInfo.DbConnections.Where(x => x.Type == "local").FirstOrDefault() ??
                       ConnectionInfo.DbConnections.Add("local", name, name);
            conn.Name = name;
            conn.Data = name;
            ConnectionInfo.SelectedConnection = conn.Name;
        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            cmbSearch.DroppedDown = true;
        }
        
        private void OnSearchDropDown(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                cmbSearch.Items.Clear();
                var table = Info.Services.DatabaseHelper.GetAvailableSqlServers();
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows) cmbSearch.Items.Add(row[0].ToString());
                    var server = cmbSearch.Items.Cast<object>().Where(i => i.ToString().ToUpper().EndsWith(@"\TOURWRITER")).FirstOrDefault();
                    if (server != null) cmbSearch.SelectedItem = server;
                }
            }
            finally { Cursor = Cursors.Default; }
        }

        private void lnkInstall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NextControl = new InstallOptions();
            GoNext();
        }

        #region IConnectionControl members

        //public string GetServerName()
        //{
        //    return txtName.Text;
        //}

        //public string GetUserName()
        //{
        //    return null;
        //}

        //public string GetPassword()
        //{
        //    return null;
        //}

        //public string GetRemoteName()
        //{
        //    return null;
        //}

        //public string GetRemoteConnection()
        //{
        //    return null;
        //}
        
        //public StringDictionary GetRemoteServers()
        //{
        //    return null;
        //}
        
        public bool ValidateAndFinalise()
        {
            if (cmbSearch.Text.Trim().Length == 0)
            {
                App.ShowInfo("No server connection entered");
                return false;
            }
            return true;
        }

        #endregion
    }
}
