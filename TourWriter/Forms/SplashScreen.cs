using System.Windows.Forms;

namespace TourWriter.Forms
{
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// Thread-safe setter for message text. 
        /// </summary>
        public string MessageText
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate { MessageText = value; }));
                }
                else
                {
                    lblMessage.Text = value;
                }
            }
        }

        /// <summary>
        /// Thread-safe setter for version text. 
        /// </summary>
        public string VersionText
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate { VersionText = value; }));
                }
                else
                {
                    lblVersion.Text = value;
                }
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }
    }
}
