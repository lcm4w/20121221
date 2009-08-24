using System;
using System.Data;
using System.Windows.Forms;

namespace TourWriter.Forms
{
    public partial class ServerManager : Form
    {
        public string Server
        {
            get { return txtServer.Text; }
            set { txtServer.Text = value; }
        }

        public ServerManager()
        {
            InitializeComponent();
        }

        private void LoadNetworkList()
        {
            try
            {
                lstServers.Items.Clear();
                lstServers.Items.Add("Retrieving data...");
                lstServers.Enabled = false;
                Application.DoEvents();

                Cursor = Cursors.WaitCursor;

                DataTable table = Info.Services.DatabaseHelper.GetAvailableSqlServers();

                lstServers.Items.Clear();
                if (table.Rows.Count > 0)
                {
                    lstServers.Enabled = true;
                    foreach (DataRow row in table.Rows)
                        lstServers.Items.Add(row[0].ToString());
                }
                else
                    lstServers.Items.Add("No servers found");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #region Events

        private void ServerManager_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "TourWriter version: " + AssemblyInfo.FileVersion;
            txtServer.Select();
        }

        private void btnSearch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadNetworkList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (txtServer.Text != "");
        }

        private void lstServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstServers.SelectedItem != null)
                txtServer.Text = lstServers.SelectedItem.ToString();
        }

        private void lstServers_DoubleClick(object sender, EventArgs e)
        {
            if (lstServers.SelectedItem != null)
                btnOK.PerformClick();
        }

        #endregion
    }
}