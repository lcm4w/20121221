using System.Windows.Forms;
using TourWriter.Info;

namespace TourWriter.Modules.SupplierModule.Content
{
    public partial class ContentEditor : UserControl
    {
        private SupplierSet.ContentRow _content;
        public SupplierSet.ContentRow Content
        {
            get { return _content; }
            set
            {
                _content = value;
                if (_content == null) return;


                txtName.DataBindings.Clear();
                txtName.DataBindings.Add("Text", _content, "ContentName");

                txtHeading.DataBindings.Clear();
                txtHeading.DataBindings.Add("Text", _content, "Heading");

                txtBody.DataBindings.Clear();
                txtBody.DataBindings.Add("Text", _content, "Body");

                txtImage.DataBindings.Clear();
                txtImage.DataBindings.Add("Text", _content, "ImagePath");
            }
        }

        public ContentEditor()
        {
            InitializeComponent();
        }

        public void RefreshName()
        {
            txtName.Text = _content.ContentName;
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            const string fileFilter = "Word objects (*.jpg;*.jpeg;*.gif;*.png;*.bmp)|*.jpg;*.jpeg;*.gif;*.png;*.bmp|All files (*.*)|*.*";
            var file = App.SelectExternalFile(true, "Choose image", fileFilter, 1);
            txtImage.Text = file;
            _content.ImagePath = file;
            _content.EndEdit();
        }
    }
}
