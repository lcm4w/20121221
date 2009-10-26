using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinTree;
using TourWriter.Forms;
using TourWriter.Info;

namespace TourWriter.Modules
{
    /// <summary>
    /// Summary description for ModuleBase.
    /// </summary>
    public class ModuleBase : Form
    {
        protected bool _doLicenseCheck = true;

        private const string MethodAccessException =
            "Base class does not implement this method or operation.";

        protected string displayTypeName;
        private UltraLabel ultraLabel1;
        private UltraLabel ultraLabel2;
        private readonly Container components = null;
        private MainForm MainForm
        {
            get { return MdiParent as MainForm; }
        }
        protected ProgressBarInfo ProgressBar
        {
            get { return MainForm.StatusBar_ProgressBar; }
        }


        public ModuleBase()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Determines whether the controls header area is visible or hidden.")]
        public bool HeaderVisible
        {
            get { return ultraLabel1.Visible && ultraLabel2.Visible; }
            set
            {
                ultraLabel1.Visible = value;
                ultraLabel2.Visible = value;
            }
        }

        [Browsable(true)]
        [Description("Sets the ToolStrip that will be merged with the parent form ToolStrip.")]
        private ToolStrip mainToolStrip;
        public ToolStrip MainToolStrip
        {
            get { return mainToolStrip; }
            set { mainToolStrip = value; }
        }

        private void ModuleBase_Load(object sender, EventArgs e)
        {
            if (_doLicenseCheck && !DesignMode)
                Services.LicenseService.CheckLicense();
        }

        private void ModuleBase_Shown(object sender, EventArgs e)
        {
            UpdateMainForm(true);
            
            Activated += ModuleBase_Activated;
            Deactivate += ModuleBase_Deactivate;
        }

        private void ModuleBase_Activated(object sender, EventArgs e)
        {
            App.LastActive = DateTime.Now;
            UpdateMainForm(true);
        }

        private void ModuleBase_Deactivate(object sender, EventArgs e)
        {
            UpdateMainForm(false);
        }
        
        protected virtual void ModuleBase_Closing(object sender, CancelEventArgs e)
        {
            if (IsDataDirty())
            {
                DialogResult result = MessageBox.Show(
                    App.GetResourceString("AskDoSave"), Text,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        {
                            SaveDataChanges();
                            break;
                        }
                    case DialogResult.No:
                        {
                            CancelDataChanges();
                            break;
                        }
                    case DialogResult.Cancel:
                        {
                            e.Cancel = true;
                            return;
                        }
                }
            }
            App.ClearBindings(this);
            Dispose();
        }
        
        
        private void UpdateMainForm(bool visible)
        {
            if (visible)
            {
                // Highlight menu item.
                if (IsLinkedToMenu)
                    MenuNode = App.MainForm.ActivateMenuNode(Tag as UltraTreeNode);

                // Set main text.
                SetFormActiveText();

                // Merge tool strip
                if (!DesignMode && MainToolStrip != null)
                    MainForm.MergeToolStrip(MainToolStrip);
            }
            else
            {
                SetFormInactiveText();

                // Revert tool strip
                if (!DesignMode && MainToolStrip != null)
                    MainForm.RevertToolStrip(MainToolStrip);
            }
        }


        protected virtual UltraTreeNode MenuNode
        {
            get { return Tag as UltraTreeNode; }
            set
            {
                if (value != null)
                {
                    if (value.GetType() != typeof (UltraTreeNode))
                        throw new ArgumentException(
                            "Form Tag must be a Infragistics.Win.UltraWinTree.UltraTreeNode");

                    Icon = Icon.FromHandle(((Bitmap) value.LeftImages[0]).GetHicon());
                }
                Tag = value;
            }
        }

        protected virtual bool IsDataDirty()
        {
            throw new MethodAccessException(MethodAccessException);
        }

        protected virtual void CommitOpenEdits()
        {
            throw new MethodAccessException(MethodAccessException);
        }

        protected virtual void SaveDataChanges()
        {
            throw new MethodAccessException(MethodAccessException);
        }

        protected virtual void CancelDataChanges()
        {
            throw new MethodAccessException(MethodAccessException);
        }

        protected virtual string GetDisplayName()
        {
            throw new MethodAccessException(MethodAccessException);
        }


        protected bool IsLinkedToMenu
        {
            get
            {
                return (MenuNode != null) && (MenuNode.Tag != null) &&
                       (MenuNode.Tag.GetType() == typeof (NavigationTreeItemInfo));
            }
        }

        protected void SetFormActiveText()
        {
            if (App.MainForm != null)
            {
                Text = Text.StartsWith("*") ? "*" + GetDisplayName() : GetDisplayName();

                if (MenuNode != null)
                    ultraLabel1.Text = MenuNode.FullPath;
            }
        }

        protected void SetFormInactiveText()
        {
            if (Text.Length > 10)
                Text = String.Format("{0}...", Text.Substring(0, 9));
        }

        delegate void SetDataDirtyNameDelegate();
        protected void SetDataDirtyName()
        {
            if (!InvokeRequired)
            {
                if (!Text.StartsWith("*"))
                    Text = "*" + Text.Trim();
            }
            else
                Invoke(new SetDataDirtyNameDelegate(SetDataDirtyName));
        }

        protected void SetDataCleanName()
        {
            Text = Text.TrimStart('*');
        }

        protected void UpdateMainForm(UltraTree menu, bool isRecordActive)
        {
            // update the main label text
            SetFormActiveText();

            // update the item on the menu
            if (IsLinkedToMenu)
            {
                // menu item text
                MenuNode.Text = GetDisplayName();
                MenuNode.Override.NodeAppearance.FontData.Strikeout = isRecordActive
                                                                          ?
                                                                      DefaultableBoolean.False
                                                                          : DefaultableBoolean.True;

                // menu item tag
                ((NavigationTreeItemInfo) MenuNode.Tag).ItemName = GetDisplayName();
                ((NavigationTreeItemInfo) MenuNode.Tag).IsActive = isRecordActive;

                // highlight menu item
                MenuHelper.RefreshSort(menu);
                MenuNode = App.MainForm.ActivateMenuNode(Tag as UltraTreeNode);
            }
        }

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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            appearance1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            appearance1.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance1.BorderColor = System.Drawing.SystemColors.ActiveCaption;
            appearance1.ForeColor = System.Drawing.SystemColors.Info;
            appearance1.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            appearance1.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.BorderStyleOuter = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(556, 30);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Module name";
            this.ultraLabel1.WrapText = false;
            // 
            // ultraLabel2
            // 
            appearance2.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            appearance2.BackColor2 = System.Drawing.SystemColors.InactiveCaption;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ultraLabel2.Appearance = appearance2;
            this.ultraLabel2.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraLabel2.Location = new System.Drawing.Point(0, 30);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(556, 10);
            this.ultraLabel2.TabIndex = 1;
            // 
            // ModuleBase
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(556, 362);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.DoubleBuffered = true;
            this.Name = "ModuleBase";
            this.Load += new System.EventHandler(this.ModuleBase_Load);
            this.Shown += new System.EventHandler(this.ModuleBase_Shown);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ModuleBase_Closing);
            this.ResumeLayout(false);

        }

        #endregion

    }
}