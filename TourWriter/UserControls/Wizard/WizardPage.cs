using System.ComponentModel;
using System.Windows.Forms;

namespace TourWriter.UserControls
{
    [DefaultEvent("SetActive")]
    public class WizardPage : UserControl
    {
        #region Designer

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Label lblText;

        private GroupBox groupBox1;
        private Panel pnlHeader;
        private Container components = null;

        public WizardPage()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblText = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.Font =
                new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold,
                                        System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
            this.lblText.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblText.Location = new System.Drawing.Point(4, 4);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(540, 20);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Wizard page text";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(4, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 8);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblText);
            this.pnlHeader.Controls.Add(this.groupBox1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(548, 28);
            this.pnlHeader.TabIndex = 2;
            // 
            // WizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.pnlHeader);
            this.Name = "WizardPage";
            this.Size = new System.Drawing.Size(548, 156);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        #endregion

        private Wizard wizard;

        public Wizard Wizard
        {
            get { return wizard; }
            set { wizard = value; }
        }


        [Category("Wizard")]
        public string PageText
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }


        public void SetWizardButtons(WizardButtons buttons)
        {
            Wizard.SetWizardButtons(buttons);
        }


        [Category("Wizard")]
        public event CancelEventHandler SetActive;

        public virtual void OnSetActive(CancelEventArgs e)
        {
            if (SetActive != null)
                SetActive(this, e);
        }


        [Category("Wizard")]
        public event WizardPageEventHandler WizardNext;

        public virtual void OnWizardNext(WizardPageEventArgs e)
        {
            if (WizardNext != null)
                WizardNext(this, e);
        }


        [Category("Wizard")]
        public event WizardPageEventHandler WizardBack;

        public virtual void OnWizardBack(WizardPageEventArgs e)
        {
            if (WizardBack != null)
                WizardBack(this, e);
        }


        [Category("Wizard")]
        public event CancelEventHandler WizardFinish;

        public virtual void OnWizardFinish(CancelEventArgs e)
        {
            if (WizardFinish != null)
                WizardFinish(this, e);
        }


        [Category("Wizard")]
        public event CancelEventHandler QueryCancel;

        public virtual void OnQueryCancel(CancelEventArgs e)
        {
            if (QueryCancel != null)
                QueryCancel(this, e);
        }
    }
}