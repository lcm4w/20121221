using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.Misc;

namespace TourWriter.UserControls
{
    /// <summary>
    /// Summary description for EmailWizard.
    /// </summary>
    public class Wizard : Form
    {
        #region Designer

        private UltraButton btnBack;
        private UltraButton btnNext;
        private UltraButton btnCancel;
        private Panel pnlPage;
        private GroupBox groupBox1;
        private UltraButton btnFinish;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
               

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBack = new Infragistics.Win.Misc.UltraButton();
            this.btnNext = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.pnlPage = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFinish = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(336, 436);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "< Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(412, 436);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(500, 436);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlPage
            // 
            this.pnlPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPage.Location = new System.Drawing.Point(4, 4);
            this.pnlPage.Name = "pnlPage";
            this.pnlPage.Size = new System.Drawing.Size(576, 408);
            this.pnlPage.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(8, 420);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 4);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // btnFinish
            // 
            this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinish.Location = new System.Drawing.Point(412, 436);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(75, 23);
            this.btnFinish.TabIndex = 5;
            this.btnFinish.Text = "Finish";
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // Wizard
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 470);
            this.ControlBox = false;
            this.Controls.Add(this.pnlPage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnFinish);
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Wizard_Closing);
            this.Load += new System.EventHandler(this.Wizard_Load);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        private int index;
        private object parms;

        private List<WizardPage> wizardPages;

        public List<WizardPage> WizardPages
        {
            get
            {
                if (wizardPages == null)
                    wizardPages = new List<WizardPage>();
                return wizardPages;
            }
        }

        public object Params
        {
            get { return parms; }
            set { parms = value; }
        }


        public Wizard()
        {
            InitializeComponent();
        }

        public Wizard(Size size)
        {
            InitializeComponent();

            Size = size;
        }
        

        internal void AddPage(WizardPage page)
        {
            WizardPages.Add(page);
        }

        internal void SetWizardButtons(WizardButtons buttons)
        {
            btnBack.Enabled = ((buttons & WizardButtons.Back) != 0);

            if ((buttons & WizardButtons.Finish) != 0)
            {
                btnFinish.Visible = true;
                btnFinish.Enabled = true;

                btnNext.Visible = false;
                btnNext.Enabled = false;

                AcceptButton = btnFinish;
            }
            else
            {
                btnFinish.Visible = false;
                btnFinish.Enabled = false;

                btnNext.Visible = true;
                btnNext.Enabled = ((buttons & WizardButtons.Next) != 0);

                AcceptButton = btnNext;
            }
        }

        internal void SetButtonsEnabled(bool isEnabled)
        {
            btnBack.Enabled = isEnabled;
            btnNext.Enabled = isEnabled;
            btnFinish.Enabled = isEnabled;
            btnCancel.Enabled = isEnabled;
        }

        internal void SetSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        internal void SetMinimumSize(int width, int height)
        {
            MinimumSize = new Size(width, height);
        }

        internal void Finish()
        {
            btnFinish.PerformClick();
        }

        internal void Next()
        {
            btnNext.PerformClick();
        }

        internal void Back()
        {
            btnBack.PerformClick();
        }

        protected virtual void OnQueryCancel(CancelEventArgs e)
        {
            WizardPages[index].OnQueryCancel(e);
        }


        private void Wizard_Load(object sender, EventArgs e)
        {
            if (WizardPages.Count > 0)
                LoadPage(0);

            Icon = Properties.Resources.TourWriter16;
        }

        private void LoadPage(int pageIndex)
        {
            WizardPage page = WizardPages[pageIndex];

            if (!pnlPage.Controls.Contains(page))
            {
                page.Wizard = this;
                page.Dock = DockStyle.Fill;
                pnlPage.Controls.Add(page);
            }

            CancelEventArgs e = new CancelEventArgs();
            e.Cancel = false;
            page.OnSetActive(e);

            if (!e.Cancel)
            {
                foreach (Control c in pnlPage.Controls)
                    c.Visible = (c == page);

                pnlPage.Select();
                index = pageIndex;
            }
            ProcessButtons();
        }

        private void ChangePage(int delta)
        {
            WizardPage oldActivePage = WizardPages[index];
            WizardPage newActivePage = WizardPages[index + delta];

            WizardPageEventArgs wizardArgs = new WizardPageEventArgs();
            wizardArgs.NewPage = newActivePage;
            wizardArgs.Cancel = false;

            if (delta > 0)
                oldActivePage.OnWizardNext(wizardArgs);
            else
                oldActivePage.OnWizardBack(wizardArgs);

            if (!wizardArgs.Cancel)
                LoadPage(index + delta);
        }

        private void ProcessButtons()
        {
            btnBack.Enabled = (index > 0);
            btnNext.Visible = (index < WizardPages.Count - 1);
            AcceptButton = btnNext.Visible ? btnNext : btnFinish;
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            ChangePage(+1);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ChangePage(-1);
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            WizardPage page = WizardPages[index];
            CancelEventArgs ev = new CancelEventArgs();
            ev.Cancel = false;
            page.OnWizardFinish(ev);

            if (!ev.Cancel)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Wizard_Closing(object sender, CancelEventArgs e)
        {
            if (!btnCancel.Enabled)
                e.Cancel = true;
            else if (btnFinish.Enabled)
                OnQueryCancel(e);
        }


        internal UltraButton ButtonBack
        {
            get { return btnBack; }
        }

        internal UltraButton ButtonNext
        {
            get { return btnNext; }
        }

        internal UltraButton ButtonFinish
        {
            get { return btnFinish; }
        }

        internal UltraButton ButtonCancel
        {
            get { return btnCancel; }
        }
    }

    [Flags]
    public enum WizardButtons
    {
        None = 0x0000,
        Back = 0x0001,
        Next = 0x0002,
        Finish = 0x0004,
    }
}