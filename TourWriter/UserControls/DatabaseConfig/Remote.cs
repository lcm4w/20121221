using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class Remote : UiControlBase, IConnectionControl
    {
        private StringDictionary _servers;
 
        public Remote()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "OK";
            NextButton.Enabled = false;
            NextControl = null;
            PrevControl = new Start();
            
            lnkAdd.Visible = lnkClear.Visible = lnkNext.Visible = lnkPrev.Visible = App.IsDebugMode;

            // load existing connection info
            //_servers = Properties.Settings.Default.Databases;
            SetCurrentServer(0);

            NextButton.Click += delegate { SaveCurrentServer(); }; // set server on Next/OK click
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            NextButton.Enabled = txtInfo.Text.Length > 0;
        }
        
        public bool ValidateAndFinalise()
        {
            return true;
        }
        
        private int _currentIndex;
        private void SetCurrentServer(int index)
        {
            var conns = RemoteConnections;
            if (conns.Count == 0) return;

            if (index >= conns.Count) index = conns.Count - 1;
            if (index < 0) index = 0;
            _currentIndex = index;

            var conn = conns[_currentIndex];
            txtName.Text = conn.Name;
            txtInfo.Text = conn.Data;
        }
        
        private void SaveCurrentServer()
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()) || string.IsNullOrEmpty(txtInfo.Text.Trim())) return;

            var name = txtName.Text.Trim();
            var data = txtInfo.Text.Trim();

            var conns = RemoteConnections;
            var conn = _currentIndex > -1 && _currentIndex < conns.Count ? conns[_currentIndex] : null;
            if (conn == null)
            {
                conn = new DbConnection();
                Connections.Add(conn);
            }
            conn.Type = "remote";
            conn.Name = name;
            conn.Data = data;
        }

        private List<DbConnection> RemoteConnections
        {
            get { return Connections.Where(x => x.Type == "remote").ToList(); }
        }
        
        private void lnkAdd_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var name = "(new online server)";
            if (txtName.Text == name && txtInfo.Text.Trim() == "") return;

            SaveCurrentServer();
            Connections.Add("remote", name, "");
            SetCurrentServer(RemoteConnections.Count());
        }

        private void lnkClear_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var conn = RemoteConnections[_currentIndex];
            if (conn != null)
            {
                Connections.Remove(conn);
                txtName.Text = "";
                txtInfo.Text = "";
                _currentIndex--;
            }
        }

        private void lnkNext_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            SetCurrentServer(_currentIndex-1);
        }

        private void lnkPrev_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            SetCurrentServer(_currentIndex + 1);
        }

    }
}
