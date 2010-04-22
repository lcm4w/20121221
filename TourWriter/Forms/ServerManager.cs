using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TourWriter.Forms
{
    public partial class ServerManager : Form
    {
        private const string NoLocalServers = "No servers found";
        private readonly ListBox _lstServers;
        public string ServerName;

        public ServerManager()
        {
            _lstServers = new ListBox { Visible = false };
            Controls.Add(_lstServers);

            InitializeComponent();
            InitializeServerList();
        }

        private void InitializeServerList()
        {
            _lstServers.Width = txtLocalName.Width;
            _lstServers.Location = new Point(txtLocalName.Location.X, txtLocalName.Location.Y + 20);
            _lstServers.SelectedIndexChanged += delegate
            {
                if (_lstServers.SelectedItem == null) return;
                txtLocalName.Text = _lstServers.SelectedItem.ToString();
                _lstServers.Visible = false;
            };
        }

        private void ServerManager_Load(object sender, EventArgs e)
        {
            txtRemoteConnection.Text = Properties.Settings.Default.RemoteConnection;
            lblVersion.Text = "TourWriter version: " + AssemblyInfo.FileVersion;
        }
        
        private void PopulateServerList()
        {
            try
            {
                _lstServers.Items.Clear();
                _lstServers.Items.Add("Searching local network......");
                _lstServers.Enabled = false;
                Application.DoEvents();

                Cursor = Cursors.WaitCursor;

                var table = Info.Services.DatabaseHelper.GetAvailableSqlServers();

                _lstServers.Items.Clear();
                if (table.Rows.Count > 0)
                {
                    _lstServers.Enabled = true;
                    foreach (DataRow row in table.Rows)
                        _lstServers.Items.Add(row[0].ToString());
                }
                else
                    _lstServers.Items.Add(NoLocalServers);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void EnableDisableOkButton()
        {
            btnOK.Enabled = txtLocalName.Text.Length > 0 || txtRemoteConnection.Text.Length > 0;
        }


        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            ServerName = "";
            var isLocal = !string.IsNullOrEmpty(txtLocalName.Text);

            if (isLocal)
            {
                ServerName = txtLocalName.Text;
            }
            else if (!string.IsNullOrEmpty(txtRemoteConnection.Text))
            {
                ServerName = App.RemoteConnectionName;
                Properties.Settings.Default.RemoteConnection = txtRemoteConnection.Text;
                Properties.Settings.Default.Save();
            }
            DialogResult = DialogResult.OK;
        }

        private void btnListServers_Click(object sender, EventArgs e)
        {
            _lstServers.Visible = true;
            if (_lstServers.Items.Count == 0 || _lstServers.Items[0].ToString() == NoLocalServers)
                PopulateServerList();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            EnableDisableOkButton();
        }

        private void txtRemoteConnection_TextChanged(object sender, EventArgs e)
        {
            EnableDisableOkButton();
        }

        #endregion
    }
}