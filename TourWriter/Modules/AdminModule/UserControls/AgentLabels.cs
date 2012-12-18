using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;
using TourWriter.BusinessLogic;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.ContactModule;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using CellClickAction=Infragistics.Win.UltraWinGrid.CellClickAction;
using System.Collections;

namespace TourWriter.Modules.AdminModule.UserControls
{
	public class AgentLabels : UserControl
	{
		#region Designer

		private System.Windows.Forms.Label lblHeading;
		private System.Windows.Forms.Label label6;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridAgents;
        internal System.Windows.Forms.ImageList imageList1;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private ToolStripButton btnAgentAdd;
        private ToolStripButton btnAgentDel;
        private ToolStripButton btnAgentCopy;
        private Label label10;
        private System.Windows.Forms.ToolTip toolTip1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private ComboBox cmbOverride;
        private UltraGrid gridMarginOverride;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl9;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl4;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl10;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox picVoucherLogo;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtVoucherLogo;
        private Label label11;
        private Infragistics.Win.Misc.UltraButton btnVoucherLogo;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl11;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtVoucherFooter;
        private Label label12;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl3;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage3;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl8;
        private Label label9;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTermsConditions;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private UltraGrid gridContact;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnContactAdd;
        private ToolStripButton btnContactDel;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtComments;
        private Label label3;
        private Label label4;
        private TourWriter.UserControls.MyToolStrip miniToolStrip;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl Currenc;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtCurrencyMargin;
        private Label label16;
        private Label label15;
        private Label label2;
        private Label label5;
        private Label label7;
        private Label label8;
        private Label label13;
        private ComboBox cmbMargin;
        private Label label14;
        private Label label17;
		private System.ComponentModel.IContainer components;

		public AgentLabels()
		{
			InitializeComponent();
		}
		
		protected override void Dispose(bool disposing)
		{
			EndAllEdits();

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentLabels));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTermsConditions = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraTabPageControl10 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.picVoucherLogo = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.txtVoucherLogo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label11 = new System.Windows.Forms.Label();
            this.btnVoucherLogo = new Infragistics.Win.Misc.UltraButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ultraTabPageControl11 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.txtVoucherFooter = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label12 = new System.Windows.Forms.Label();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gridContact = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.txtComments = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage3 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl9 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbMargin = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCurrencyMargin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbOverride = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.gridMarginOverride = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridAgents = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.Currenc = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.btnAgentCopy = new System.Windows.Forms.ToolStripButton();
            this.btnAgentAdd = new System.Windows.Forms.ToolStripButton();
            this.btnAgentDel = new System.Windows.Forms.ToolStripButton();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnContactAdd = new System.Windows.Forms.ToolStripButton();
            this.btnContactDel = new System.Windows.Forms.ToolStripButton();
            this.miniToolStrip = new TourWriter.UserControls.MyToolStrip();
            this.ultraTabPageControl8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTermsConditions)).BeginInit();
            this.ultraTabPageControl10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVoucherLogo)).BeginInit();
            this.ultraTabPageControl11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVoucherFooter)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridContact)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComments)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl3)).BeginInit();
            this.ultraTabControl3.SuspendLayout();
            this.ultraTabPageControl9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl4)).BeginInit();
            this.ultraTabControl4.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrencyMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMarginOverride)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Currenc)).BeginInit();
            this.Currenc.SuspendLayout();
            this.myToolStrip1.SuspendLayout();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Controls.Add(this.label9);
            this.ultraTabPageControl8.Controls.Add(this.txtTermsConditions);
            this.ultraTabPageControl8.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(784, 385);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(8, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(476, 23);
            this.label9.TabIndex = 43;
            this.label9.Text = "Note to be included on Client summary itinerary";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTermsConditions
            // 
            this.txtTermsConditions.AcceptsReturn = true;
            this.txtTermsConditions.AcceptsTab = true;
            this.txtTermsConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTermsConditions.Location = new System.Drawing.Point(8, 28);
            this.txtTermsConditions.Multiline = true;
            this.txtTermsConditions.Name = "txtTermsConditions";
            this.txtTermsConditions.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTermsConditions.Size = new System.Drawing.Size(773, 354);
            this.txtTermsConditions.TabIndex = 42;
            // 
            // ultraTabPageControl10
            // 
            this.ultraTabPageControl10.Controls.Add(this.picVoucherLogo);
            this.ultraTabPageControl10.Controls.Add(this.txtVoucherLogo);
            this.ultraTabPageControl10.Controls.Add(this.label11);
            this.ultraTabPageControl10.Controls.Add(this.btnVoucherLogo);
            this.ultraTabPageControl10.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl10.Name = "ultraTabPageControl10";
            this.ultraTabPageControl10.Size = new System.Drawing.Size(786, 386);
            // 
            // picVoucherLogo
            // 
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Right;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.picVoucherLogo.Appearance = appearance1;
            this.picVoucherLogo.BorderShadowColor = System.Drawing.Color.Empty;
            this.picVoucherLogo.Location = new System.Drawing.Point(8, 32);
            this.picVoucherLogo.Name = "picVoucherLogo";
            this.picVoucherLogo.Size = new System.Drawing.Size(480, 128);
            this.picVoucherLogo.TabIndex = 42;
            // 
            // txtVoucherLogo
            // 
            this.txtVoucherLogo.Location = new System.Drawing.Point(36, 4);
            this.txtVoucherLogo.Name = "txtVoucherLogo";
            this.txtVoucherLogo.Size = new System.Drawing.Size(428, 21);
            this.txtVoucherLogo.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 23);
            this.label11.TabIndex = 40;
            this.label11.Text = "Logo";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnVoucherLogo
            // 
            appearance2.Image = 16;
            this.btnVoucherLogo.Appearance = appearance2;
            this.btnVoucherLogo.ImageList = this.imageList1;
            this.btnVoucherLogo.Location = new System.Drawing.Point(464, 4);
            this.btnVoucherLogo.Name = "btnVoucherLogo";
            this.btnVoucherLogo.Size = new System.Drawing.Size(25, 23);
            this.btnVoucherLogo.TabIndex = 39;
            this.btnVoucherLogo.Click += new System.EventHandler(this.btnVoucherLogo_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            this.imageList1.Images.SetKeyName(27, "");
            this.imageList1.Images.SetKeyName(28, "");
            this.imageList1.Images.SetKeyName(29, "");
            this.imageList1.Images.SetKeyName(30, "");
            this.imageList1.Images.SetKeyName(31, "");
            this.imageList1.Images.SetKeyName(32, "");
            this.imageList1.Images.SetKeyName(33, "");
            this.imageList1.Images.SetKeyName(34, "");
            this.imageList1.Images.SetKeyName(35, "");
            this.imageList1.Images.SetKeyName(36, "");
            this.imageList1.Images.SetKeyName(37, "");
            this.imageList1.Images.SetKeyName(38, "");
            this.imageList1.Images.SetKeyName(39, "");
            this.imageList1.Images.SetKeyName(40, "");
            this.imageList1.Images.SetKeyName(41, "");
            // 
            // ultraTabPageControl11
            // 
            this.ultraTabPageControl11.Controls.Add(this.txtVoucherFooter);
            this.ultraTabPageControl11.Controls.Add(this.label12);
            this.ultraTabPageControl11.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl11.Name = "ultraTabPageControl11";
            this.ultraTabPageControl11.Size = new System.Drawing.Size(786, 386);
            // 
            // txtVoucherFooter
            // 
            this.txtVoucherFooter.AcceptsReturn = true;
            this.txtVoucherFooter.AcceptsTab = true;
            this.txtVoucherFooter.Location = new System.Drawing.Point(4, 24);
            this.txtVoucherFooter.Multiline = true;
            this.txtVoucherFooter.Name = "txtVoucherFooter";
            this.txtVoucherFooter.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVoucherFooter.Size = new System.Drawing.Size(484, 136);
            this.txtVoucherFooter.TabIndex = 10;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(4, 4);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(484, 23);
            this.label12.TabIndex = 41;
            this.label12.Text = "Notes to be included on vouchers";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.gridContact);
            this.ultraTabPageControl2.Controls.Add(this.myToolStrip2);
            this.ultraTabPageControl2.Controls.Add(this.txtComments);
            this.ultraTabPageControl2.Controls.Add(this.label3);
            this.ultraTabPageControl2.Controls.Add(this.label4);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(795, 393);
            // 
            // gridContact
            // 
            this.gridContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridContact.Location = new System.Drawing.Point(8, 113);
            this.gridContact.Name = "gridContact";
            this.gridContact.Size = new System.Drawing.Size(510, 302);
            this.gridContact.TabIndex = 9;
            this.gridContact.Text = "Contact list  (drag-drop)";
            this.gridContact.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridContact_InitializeLayout);
            this.gridContact.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.gridContact_InitializeRow);
            this.gridContact.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridContact_CellChange);
            this.gridContact.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridContact_ClickCellButton);
            // 
            // txtComments
            // 
            this.txtComments.AcceptsReturn = true;
            this.txtComments.AcceptsTab = true;
            this.txtComments.Location = new System.Drawing.Point(60, 8);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(461, 64);
            this.txtComments.TabIndex = 40;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 23);
            this.label3.TabIndex = 41;
            this.label3.Text = "Comments";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(2, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 23);
            this.label4.TabIndex = 42;
            this.label4.Text = "Contacts";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ultraTabControl3);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(795, 393);
            // 
            // ultraTabControl3
            // 
            this.ultraTabControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl3.Controls.Add(this.ultraTabSharedControlsPage3);
            this.ultraTabControl3.Controls.Add(this.ultraTabPageControl8);
            this.ultraTabControl3.Location = new System.Drawing.Point(4, 4);
            this.ultraTabControl3.Name = "ultraTabControl3";
            this.ultraTabControl3.SharedControlsPage = this.ultraTabSharedControlsPage3;
            this.ultraTabControl3.Size = new System.Drawing.Size(788, 411);
            this.ultraTabControl3.TabIndex = 44;
            ultraTab1.TabPage = this.ultraTabPageControl8;
            ultraTab1.Text = "Itinerary footer";
            this.ultraTabControl3.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            // 
            // ultraTabSharedControlsPage3
            // 
            this.ultraTabSharedControlsPage3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage3.Name = "ultraTabSharedControlsPage3";
            this.ultraTabSharedControlsPage3.Size = new System.Drawing.Size(784, 385);
            // 
            // ultraTabPageControl9
            // 
            this.ultraTabPageControl9.Controls.Add(this.ultraTabControl4);
            this.ultraTabPageControl9.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl9.Name = "ultraTabPageControl9";
            this.ultraTabPageControl9.Size = new System.Drawing.Size(795, 393);
            // 
            // ultraTabControl4
            // 
            this.ultraTabControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl4.Controls.Add(this.ultraTabSharedControlsPage4);
            this.ultraTabControl4.Controls.Add(this.ultraTabPageControl10);
            this.ultraTabControl4.Controls.Add(this.ultraTabPageControl11);
            this.ultraTabControl4.Location = new System.Drawing.Point(2, 3);
            this.ultraTabControl4.Name = "ultraTabControl4";
            this.ultraTabControl4.SharedControlsPage = this.ultraTabSharedControlsPage4;
            this.ultraTabControl4.Size = new System.Drawing.Size(790, 412);
            this.ultraTabControl4.TabIndex = 43;
            ultraTab2.TabPage = this.ultraTabPageControl10;
            ultraTab2.Text = "Header";
            ultraTab3.TabPage = this.ultraTabPageControl11;
            ultraTab3.Text = "Footer";
            this.ultraTabControl4.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab2,
            ultraTab3});
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(786, 386);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.label17);
            this.ultraTabPageControl3.Controls.Add(this.label14);
            this.ultraTabPageControl3.Controls.Add(this.cmbMargin);
            this.ultraTabPageControl3.Controls.Add(this.label13);
            this.ultraTabPageControl3.Controls.Add(this.txtCurrencyMargin);
            this.ultraTabPageControl3.Controls.Add(this.label8);
            this.ultraTabPageControl3.Controls.Add(this.label7);
            this.ultraTabPageControl3.Controls.Add(this.label5);
            this.ultraTabPageControl3.Controls.Add(this.label2);
            this.ultraTabPageControl3.Controls.Add(this.cmbOverride);
            this.ultraTabPageControl3.Controls.Add(this.label16);
            this.ultraTabPageControl3.Controls.Add(this.label15);
            this.ultraTabPageControl3.Controls.Add(this.gridMarginOverride);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(795, 393);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Location = new System.Drawing.Point(214, 159);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(119, 13);
            this.label17.TabIndex = 161;
            this.label17.Text = "the specified % override";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(180, 138);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 13);
            this.label14.TabIndex = 160;
            this.label14.Text = "to be";
            // 
            // cmbMargin
            // 
            this.cmbMargin.DisplayMember = "Text";
            this.cmbMargin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMargin.FormattingEnabled = true;
            this.cmbMargin.Location = new System.Drawing.Point(64, 134);
            this.cmbMargin.Name = "cmbMargin";
            this.cmbMargin.Size = new System.Drawing.Size(110, 21);
            this.cmbMargin.TabIndex = 159;
            this.cmbMargin.ValueMember = "Value";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Location = new System.Drawing.Point(8, 138);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 13);
            this.label13.TabIndex = 158;
            this.label13.Text = "Adjust my";
            // 
            // txtCurrencyMargin
            // 
            this.txtCurrencyMargin.FormatString = "##0.00";
            this.txtCurrencyMargin.Location = new System.Drawing.Point(163, 18);
            this.txtCurrencyMargin.MaskInput = "-nnn.nn";
            this.txtCurrencyMargin.Name = "txtCurrencyMargin";
            this.txtCurrencyMargin.Nullable = true;
            this.txtCurrencyMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtCurrencyMargin.Size = new System.Drawing.Size(110, 21);
            this.txtCurrencyMargin.TabIndex = 150;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.CausesValidation = false;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(272, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 13);
            this.label8.TabIndex = 156;
            this.label8.Text = "%";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(8, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(145, 13);
            this.label7.TabIndex = 155;
            this.label7.Text = "Default Booking Margin Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.CausesValidation = false;
            this.label5.Location = new System.Drawing.Point(340, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 13);
            this.label5.TabIndex = 153;
            this.label5.Text = "Default Margins for Booking Types";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.CausesValidation = false;
            this.label2.Location = new System.Drawing.Point(8, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 43);
            this.label2.TabIndex = 152;
            this.label2.Text = "Only when you have default margins set (see grid on right),\r\nthis sets the method" +
    " (markup, commission, or gross) used to \r\ncalculate the margin.";
            // 
            // cmbOverride
            // 
            this.cmbOverride.DisplayMember = "Text";
            this.cmbOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOverride.FormattingEnabled = true;
            this.cmbOverride.Location = new System.Drawing.Point(215, 135);
            this.cmbOverride.Name = "cmbOverride";
            this.cmbOverride.Size = new System.Drawing.Size(110, 21);
            this.cmbOverride.TabIndex = 133;
            this.cmbOverride.ValueMember = "Value";
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.CausesValidation = false;
            this.label16.Location = new System.Drawing.Point(8, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(302, 46);
            this.label16.TabIndex = 149;
            this.label16.Text = "Only for Webservice rates, this sets a default margin to add\r\nto the \'now\' rate r" +
    "eturned from the webservice, whenver you\r\nupdate exchange rates (user can overri" +
    "de).";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.CausesValidation = false;
            this.label15.Location = new System.Drawing.Point(8, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(121, 13);
            this.label15.TabIndex = 148;
            this.label15.Text = "Default Currency Margin";
            // 
            // gridMarginOverride
            // 
            this.gridMarginOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            appearance3.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridMarginOverride.DisplayLayout.Appearance = appearance3;
            this.gridMarginOverride.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridMarginOverride.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance4.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMarginOverride.DisplayLayout.GroupByBox.Appearance = appearance4;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMarginOverride.DisplayLayout.GroupByBox.BandLabelAppearance = appearance5;
            this.gridMarginOverride.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackColor2 = System.Drawing.SystemColors.Control;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMarginOverride.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.gridMarginOverride.DisplayLayout.MaxColScrollRegions = 1;
            this.gridMarginOverride.DisplayLayout.MaxRowScrollRegions = 1;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            appearance7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridMarginOverride.DisplayLayout.Override.ActiveCellAppearance = appearance7;
            appearance8.BackColor = System.Drawing.SystemColors.Highlight;
            appearance8.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridMarginOverride.DisplayLayout.Override.ActiveRowAppearance = appearance8;
            this.gridMarginOverride.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridMarginOverride.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            this.gridMarginOverride.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridMarginOverride.DisplayLayout.Override.CellAppearance = appearance10;
            this.gridMarginOverride.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridMarginOverride.DisplayLayout.Override.CellPadding = 0;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMarginOverride.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            appearance12.TextHAlignAsString = "Left";
            this.gridMarginOverride.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.gridMarginOverride.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridMarginOverride.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.gridMarginOverride.DisplayLayout.Override.RowAppearance = appearance13;
            this.gridMarginOverride.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridMarginOverride.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.gridMarginOverride.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridMarginOverride.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridMarginOverride.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.gridMarginOverride.Enabled = false;
            this.gridMarginOverride.Location = new System.Drawing.Point(341, 30);
            this.gridMarginOverride.Name = "gridMarginOverride";
            this.gridMarginOverride.Size = new System.Drawing.Size(374, 360);
            this.gridMarginOverride.TabIndex = 125;
            this.gridMarginOverride.Text = "ultraGrid1";
            this.gridMarginOverride.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridMarginOverride_InitializeLayout);
            this.gridMarginOverride.AfterExitEditMode += new System.EventHandler(this.gridMarginOverride_AfterExitEditMode);
            // 
            // gridAgents
            // 
            this.gridAgents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridAgents.Location = new System.Drawing.Point(4, 89);
            this.gridAgents.Name = "gridAgents";
            this.gridAgents.Size = new System.Drawing.Size(799, 207);
            this.gridAgents.TabIndex = 1;
            this.gridAgents.Text = "Agent Labels";
            this.gridAgents.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridAgents_AfterCellUpdate);
            this.gridAgents.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridAgents_InitializeLayout);
            this.gridAgents.AfterRowActivate += new System.EventHandler(this.gridAgents_AfterRowActivate);
            this.gridAgents.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridAgents_CellChange);
            this.gridAgents.Click += new System.EventHandler(this.gridAgents_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 29;
            this.lblHeading.Text = "Agent labels";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(500, 40);
            this.label6.TabIndex = 28;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 73);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 126;
            this.label10.Text = "Agent Labels";
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(795, 393);
            // 
            // Currenc
            // 
            this.Currenc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Currenc.Controls.Add(this.ultraTabSharedControlsPage1);
            this.Currenc.Controls.Add(this.ultraTabPageControl2);
            this.Currenc.Controls.Add(this.ultraTabPageControl1);
            this.Currenc.Controls.Add(this.ultraTabPageControl9);
            this.Currenc.Controls.Add(this.ultraTabPageControl3);
            this.Currenc.Location = new System.Drawing.Point(4, 302);
            this.Currenc.Name = "Currenc";
            this.Currenc.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.Currenc.Size = new System.Drawing.Size(799, 419);
            this.Currenc.TabIndex = 47;
            ultraTab4.Key = "Agent";
            ultraTab4.TabPage = this.ultraTabPageControl2;
            ultraTab4.Text = "Agent";
            ultraTab5.Key = "Clients";
            ultraTab5.TabPage = this.ultraTabPageControl1;
            ultraTab5.Text = "Client texts";
            ultraTab6.Key = "Vouchers";
            ultraTab6.TabPage = this.ultraTabPageControl9;
            ultraTab6.Text = "Voucher Texts";
            ultraTab7.Key = "MarginOverrides";
            ultraTab7.TabPage = this.ultraTabPageControl3;
            ultraTab7.Text = "Margin overrides";
            this.Currenc.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab4,
            ultraTab5,
            ultraTab6,
            ultraTab7});
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAgentCopy,
            this.btnAgentAdd,
            this.btnAgentDel});
            this.myToolStrip1.Location = new System.Drawing.Point(731, 66);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(72, 25);
            this.myToolStrip1.TabIndex = 125;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // btnAgentCopy
            // 
            this.btnAgentCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAgentCopy.Image = global::TourWriter.Properties.Resources.PageCopy;
            this.btnAgentCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAgentCopy.Name = "btnAgentCopy";
            this.btnAgentCopy.Size = new System.Drawing.Size(23, 22);
            this.btnAgentCopy.Click += new System.EventHandler(this.btnAgentCopy_Click);
            // 
            // btnAgentAdd
            // 
            this.btnAgentAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAgentAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnAgentAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAgentAdd.Name = "btnAgentAdd";
            this.btnAgentAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAgentAdd.Click += new System.EventHandler(this.btnAgentAdd_Click);
            // 
            // btnAgentDel
            // 
            this.btnAgentDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAgentDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnAgentDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAgentDel.Name = "btnAgentDel";
            this.btnAgentDel.Size = new System.Drawing.Size(23, 22);
            this.btnAgentDel.Click += new System.EventHandler(this.btnAgentDel_Click);
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnContactAdd,
            this.btnContactDel});
            this.myToolStrip2.Location = new System.Drawing.Point(476, 90);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 127;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnContactAdd
            // 
            this.btnContactAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnContactAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnContactAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnContactAdd.Name = "btnContactAdd";
            this.btnContactAdd.Size = new System.Drawing.Size(23, 22);
            this.btnContactAdd.Click += new System.EventHandler(this.btnContactAdd_Click);
            // 
            // btnContactDel
            // 
            this.btnContactDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnContactDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnContactDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnContactDel.Name = "btnContactDel";
            this.btnContactDel.Size = new System.Drawing.Size(23, 22);
            this.btnContactDel.Click += new System.EventHandler(this.btnContactDel_Click);
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.DisableAllMenuItems = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(0, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.miniToolStrip.Size = new System.Drawing.Size(49, 25);
            this.miniToolStrip.TabIndex = 127;
            // 
            // AgentLabels
            // 
            this.AllowDrop = true;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridAgents);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.myToolStrip1);
            this.Controls.Add(this.Currenc);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "AgentLabels";
            this.Size = new System.Drawing.Size(815, 724);
            this.Load += new System.EventHandler(this.AgentLabels_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AgentLabel_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AgentLabel_DragEnter);
            this.ultraTabPageControl8.ResumeLayout(false);
            this.ultraTabPageControl8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTermsConditions)).EndInit();
            this.ultraTabPageControl10.ResumeLayout(false);
            this.ultraTabPageControl10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVoucherLogo)).EndInit();
            this.ultraTabPageControl11.ResumeLayout(false);
            this.ultraTabPageControl11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVoucherFooter)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridContact)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComments)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl3)).EndInit();
            this.ultraTabControl3.ResumeLayout(false);
            this.ultraTabPageControl9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl4)).EndInit();
            this.ultraTabControl4.ResumeLayout(false);
            this.ultraTabPageControl3.ResumeLayout(false);
            this.ultraTabPageControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrencyMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMarginOverride)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Currenc)).EndInit();
            this.Currenc.ResumeLayout(false);
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion										
		#endregion

		private void AgentLabels_Load(object sender, EventArgs e)
		{
			DataBind();
            
            cmbOverride.DataSource = new ArrayList {new { Text = "Minimum of", Value = "min" }, 
                                                 new { Text = "Maximum of", Value = "max" },
                                                 new { Text = "Exactly", Value = "exact" }};
            cmbOverride.SelectedIndexChanged += cmbOverride_SelectedIndexChanged;

            cmbMargin.DataSource = new ArrayList {new { Text = "Markup", Value = "mup" }, 
                                                 new { Text = "Commission", Value = "com" },
                                                 new { Text = "Agent Commission", Value = "grs" }};
            cmbMargin.SelectedIndexChanged += cmbMargin_SelectedIndexChanged;

			if(gridAgents.Rows.Count < 1)
				Currenc.Enabled = false;
		}

        private void DataBind()
	    {
	        EnsureAgentCodes();

            DataTable table = Cache.ToolSet.ServiceType.Copy();

            // copy the row filter settings from the original table
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);
            dataSet.DefaultViewManager.DataViewSettings[dataSet.Tables[0].TableName].RowFilter =
                Cache.ToolSet.DefaultViewManager.DataViewSettings[Cache.ToolSet.ServiceType.TableName].RowFilter;

            table.Columns.Add("Override", typeof(decimal));
            gridMarginOverride.DataSource = table;

			gridAgents.SetDataBinding(Cache.AgentSet, "Agent");
            txtTermsConditions.DataBindings.Add("Text", Cache.AgentSet, "Agent.ClientFooter");
            txtVoucherLogo.DataBindings.Add("Text", Cache.AgentSet, "Agent.VoucherLogoFile");
            txtVoucherFooter.DataBindings.Add("Text", Cache.AgentSet, "Agent.VoucherFooter");
            gridContact.SetDataBinding(Cache.AgentSet, "Agent.AgentAgentContact");
            txtComments.DataBindings.Add("Text", Cache.AgentSet, "Agent.Comments");
            txtCurrencyMargin.DataBindings.Add("Value", Cache.AgentSet, "Agent.DefaultCurrencyMargin");
		}
		
		private void EndAllEdits()
		{
			Validate();
			gridAgents.UpdateData();
			gridContact.UpdateData();
            gridMarginOverride.UpdateData();
            SaveMarginOverrideChanges();
		}

		#region Agent
        private AgentSet.AgentRow GetSelectedAgentRow()
        {
            int agentId = (int)gridAgents.ActiveRow.Cells["AgentID"].Value;
            return Cache.AgentSet.Agent.FindByAgentID(agentId);
        }

        #region AgentCode (new property on Agent table)
        private static readonly Random Rand = new Random();
        /// <summary>
        /// Create a randomized 8 char string.
        /// </summary>
        /// <returns></returns>
        public static string CreateAgentCode()
        {
            const string alphaSeed = "23456789abcdefghijkmnopqrstuvwxyz";
            var rand = new string(Enumerable.Repeat(alphaSeed, 8).Select(s => s[Rand.Next(s.Length)]).ToArray());
            return rand;
        }

        /// <summary>
        /// Populate the new AgentCode property on the Agent table, this was added late and old rows will be NULL.
        /// </summary>
        private static void EnsureAgentCodes()
        {
            var agents = Cache.AgentSet.Agent.Where(agent => agent.IsAgentCodeNull());
            if (agents.Any())
            {
                Cache.AgentSet.Agent.Columns["AgentCode"].ReadOnly = false;
                foreach (var agent in agents) agent.AgentCode = CreateAgentCode();
                Cache.AgentSet.Agent.Columns["AgentCode"].ReadOnly = true;
            }
        }
        #endregion

        private void AddAgent()
        {
            // initialise new row
            AgentSet.AgentRow r = Cache.AgentSet.Agent.NewAgentRow();
            r.AgentName = App.CreateUniqueNameValue(gridAgents.Rows, "AgentName", "New Agent");
            r.IsDefaultAgent = (Cache.AgentSet.Agent.Rows.Count == 0) ? true : false;
            r.AddedOn = DateTime.Now;
            r.AddedBy = Cache.User.UserID;

            Cache.AgentSet.Agent.Columns["AgentCode"].ReadOnly = false;
            r.AgentCode = CreateAgentCode();
            Cache.AgentSet.Agent.Columns["AgentCode"].ReadOnly = true;

            // add the row
            Cache.AgentSet.Agent.AddAgentRow(r);

            // select new row
            gridAgents.ActiveRow = gridAgents.Rows[gridAgents.Rows.Count - 1];

            // enable agent details to be entered
            Currenc.Enabled = true;

            EnableDisableAgents();
            RebuildAgentList();
        }

        private void DeleteAgent()
        {
            if (gridAgents.ActiveRow == null)
                return;

            int agentId = (int)gridAgents.ActiveRow.Cells["AgentID"].Value;

            string msg;
            if (AgentHasChildren(agentId))
            {
                msg = "This agent has child agents. Deleting it will cause the child agents to be orphaned.\r\n" +
                      "Are you sure you want to delete it?";
            }
            else
            {
                msg = App.GetResourceString("AskDeleteRow");
            }

            if (App.AskYesNo(msg))
                GridHelper.DeleteActiveRow(gridAgents, true);

            EnableDisableAgents();
            RebuildAgentList();
        }

        private void CopyAgent()
        {
            if (gridAgents.ActiveRow == null) return;
            AgentSet.AgentRow copyAgent = Cache.AgentSet.Agent.FindByAgentID((int)gridAgents.ActiveRow.Cells["AgentID"].Value);
            if (App.AskYesNo("Copy agent: " + copyAgent.AgentName + "?"))
            {
                // agent
                var agent = Cache.AgentSet.Agent.NewAgentRow();
                var id = agent.AgentID;
                agent.ItemArray = copyAgent.ItemArray;
                agent.AgentID = id;
                agent.IsDefaultAgent = false;
                agent.AgentName = agent.AgentName + " (copy)";
                Cache.AgentSet.Agent.AddAgentRow(agent);

                // contacts
                foreach (var row in copyAgent.GetAgentContactRows())
                {
                    var contact = Cache.AgentSet.AgentContact.NewAgentContactRow();
                    contact.ItemArray = row.ItemArray;
                    contact.AgentID = id;
                    Cache.AgentSet.AgentContact.AddAgentContactRow(contact);
                }

                // margins
                foreach (var row in copyAgent.GetAgentMarginRows())
                {
                    var margin = Cache.AgentSet.AgentMargin.NewAgentMarginRow();
                    margin.ItemArray = row.ItemArray;
                    margin.AgentID = id;
                    Cache.AgentSet.AgentMargin.AddAgentMarginRow(margin);
                }

                // purchs term
                if (!copyAgent.IsPurchasePaymentTermIDNull())
                {
                    var copyPurch = Cache.AgentSet.PaymentTerm.Where(x => x.RowState != DataRowState.Deleted && x.PaymentTermID == copyAgent.PurchasePaymentTermID).FirstOrDefault();
                    if (copyPurch != null)
                    {
                        var purch = Cache.AgentSet.PaymentTerm.NewPaymentTermRow();
                        id = purch.PaymentTermID;
                        purch.ItemArray = copyPurch.ItemArray;
                        purch.PaymentTermID = id;
                        Cache.AgentSet.PaymentTerm.AddPaymentTermRow(purch);
                        agent.PurchasePaymentTermID = purch.PaymentTermID;
                    }
                }

                // sales term
                if (!copyAgent.IsSalePaymentTermIDNull())
                {
                    var copySale = Cache.AgentSet.PaymentTerm.Where(x => x.RowState != DataRowState.Deleted && x.PaymentTermID == copyAgent.SalePaymentTermID).FirstOrDefault();
                    if (copySale != null)
                    {
                        var sale = Cache.AgentSet.PaymentTerm.NewPaymentTermRow();
                        id = sale.PaymentTermID;
                        sale.ItemArray = copySale.ItemArray;
                        sale.PaymentTermID = id;
                        Cache.AgentSet.PaymentTerm.AddPaymentTermRow(sale);
                        agent.SalePaymentTermID = sale.PaymentTermID;
                    }
                }
                GridHelper.SetActiveRow(gridAgents, "AgentID", agent.AgentID, "AgentName");
            }

            RebuildAgentList();
        }

        private static bool AgentHasChildren(int agentId)
        {
            // check if the agent has children
            bool hasChildren = false;
            foreach (AgentSet.AgentRow row in Cache.AgentSet.Agent)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                if (!row.IsParentAgentIDNull() && row.ParentAgentID == agentId)
                {
                    hasChildren = true;
                    break;
                }
            }

            return hasChildren;
        }

        private void RebuildAgentList()
        {
            if (!gridAgents.DisplayLayout.ValueLists.Exists("AgentList"))
            {
                // create the value list if it doesn't exist
                gridAgents.DisplayLayout.ValueLists.Add("AgentList");
                gridAgents.DisplayLayout.ValueLists["AgentList"].SortStyle = ValueListSortStyle.Ascending;
            }

            // clear the value list
            gridAgents.DisplayLayout.ValueLists["AgentList"].ValueListItems.Clear();

            // populate the value list with agents
            gridAgents.DisplayLayout.ValueLists["AgentList"].ValueListItems.Add(DBNull.Value, "(none)");
            foreach (AgentSet.AgentRow r in Cache.AgentSet.Agent.Rows)
            {
                if (r.RowState != DataRowState.Deleted)
                    gridAgents.DisplayLayout.ValueLists["AgentList"].ValueListItems.Add(r.AgentID, r.AgentName);
            }
        }

		private void gridAgents_InitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
		    RebuildAgentList();

			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
				if(c.Key == "IsDefaultAgent")
				{
					c.Header.Caption = "Default";
					c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
				    c.CellClickAction = CellClickAction.Edit;
				}
				else if(c.Key == "AgentName")
				{
					c.Header.Caption = "Name";
					c.SortIndicator = SortIndicator.Ascending;
					c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "ParentAgentID")
                {
                    c.Header.Caption = "Belongs to";
                    c.Header.ToolTipText = "Agent can belong to parent agent";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    c.ValueList = gridAgents.DisplayLayout.ValueLists["AgentList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "AgentCode")
                {
                    c.Header.Caption = "Code";
                    c.Header.ToolTipText = "Unique system code";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.EditAndSelectText;
                }
				else
					c.Hidden = true;
			}
			// configure
		    GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

			GridHelper.LayoutCol(gridAgents, "IsDefaultAgent", 1, 20);
            GridHelper.LayoutCol(gridAgents, "AgentName", 2, 120);
            GridHelper.LayoutCol(gridAgents, "AgentCode", 4, 20);

			EnableDisableAgents();
		}


        private void gridAgents_AfterRowActivate(object sender, EventArgs e)
        {
            // clear last rows images
            // don't load row image, to save bandwidth
            picVoucherLogo.Image = null;

            gridMarginOverride.Enabled = true;
            InitializeOverrides();
        }

        private void gridAgents_Click(object sender, EventArgs e)
        {
            gridMarginOverride.Enabled = (gridAgents.ActiveRow != null);
        }

		private void gridAgents_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			// if default agent changed, unselect any others
			if(e.Cell.Column.Key == "IsDefaultAgent")
				if((bool)e.Cell.OriginalValue == false)
                    foreach (AgentSet.AgentRow a in Cache.AgentSet.Agent)
                        if(a.RowState != DataRowState.Deleted)
						    if(a.AgentID != (int)e.Cell.Row.Cells["AgentID"].Value)
                                if(a.IsDefaultAgent)
								    a.IsDefaultAgent = false;
		}

        private void gridAgents_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key == "AgentName")
            {
                RebuildAgentList();
            }
        }

		private void btnAgentAdd_Click(object sender, EventArgs e)
		{
		    AddAgent();
		}

		private void btnAgentDel_Click(object sender, EventArgs e)
		{
		    DeleteAgent();
		}

        private void btnAgentCopy_Click(object sender, EventArgs e)
        {
            CopyAgent();
        }
										
		private void btnLogoFile_Click(object sender, EventArgs e)
		{	
            string file = App.SelectExternalFile(true, "Select reports logo",
                "Images (*.BMP;*.JPG;*.JPEG;*.GIF)|*.BMP;*.JPG;*.JPEG;*.GIF|All files (*.*)|*.*", 1);

            if(file != null)
			{
                gridAgents.ActiveRow.Cells["LogoFile"].Value = file;
				gridAgents.UpdateData();
                //txtLogoFile.Text = file; note: remove this later
				
				try
				{
                    /*picLogo.Image = null; note: remove this later
                    picLogo.Image = Image.FromFile(ExternalFilesHelper.ConvertToAbsolutePath(file));*/
				}
				catch(OutOfMemoryException ex)
				{
					throw new System.IO.FileLoadException(
                        "Invalid file format for file " + file, ex);
				}
			}
		}
		
		private void btnVoucherLogo_Click(object sender, EventArgs e)
		{
            string file = App.SelectExternalFile(true, "Select voucher logo", 
                "Images (*.BMP;*.JPG;*.JPEG;*.GIF)|*.BMP;*.JPG;*.JPEG;*.GIF|All files (*.*)|*.*", 1);
            
            if (file != null)
			{
                gridAgents.ActiveRow.Cells["VoucherLogoFile"].Value = file;
				gridAgents.UpdateData();
                txtVoucherLogo.Text = file;
				
				try
				{
					picVoucherLogo.Image = null;
                    picVoucherLogo.Image = Image.FromFile(ExternalFilesHelper.ConvertToAbsolutePath(file));
				}
				catch(OutOfMemoryException ex)
				{
					throw new System.IO.FileLoadException(
                        "Invalid file format for file " + file, ex);
				}
			}
		}

		private void EnableDisableAgents()
		{
			Currenc.Enabled = gridAgents.Rows.Count > 0;
		}
		#endregion

		#region Contact
		private void gridContact_InitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			// add button column
			if(!e.Layout.Bands[0].Columns.Exists("View"))
				e.Layout.Bands[0].Columns.Insert(0, "View");
            
			gridContact.DataSource = null;
            gridContact.DataSource = Cache.AgentSet.AgentContact;

			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
				if(c.Key == "Description")
				{
					c.Header.Caption = "Contact Notes";
					//c.Width = 80;
				}
				else if(c.Key == "IsDefaultContact")
				{
					c.Header.Caption = "Contact";
					c.Width = 30;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
				}
				else if(c.Key == "IsDefaultBilling")
				{
					c.Header.Caption = "Billing";
					c.Width = 30;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
				}
				else if(c.Key == "View")
				{
					c.Width = 30;
					c.Header.Caption = "View...";					
					c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
					c.ButtonDisplayStyle = ButtonDisplayStyle.Always;
				}
				else
					c.Hidden = true;
			}
			// configure
		    GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

			e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
		}
		
		private void gridContact_InitializeRow(object sender, InitializeRowEventArgs e)
		{
			e.Row.Cells["View"].Value = "...";
		}
		
		private void gridContact_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			if(e.Cell.Column.Key == "View")
			{
                ContactMain contact = new ContactMain((int)gridContact.ActiveRow.Cells["ContactID"].Value);	
		
				if (contact.ShowDialog(this) == DialogResult.OK)
				{
					// reload contact to reflect changes
                    Cache.AgentSet.Contact.BeginLoadData();
                    Cache.AgentSet.Contact.LoadDataRow(((ContactSet.ContactRow)contact.ContactRow).ItemArray, true);
                    Cache.AgentSet.Contact.EndLoadData();
				}
				contact.Dispose();
			}		
		}

		private void gridContact_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{		
			if(e.Cell.Column.Key == "IsDefaultContact")
			{	
				if(!(bool)e.Cell.Value) // underlying not ticked so this a tick
				{
					// make others false
					foreach(UltraGridRow r in gridContact.Rows)
						if(r != e.Cell.Row && (bool)r.Cells["IsDefaultContact"].Value) 
							r.Cells["IsDefaultContact"].Value = false;
				}	
			}
			else if(e.Cell.Column.Key == "IsDefaultBilling")
			{	
				if(!(bool)e.Cell.Value) // underlying not ticked so this a tick
				{
					// make others false
					foreach(UltraGridRow r in gridContact.Rows)
						if(r != e.Cell.Row && (bool)r.Cells["IsDefaultBilling"].Value) 
							r.Cells["IsDefaultBilling"].Value = false;
				}	
			}
		}
				
		private void btnContactAdd_Click(object sender, EventArgs e)
		{
			// open contact dialog with new contact row
			ContactMain contact = new ContactMain();	
		
			if (contact.ShowDialog(this) == DialogResult.OK)
			{
				// get new Contact row
				ContactSet.ContactRow c = (ContactSet.ContactRow)contact.ContactRow;

				// initialise new AgentContact row
                AgentSet.AgentContactRow ac = Cache.AgentSet.AgentContact.NewAgentContactRow();
				ac.AgentID = (int)gridAgents.ActiveRow.Cells["AgentID"].Value;
				ac.ContactID = c.ContactID;
				ac.Description = c.ContactName;
                bool isFirstRow = (Cache.AgentSet.AgentContact.Rows.Count == 0);
				ac.IsDefaultContact = isFirstRow;
				ac.IsDefaultBilling = isFirstRow;

				// add the row
                Cache.AgentSet.Contact.ImportRow(c);
                Cache.AgentSet.AgentContact.AddAgentContactRow(ac);
				
				// activate new row
				gridContact.Focus();
				gridContact.ActiveCell = gridContact.Rows[gridContact.Rows.Count-1].Cells["Description"];
				gridContact.PerformAction(UltraGridAction.EnterEditMode, false, false);
			}
			contact.Dispose();

		}

		private void btnContactDel_Click(object sender, EventArgs e)
		{			
			UltraGridRow row;
			row = gridContact.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				int id = (int)row.Cells["ContactID"].Value;
				row.Delete(false);
                Cache.AgentSet.Contact.RemoveContactRow(Cache.AgentSet.Contact.FindByContactID(id));

				// select next row
				if( ( i > gridContact.Rows.Count-1 ? --i : i) > -1)					 
				{
					gridContact.ActiveRow = gridContact.Rows[i];
					gridContact.ActiveCell = gridContact.ActiveRow.Cells["Description"];
				}
			}
		}
				
		private void AgentLabel_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(SelectedNodesCollection)))
			{
				e.Effect = DragDropEffects.Link;
			}
			else
				e.Effect = DragDropEffects.None;
		}    

		private void AgentLabel_DragDrop(object sender, DragEventArgs e)
		{
            Object obj = e.Data.GetData(typeof(SelectedNodesCollection));
            SelectedNodesCollection nodeCollection = obj as SelectedNodesCollection;
			
			for(int i = 0; i < nodeCollection.Count; i++)
			{
                NavigationTreeItemInfo nodeInfo = nodeCollection[i].Tag as NavigationTreeItemInfo;

				if(nodeInfo.ItemType == NavigationTreeItemInfo.ItemTypes.Contact)
				{
					int agentID = (int)gridAgents.ActiveRow.Cells["AgentID"].Value;
					int contactID = nodeInfo.ItemID;
					string contactName = nodeInfo.ItemName;

					// ensure row doesn't already exist
                    AgentSet.AgentContactRow row = Cache.AgentSet.AgentContact.FindByAgentIDContactID(
						agentID, contactID);
					
					if(row != null)
					{
						MessageBox.Show(App.GetResourceString("ShowRowAlreadyExists"));
					}
					else
					{
						// first get the contact row to handle constraints
                        if (Cache.AgentSet.Contact.FindByContactID(contactID) == null)
						{						
							Contact c = new Contact();
                            Cache.AgentSet.Contact.ImportRow(c.GetContactSet(contactID).Contact[0]);	
						}
						// create new AgentContact row
                        AgentSet.AgentContactRow ac = Cache.AgentSet.AgentContact.NewAgentContactRow();
						ac.AgentID = agentID;
						ac.ContactID = contactID;
						ac.Description = contactName;
                        bool isFirstRow = (Cache.AgentSet.AgentContact.Rows.Count == 0);
						ac.IsDefaultContact = isFirstRow;
						ac.IsDefaultBilling = isFirstRow;

						// add the new row	
                        Cache.AgentSet.AgentContact.AddAgentContactRow(ac);

						// activate new row
						gridContact.Focus();
						gridContact.ActiveCell = gridContact.Rows[gridContact.Rows.Count-1].Cells["Description"];
						gridContact.PerformAction(UltraGridAction.EnterEditMode, false, false);
					}
				}
			}		
		}
		#endregion

        #region Margin override

        private void InitializeOverrides()
        {
            DataTable table = (DataTable)gridMarginOverride.DataSource;
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                // populate override column
                AgentSet.AgentMarginRow agentMarginRow =
                    Cache.AgentSet.AgentMargin.FindByAgentIDServiceTypeID(
                    (int)gridAgents.ActiveRow.Cells["AgentID"].Value, (int)row["ServiceTypeID"]);

                if (agentMarginRow != null)
                    row["Override"] = agentMarginRow.Margin;
                else
                    row["Override"] = DBNull.Value;
            }

            AgentSet.AgentRow agentRow = GetSelectedAgentRow();
            if (!agentRow.IsNetComOrMupNull())
            {
                cmbMargin.SelectedValue = agentRow.NetComOrMup;
            }
            else
            {
                cmbMargin.SelectedIndex = -1;
            }

            if (!agentRow.IsNetMinOrMaxNull())
            {
                cmbOverride.SelectedValue = agentRow.NetMinOrMax;
            }
            else
            {
                cmbOverride.SelectedIndex = -1;
            }            
        }        

        private void SaveMarginOverrideChanges()
        {
            if (gridAgents.ActiveRow == null)
                return;

            foreach (UltraGridRow row in gridMarginOverride.Rows)
            {
                int agentId = (int)gridAgents.ActiveRow.Cells["AgentID"].Value;
                int serviceTypeId = (int)row.Cells["ServiceTypeID"].Value;
                decimal? margin = row.Cells["Override"].Value != DBNull.Value ?
                                  (decimal?)row.Cells["Override"].Value : 
                                  null;

                Cache.AgentSet.AgentMargin.AddInsertOrDelete(agentId, serviceTypeId, margin);
            }
        }        

        private void gridMarginOverride_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ServiceTypeName")
                {
                    c.Width = 110;
                    c.Header.Caption = "Service type";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "Override")
                {
                    c.Width = 70;
                    c.MaxWidth = 70;
                    c.Format = "#0.00\\%";
                    c.MaskInput = "nnn.nn% ";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                {
                    c.Hidden = true;
                    c.TabStop = false;
                }
            }

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
        }

        private void gridMarginOverride_AfterExitEditMode(object sender, EventArgs e)
        {
            SaveMarginOverrideChanges();
        }

        #endregion

        protected override void OnValidating(CancelEventArgs e)
		{
			EndAllEdits();
			base.OnValidating(e);
		}

        private void cmbOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOverride.SelectedIndex >= 0)
            {
                var selectedRow = GetSelectedAgentRow();
                if (!selectedRow.IsNetComOrMupNull() && selectedRow.NetComOrMup != (string)cmbMargin.SelectedValue)
                    selectedRow.NetComOrMup = (string)cmbMargin.SelectedValue; // update it
            }            
        }

        private void cmbMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMargin.SelectedIndex >= 0)
            {
                var selectedRow = GetSelectedAgentRow();
                if (!selectedRow.IsNetComOrMupNull() && selectedRow.NetComOrMup != (string)cmbMargin.SelectedValue)
                    selectedRow.NetComOrMup = (string)cmbMargin.SelectedValue; // update it

                if (cmbMargin.SelectedValue == "grs")
                {
                    GetSelectedAgentRow().NetMinOrMax = null;
                    label14.Visible = false;
                    label17.Visible = false;
                    cmbOverride.Visible = false;
                    cmbOverride.SelectedIndex = -1; // remove selected
                }
                else
                {
                    label14.Visible = true;
                    label17.Visible = true;
                    cmbOverride.Visible = true;
                    cmbOverride.SelectedIndex = 2;  // default to 'exact'
                }
            }
        }
    }
}
