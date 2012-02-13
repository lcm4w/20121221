using System.Windows.Forms;
using TourWriter.Properties;
using TourWriter.Utilities.Encryption;

namespace Travelmesh
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            
            var x = Settings.Default.TmSyncUser;
            if (x.Length > 0) x = EncryptionHelper.DecryptString(x);
            var s = x.Split(':');
            if (s.Length > 1) txtUser.Text = s[0];

            txtUser.Select();
            txtUser.SelectAll();
        }

        public string User { get {return txtUser.Text;}}
        public string Pass { get {return txtPass.Text;}}

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
