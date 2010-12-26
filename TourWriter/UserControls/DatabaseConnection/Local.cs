using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class Local : UiControlBase, IConnectionControl
    {
        private readonly ListBox _lstServers;

        public Local()
        {
            InitializeComponent();

            _lstServers = new ListBox
                              {
                                  Visible = false,
                                  Location = new Point(txtName.Location.X, txtName.Location.Y + 20)
                              };
            Controls.Add(_lstServers);
            _lstServers.SelectedIndexChanged += OnIndexChanged;
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
        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            _lstServers.Visible = true;
            _lstServers.Width = txtName.Width;
            _lstServers.Items.Clear();
            _lstServers.Items.Add("Searching local network......");
            _lstServers.Enabled = false;
            Application.DoEvents();
            Cursor = Cursors.WaitCursor;

            try
            {
                var table = Info.Services.DatabaseHelper.GetAvailableSqlServers();
                _lstServers.Items.Clear();
                if (table.Rows.Count > 0)
                {
                    _lstServers.Enabled = true;
                    foreach (DataRow row in table.Rows) _lstServers.Items.Add(row[0].ToString());
                    var server = _lstServers.Items.Cast<object>().Where(i => i.ToString().ToUpper().EndsWith(@"\TOURWRITER")).FirstOrDefault();
                    if (server != null) _lstServers.SelectedItem = server;
                }
                else _lstServers.Items.Add("No servers found");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        void OnIndexChanged(object sender, EventArgs e)
        {
            if (_lstServers.SelectedItem == null) return;
            txtName.Text = _lstServers.SelectedItem.ToString();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            NextButton.Enabled = txtName.Text.Length > 0;
        }
        
        #region IConnectionControl members

        public string GetServerName()
        {
            return txtName.Text;
        }

        public string GetUserName()
        {
            return null;
        }

        public string GetPassword()
        {
            return null;
        }

        public string GetRemoteName()
        {
            return null;
        }

        public string GetRemoteConnection()
        {
            return null;
        }
        
        public bool ValidateAndFinalise()
        {
            return true;
        }

        #endregion
    }
}
