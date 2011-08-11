using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class RemoteSettings : UiControlBase, IConnectionControl
    {
        private List<DbConnection> RemoteConnections
        {
            get { return Connections.Where(x => x.Type == "remote").ToList(); }
        }
     
        
        public RemoteSettings()
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
            PrevControl = new StartPage();
            
            lnkAdd.Visible = lnkClear.Visible = lnkNext.Visible = lnkPrev.Visible = lblName.Visible = txtName.Visible = App.IsDebugMode;

            if (RemoteConnections.Count == 0)
                Connections.Add("remote", "", "");
            SetCurrentServer(0);

            NextButton.Click += delegate { SaveCurrentServer(); };
            BackButton.Click += delegate { SaveCurrentServer(); };
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            //NextButton.Enabled = txtInfo.Text.Length > 0;
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

            SetNavigationLinks();
        }
        
        private void SaveCurrentServer()
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()) && string.IsNullOrEmpty(txtInfo.Text.Trim()))
            {
                RemoveCurrentServer();
                return;
            }

            var name = txtName.Text.Trim();
            var data = txtInfo.Text.Trim();

            if (!App.IsDebugMode && data.Trim() == "")
            {
                RemoveCurrentServer();
                return;
            }
            if (!App.IsDebugMode && name.Trim() == "") 
                name = "(online server)";

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

        private void RemoveCurrentServer()
        {
            if (_currentIndex < 0) _currentIndex = 0;
            if (RemoteConnections.Count == 0) return;

            var conn = RemoteConnections[_currentIndex];
            if (conn != null)
            {
                Connections.Remove(conn);
                txtName.Text = "";
                txtInfo.Text = "";

                if (_currentIndex < RemoteConnections.Count)
                    SetCurrentServer(_currentIndex);
                else SetCurrentServer(_currentIndex - 1);
            }
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
            RemoveCurrentServer();
        }

        private void lnkNext_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            SaveCurrentServer();
            SetCurrentServer(_currentIndex+1);
        }

        private void lnkPrev_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            SaveCurrentServer();
            SetCurrentServer(_currentIndex-1);
        }

        private void SetNavigationLinks()
        {
            lnkPrev.Enabled = _currentIndex > 0 && RemoteConnections.Count > 0;
            lnkNext.Enabled = _currentIndex < RemoteConnections.Count-1 && RemoteConnections.Count > 0;
        }
    }
}
