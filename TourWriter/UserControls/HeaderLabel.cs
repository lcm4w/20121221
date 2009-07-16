using System;
using System.Windows.Forms;

namespace TourWriter.UserControls
{
    public partial class HeaderLabel : UserControl
    {
        public HeaderLabel()
        {
            InitializeComponent();
        }
        
        public String TextString
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public Infragistics.Win.VAlign TextVAlign
        {
            get { return label.Appearance.TextVAlign; }
            set { label.Appearance.TextVAlign = value; }
        }

        public Infragistics.Win.HAlign TextHAlign
        {
            get { return label.Appearance.TextHAlign; }
            set { label.Appearance.TextHAlign = value; }
        }
    }
}
