using System;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.Forms
{
    public partial class LicenseExpiredForm : Form
    {
        private readonly int _secDelay;

        public LicenseExpiredForm(int secDelay, string message)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            _secDelay = secDelay;
            lblExpiredMessage.Text = message;
        }

        private void LicenseExpiredForm_Load(object sender, EventArgs e)
        {
            var thread = new Thread(CountdownThread) { Name = "LicenceExpired_CountdownThread" };
            thread.Start();
        }

        private void CountdownThread()
        {
            // delay
            for (var i = _secDelay; i > 0; i--)
            {
                var count = i;
                Invoke(new MethodInvoker(delegate { btnOk.Text = count.ToString(); }));
                Thread.Sleep(1000);
            }
            // enable
            Invoke(new MethodInvoker(delegate { btnOk.Text = "OK"; btnOk.Enabled = true; }));
        }
    }
}
