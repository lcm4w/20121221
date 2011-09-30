using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class TrialProgress : Form
    {
        private int _timeout = 20000;
        private BackgroundWorker _worker;
        private readonly Point _formLocation;
        private System.Threading.Timer _timer;

        public string CompanyName { private get; set; }
        public string UserName { private get; set; }
        public string UserEmail { private get; set; }
        public string Key { get; private set; }
        public object Error { get; private set; }
        public string Log { get { return richTextBox1.Text; } }


        public TrialProgress(Point location)
        {
            InitializeComponent();
            _formLocation = location;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Location = _formLocation;

            _timer = new System.Threading.Timer(TimeoutFired, null, _timeout, Timeout.Infinite);

            _worker = new BackgroundWorker();
            _worker.DoWork += WorkerDoWork;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.WorkerSupportsCancellation = true;
            _worker.RunWorkerAsync();
        }
        
        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Key = "";

                WriteLog("Initializing online database...");
                Key = Travelmesh.TrialDatabase.Create(CompanyName, UserName, UserEmail);

                WriteLog("Verifying online database...");
                Travelmesh.TrialDatabase.TestConnection(Key, CompanyName);

                WriteLog("Complete.");
                Thread.Sleep(500);
            }
            catch(Exception ex)
            {
                e.Result = ex;
            }
        }

        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Dispose();
            Thread.Sleep(100);

            if (e.Result is Exception)
            {
                Error = e.Result;
                Close();
            }
            if (Key != null && Key.Trim().Length > 0)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void WriteLog(string text)
        {
            Invoke(new MethodInvoker(delegate { richTextBox1.Text += text + "\r\n"; }));
        }

        private void TimeoutFired(object state)
        {
            Error = new TimeoutException("The server request timed out");
            _worker.CancelAsync();
            _worker.Dispose();
            _timer.Dispose();
            Invoke(new MethodInvoker(Close));
        }
    }
}
