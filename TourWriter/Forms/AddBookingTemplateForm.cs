using System;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.Forms
{
    public partial class AddBookingTemplateForm : Form
    {
        public string TemplateName
        {
            get { return txtName.Text; }
        }

        public string TemplateFile
        {
            get { return txtFile.Text; }
        }

        public AddBookingTemplateForm()
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;
        }

        private bool ValidateForm()
        {
            var msg = "";
            if (txtName.Text.Trim() == "")
                msg += "Template name cannot be empty";
            if (txtFile.Text.Trim() == "")
                msg += (msg.Length > 0 ? "\r\n" : "") +
                    "Template path cannot be empty";

            if (msg != "")
            {
                App.ShowError(msg);
                return false;
            }
            return true;
        }

        #region Events

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = ValidateForm() ? DialogResult.OK : DialogResult.None;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string file = App.SelectExternalFile(true, "Select email template", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*", 0);
            if (!String.IsNullOrEmpty(file))
            {
                txtFile.Text = file;
            }
        }

        #endregion
    }
}
