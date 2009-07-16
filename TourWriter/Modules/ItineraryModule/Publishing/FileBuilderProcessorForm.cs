using System;
using System.Windows.Forms;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    public partial class FileBuilderProcessorForm : Form
    {
        public FileBuilderProcessorForm()
        {
            InitializeComponent();
        }
        

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            IncrementProgress(1);    
            SetText(String.Format("Inserting item {0} of {1}", progressBar1.Value, progressBar1.Maximum));

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

        }
        

        delegate void SetTextDelegate(string text);
        private void SetText(string text)
        {
            if (InvokeRequired)
                Invoke(new SetTextDelegate(SetText), new object[] { text });
            else
                label1.Text = text;
        }

        delegate void IncrementProgressDelegate(int i);
        private void IncrementProgress(int i)
        {
            if (InvokeRequired)
                Invoke(new IncrementProgressDelegate(IncrementProgress), new object[] { i });
            else
                progressBar1.Increment(i);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
    }
}