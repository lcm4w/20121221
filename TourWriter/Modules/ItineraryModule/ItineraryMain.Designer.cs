using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using TourWriter.Modules.ItineraryModule.Publishing;
using TourWriter.UserControls.Reports;

namespace TourWriter.Modules.ItineraryModule
{
    partial class ItineraryMain
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItineraryMain));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Message", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageTo");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageFrom");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageFile");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedOn");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedBy");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RowVersion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageItineraryMessage");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageSupplierMessage");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("MessageItineraryMessage", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ItineraryID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedOn");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedBy");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("MessageSupplierMessage", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SupplierID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedOn");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AddedBy");
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
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab9 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab12 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab13 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab8 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl12 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.bookingsViewer = new TourWriter.Modules.ItineraryModule.Bookings.BookingsViewer();
            this.ultraTabPageControl11 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.bookingsQuote = new TourWriter.Modules.ItineraryModule.Bookings.BookingsQuote();
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.reportControl = new TourWriter.UserControls.Reports.ReportManager();
            this.ultraTabPageControl6 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.txtTemplateFile = new System.Windows.Forms.TextBox();
            this.btnChooseTemplate = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.grpReports = new System.Windows.Forms.GroupBox();
            this.pnlReports = new System.Windows.Forms.Panel();
            this.optReport = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.publisher1 = new TourWriter.Modules.ItineraryModule.Publishing.PublisherMain();
            this.tabAccounting = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.accounting1 = new TourWriter.UserControls.Accounting.Accounting();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gridMessages = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.itinerarySet = new TourWriter.Info.ItinerarySet();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnMessagesAdd = new System.Windows.Forms.ToolStripButton();
            this.btnMessageDel = new System.Windows.Forms.ToolStripButton();
            this.label35 = new System.Windows.Forms.Label();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbAgentContact = new System.Windows.Forms.ComboBox();
            this.cmbCurrency = new TourWriter.UserControls.NullableComboBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.itineraryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtArriveDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.txtDepartDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.cmbDepartCity = new System.Windows.Forms.ComboBox();
            this.cityBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.toolSet = new TourWriter.Info.ToolSet();
            this.txtAddedBy = new System.Windows.Forms.TextBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.branchBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbDepartment = new System.Windows.Forms.ComboBox();
            this.departmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbAssignedTo = new System.Windows.Forms.ComboBox();
            this.userBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbSource = new System.Windows.Forms.ComboBox();
            this.itinerarySourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.itineraryStatusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmbAgent = new System.Windows.Forms.ComboBox();
            this.agentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtDepartNote = new System.Windows.Forms.TextBox();
            this.txtArriveNote = new System.Windows.Forms.TextBox();
            this.cmbArriveCity = new System.Windows.Forms.ComboBox();
            this.cityBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.txtDepartFlight = new System.Windows.Forms.TextBox();
            this.txtArriveFlight = new System.Windows.Forms.TextBox();
            this.txtCustomId = new System.Windows.Forms.TextBox();
            this.txtItineraryId = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.cmbOrgin = new System.Windows.Forms.ComboBox();
            this.countryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtItineraryName = new System.Windows.Forms.TextBox();
            this.headerLabel4 = new TourWriter.UserControls.HeaderLabel();
            this.headerLabel3 = new TourWriter.UserControls.HeaderLabel();
            this.headerLabel2 = new TourWriter.UserControls.HeaderLabel();
            this.headerLabel1 = new TourWriter.UserControls.HeaderLabel();
            this.label19 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.ultraTabPageControl18 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tabControlBookings = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl19 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.clientEditor = new TourWriter.Modules.ItineraryModule.ClientEditor();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tabControlAdditional = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.currencyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ultraTabPageControl7 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tabControl_Main = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.itinerarySetLoaderThread = new System.ComponentModel.BackgroundWorker();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugItinerary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSave = new System.Windows.Forms.ToolStripButton();
            this.toolRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolSaveClose = new System.Windows.Forms.ToolStripButton();
            this.toolHelp = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.itineraryTasks1 = new TourWriter.Modules.ItineraryModule.ItineraryTasks();
            this.ultraTabPageControl12.SuspendLayout();
            this.ultraTabPageControl11.SuspendLayout();
            this.ultraTabPageControl8.SuspendLayout();
            this.ultraTabPageControl6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optReport)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            this.tabAccounting.SuspendLayout();
            this.ultraTabPageControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itinerarySet)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.branchBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.departmentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itinerarySourceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countryBindingSource)).BeginInit();
            this.ultraTabPageControl18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlBookings)).BeginInit();
            this.tabControlBookings.SuspendLayout();
            this.ultraTabPageControl19.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlAdditional)).BeginInit();
            this.tabControlAdditional.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl_Main)).BeginInit();
            this.tabControl_Main.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl12
            // 
            this.ultraTabPageControl12.Controls.Add(this.bookingsViewer);
            this.ultraTabPageControl12.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl12.Name = "ultraTabPageControl12";
            this.ultraTabPageControl12.Size = new System.Drawing.Size(894, 743);
            // 
            // bookingsViewer
            // 
            this.bookingsViewer.AutoScroll = true;
            this.bookingsViewer.AutoScrollMinSize = new System.Drawing.Size(552, 320);
            this.bookingsViewer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bookingsViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookingsViewer.Location = new System.Drawing.Point(0, 0);
            this.bookingsViewer.Name = "bookingsViewer";
            this.bookingsViewer.Size = new System.Drawing.Size(894, 743);
            this.bookingsViewer.TabIndex = 0;
            this.bookingsViewer.OnOpenBooking += new TourWriter.Modules.ItineraryModule.Bookings.OnBookingsViewerEditBookingHandler(this.bookingsViewer_OnOpenBooking);
            // 
            // ultraTabPageControl11
            // 
            this.ultraTabPageControl11.Controls.Add(this.bookingsQuote);
            this.ultraTabPageControl11.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl11.Name = "ultraTabPageControl11";
            this.ultraTabPageControl11.Size = new System.Drawing.Size(783, 536);
            // 
            // bookingsQuote
            // 
            this.bookingsQuote.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bookingsQuote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookingsQuote.Location = new System.Drawing.Point(0, 0);
            this.bookingsQuote.Name = "bookingsQuote";
            this.bookingsQuote.Size = new System.Drawing.Size(783, 536);
            this.bookingsQuote.TabIndex = 0;
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Controls.Add(this.reportControl);
            this.ultraTabPageControl8.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(898, 743);
            // 
            // reportControl
            // 
            this.reportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportControl.GeneralParameters = ((System.Collections.Generic.Dictionary<string, object>)(resources.GetObject("reportControl.GeneralParameters")));
            this.reportControl.Location = new System.Drawing.Point(0, 0);
            this.reportControl.Name = "reportControl";
            this.reportControl.Size = new System.Drawing.Size(898, 743);
            this.reportControl.TabIndex = 0;
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Controls.Add(this.label11);
            this.ultraTabPageControl6.Controls.Add(this.groupBox1);
            this.ultraTabPageControl6.Controls.Add(this.label10);
            this.ultraTabPageControl6.Controls.Add(this.grpReports);
            this.ultraTabPageControl6.Controls.Add(this.optReport);
            this.ultraTabPageControl6.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl6.Name = "ultraTabPageControl6";
            this.ultraTabPageControl6.Size = new System.Drawing.Size(898, 743);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(6, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(886, 13);
            this.label11.TabIndex = 154;
            this.label11.Text = "NOTE: This page will be removed in the future (once new reports are completed for" +
    " you).";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.btnExportToExcel);
            this.groupBox1.Controls.Add(this.txtTemplateFile);
            this.groupBox1.Controls.Add(this.btnChooseTemplate);
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(886, 54);
            this.groupBox1.TabIndex = 153;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export to Excel";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(19, 25);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(68, 13);
            this.label26.TabIndex = 152;
            this.label26.Text = "Template file";
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportToExcel.Location = new System.Drawing.Point(805, 20);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(75, 23);
            this.btnExportToExcel.TabIndex = 148;
            this.btnExportToExcel.Text = "Export";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // txtTemplateFile
            // 
            this.txtTemplateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTemplateFile.Location = new System.Drawing.Point(93, 22);
            this.txtTemplateFile.Name = "txtTemplateFile";
            this.txtTemplateFile.Size = new System.Drawing.Size(672, 21);
            this.txtTemplateFile.TabIndex = 149;
            // 
            // btnChooseTemplate
            // 
            this.btnChooseTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseTemplate.Location = new System.Drawing.Point(765, 21);
            this.btnChooseTemplate.Name = "btnChooseTemplate";
            this.btnChooseTemplate.Size = new System.Drawing.Size(27, 23);
            this.btnChooseTemplate.TabIndex = 150;
            this.btnChooseTemplate.Text = "...";
            this.toolTip1.SetToolTip(this.btnChooseTemplate, "Click to browse for a template file");
            this.btnChooseTemplate.UseVisualStyleBackColor = true;
            this.btnChooseTemplate.Click += new System.EventHandler(this.btnChooseTemplate_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 57;
            this.label10.Text = "Itinerary reports";
            // 
            // grpReports
            // 
            this.grpReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReports.BackColor = System.Drawing.Color.Transparent;
            this.grpReports.Controls.Add(this.pnlReports);
            this.grpReports.Location = new System.Drawing.Point(199, 88);
            this.grpReports.Name = "grpReports";
            this.grpReports.Size = new System.Drawing.Size(696, 652);
            this.grpReports.TabIndex = 56;
            this.grpReports.TabStop = false;
            this.grpReports.Text = "Report name";
            // 
            // pnlReports
            // 
            this.pnlReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReports.Location = new System.Drawing.Point(3, 17);
            this.pnlReports.Name = "pnlReports";
            this.pnlReports.Size = new System.Drawing.Size(690, 632);
            this.pnlReports.TabIndex = 55;
            // 
            // optReport
            // 
            this.optReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.optReport.Appearance = appearance1;
            this.optReport.BackColor = System.Drawing.SystemColors.Window;
            this.optReport.BackColorInternal = System.Drawing.SystemColors.Window;
            this.optReport.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.optReport.ItemOrigin = new System.Drawing.Point(4, 0);
            valueListItem1.DataValue = "PricingDetailed";
            valueListItem1.DisplayText = "Pricing detailed";
            valueListItem2.DataValue = "PricingSummary";
            valueListItem2.DisplayText = "Pricing summary";
            valueListItem3.DataValue = "ClientFinalPricing";
            valueListItem3.DisplayText = "Client pricing";
            valueListItem4.DataValue = "ClientFullPricing";
            valueListItem4.DisplayText = "Client detailed pricing";
            valueListItem5.DataValue = "ClientNoPricing";
            valueListItem5.DisplayText = "Contact list";
            valueListItem6.DataValue = "SupplierRemittance";
            valueListItem6.DisplayText = "Supplier remittance";
            valueListItem7.DataValue = "Vouchers";
            valueListItem7.DisplayText = "Vouchers";
            valueListItem8.DataValue = "ExcelReport";
            valueListItem8.DisplayText = "Excel report";
            this.optReport.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4,
            valueListItem5,
            valueListItem6,
            valueListItem7,
            valueListItem8});
            this.optReport.ItemSpacingVertical = 10;
            this.optReport.Location = new System.Drawing.Point(5, 94);
            this.optReport.Name = "optReport";
            this.optReport.Size = new System.Drawing.Size(188, 646);
            this.optReport.TabIndex = 54;
            this.optReport.TextIndentation = 4;
            this.optReport.ValueChanged += new System.EventHandler(this.optReport_ValueChanged);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.publisher1);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(898, 743);
            // 
            // publisher1
            // 
            this.publisher1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.publisher1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.publisher1.Location = new System.Drawing.Point(2, 0);
            this.publisher1.Name = "publisher1";
            this.publisher1.Size = new System.Drawing.Size(894, 740);
            this.publisher1.TabIndex = 1;
            // 
            // tabAccounting
            // 
            this.tabAccounting.Controls.Add(this.accounting1);
            this.tabAccounting.Location = new System.Drawing.Point(-10000, -10000);
            this.tabAccounting.Name = "tabAccounting";
            this.tabAccounting.Size = new System.Drawing.Size(898, 743);
            // 
            // accounting1
            // 
            this.accounting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accounting1.Location = new System.Drawing.Point(0, 0);
            this.accounting1.Name = "accounting1";
            this.accounting1.Size = new System.Drawing.Size(898, 743);
            this.accounting1.TabIndex = 0;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.gridMessages);
            this.ultraTabPageControl4.Controls.Add(this.myToolStrip2);
            this.ultraTabPageControl4.Controls.Add(this.label35);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(898, 743);
            // 
            // gridMessages
            // 
            this.gridMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMessages.DataMember = "Message";
            this.gridMessages.DataSource = this.itinerarySet;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridMessages.DisplayLayout.Appearance = appearance2;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12});
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.Header.VisiblePosition = 0;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.Header.VisiblePosition = 1;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn15.Header.VisiblePosition = 2;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.Header.VisiblePosition = 3;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16});
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn17.Header.VisiblePosition = 0;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn18.Header.VisiblePosition = 1;
            ultraGridColumn19.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn19.Header.VisiblePosition = 2;
            ultraGridColumn20.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn20.Header.VisiblePosition = 3;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20});
            this.gridMessages.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.gridMessages.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.gridMessages.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.gridMessages.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridMessages.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMessages.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMessages.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.gridMessages.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMessages.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.gridMessages.DisplayLayout.MaxColScrollRegions = 1;
            this.gridMessages.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridMessages.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridMessages.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.gridMessages.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridMessages.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.gridMessages.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridMessages.DisplayLayout.Override.CellAppearance = appearance9;
            this.gridMessages.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridMessages.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMessages.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.gridMessages.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.gridMessages.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridMessages.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.gridMessages.DisplayLayout.Override.RowAppearance = appearance12;
            this.gridMessages.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridMessages.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.gridMessages.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridMessages.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridMessages.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.gridMessages.Location = new System.Drawing.Point(2, 26);
            this.gridMessages.Name = "gridMessages";
            this.gridMessages.Size = new System.Drawing.Size(900, 717);
            this.gridMessages.TabIndex = 127;
            this.gridMessages.Text = "ultraGrid1";
            this.gridMessages.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridMessages_InitializeLayout);
            this.gridMessages.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.gridMessages_DoubleClickRow);
            // 
            // itinerarySet
            // 
            this.itinerarySet.DataSetName = "ItinerarySet";
            this.itinerarySet.Locale = new System.Globalization.CultureInfo("en-US");
            this.itinerarySet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMessagesAdd,
            this.btnMessageDel});
            this.myToolStrip2.Location = new System.Drawing.Point(848, 3);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 126;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnMessagesAdd
            // 
            this.btnMessagesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMessagesAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnMessagesAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMessagesAdd.Name = "btnMessagesAdd";
            this.btnMessagesAdd.Size = new System.Drawing.Size(23, 22);
            this.btnMessagesAdd.Text = "btnAdd";
            this.btnMessagesAdd.ToolTipText = "Add new message";
            this.btnMessagesAdd.Click += new System.EventHandler(this.btnMessagesAdd_Click);
            // 
            // btnMessageDel
            // 
            this.btnMessageDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMessageDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnMessageDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMessageDel.Name = "btnMessageDel";
            this.btnMessageDel.Size = new System.Drawing.Size(23, 22);
            this.btnMessageDel.Text = "btnDelete";
            this.btnMessageDel.ToolTipText = "Delete selected message";
            this.btnMessageDel.Click += new System.EventHandler(this.btnMessageDel_Click);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.BackColor = System.Drawing.Color.Transparent;
            this.label35.Location = new System.Drawing.Point(4, 10);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(225, 13);
            this.label35.TabIndex = 125;
            this.label35.Text = "Record of message sent to/from this itinerary";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.AutoScroll = true;
            this.ultraTabPageControl1.Controls.Add(this.label12);
            this.ultraTabPageControl1.Controls.Add(this.cmbAgentContact);
            this.ultraTabPageControl1.Controls.Add(this.cmbCurrency);
            this.ultraTabPageControl1.Controls.Add(this.lblCurrency);
            this.ultraTabPageControl1.Controls.Add(this.chkReadOnly);
            this.ultraTabPageControl1.Controls.Add(this.txtDisplayName);
            this.ultraTabPageControl1.Controls.Add(this.label9);
            this.ultraTabPageControl1.Controls.Add(this.txtArriveDate);
            this.ultraTabPageControl1.Controls.Add(this.txtDepartDate);
            this.ultraTabPageControl1.Controls.Add(this.cmbDepartCity);
            this.ultraTabPageControl1.Controls.Add(this.txtAddedBy);
            this.ultraTabPageControl1.Controls.Add(this.cmbBranch);
            this.ultraTabPageControl1.Controls.Add(this.cmbDepartment);
            this.ultraTabPageControl1.Controls.Add(this.cmbAssignedTo);
            this.ultraTabPageControl1.Controls.Add(this.cmbSource);
            this.ultraTabPageControl1.Controls.Add(this.cmbStatus);
            this.ultraTabPageControl1.Controls.Add(this.cmbAgent);
            this.ultraTabPageControl1.Controls.Add(this.txtComments);
            this.ultraTabPageControl1.Controls.Add(this.txtDepartNote);
            this.ultraTabPageControl1.Controls.Add(this.txtArriveNote);
            this.ultraTabPageControl1.Controls.Add(this.cmbArriveCity);
            this.ultraTabPageControl1.Controls.Add(this.txtDepartFlight);
            this.ultraTabPageControl1.Controls.Add(this.txtArriveFlight);
            this.ultraTabPageControl1.Controls.Add(this.txtCustomId);
            this.ultraTabPageControl1.Controls.Add(this.txtItineraryId);
            this.ultraTabPageControl1.Controls.Add(this.chkActive);
            this.ultraTabPageControl1.Controls.Add(this.cmbOrgin);
            this.ultraTabPageControl1.Controls.Add(this.txtItineraryName);
            this.ultraTabPageControl1.Controls.Add(this.headerLabel4);
            this.ultraTabPageControl1.Controls.Add(this.headerLabel3);
            this.ultraTabPageControl1.Controls.Add(this.headerLabel2);
            this.ultraTabPageControl1.Controls.Add(this.headerLabel1);
            this.ultraTabPageControl1.Controls.Add(this.label19);
            this.ultraTabPageControl1.Controls.Add(this.label16);
            this.ultraTabPageControl1.Controls.Add(this.label63);
            this.ultraTabPageControl1.Controls.Add(this.label2);
            this.ultraTabPageControl1.Controls.Add(this.label1);
            this.ultraTabPageControl1.Controls.Add(this.label5);
            this.ultraTabPageControl1.Controls.Add(this.label4);
            this.ultraTabPageControl1.Controls.Add(this.label3);
            this.ultraTabPageControl1.Controls.Add(this.label6);
            this.ultraTabPageControl1.Controls.Add(this.label7);
            this.ultraTabPageControl1.Controls.Add(this.label8);
            this.ultraTabPageControl1.Controls.Add(this.label17);
            this.ultraTabPageControl1.Controls.Add(this.label61);
            this.ultraTabPageControl1.Controls.Add(this.label69);
            this.ultraTabPageControl1.Controls.Add(this.label67);
            this.ultraTabPageControl1.Controls.Add(this.label66);
            this.ultraTabPageControl1.Controls.Add(this.label64);
            this.ultraTabPageControl1.Controls.Add(this.label62);
            this.ultraTabPageControl1.Controls.Add(this.label38);
            this.ultraTabPageControl1.Controls.Add(this.itineraryTasks1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(2, 21);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(900, 773);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(376, 341);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 13);
            this.label12.TabIndex = 147;
            this.label12.Text = "Agent Contact";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbAgentContact
            // 
            this.cmbAgentContact.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbAgentContact.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAgentContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAgentContact.FormattingEnabled = true;
            this.cmbAgentContact.Location = new System.Drawing.Point(459, 337);
            this.cmbAgentContact.Name = "cmbAgentContact";
            this.cmbAgentContact.Size = new System.Drawing.Size(250, 21);
            this.cmbAgentContact.TabIndex = 146;
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbCurrency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Location = new System.Drawing.Point(459, 416);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(250, 21);
            this.cmbCurrency.TabIndex = 144;
            this.toolTip1.SetToolTip(this.cmbCurrency, "The currency to convert all prices to");
            // 
            // lblCurrency
            // 
            this.lblCurrency.AutoSize = true;
            this.lblCurrency.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrency.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrency.Location = new System.Drawing.Point(377, 419);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(51, 13);
            this.lblCurrency.TabIndex = 145;
            this.lblCurrency.Text = "Currency";
            this.lblCurrency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.lblCurrency, "The currency to convert all prices to");
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.AutoSize = true;
            this.chkReadOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkReadOnly.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.itineraryBindingSource, "IsReadOnly", true));
            this.chkReadOnly.Location = new System.Drawing.Point(461, 17);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(79, 17);
            this.chkReadOnly.TabIndex = 143;
            this.chkReadOnly.Text = "Read only?";
            this.chkReadOnly.UseVisualStyleBackColor = true;
            // 
            // itineraryBindingSource
            // 
            this.itineraryBindingSource.DataMember = "Itinerary";
            this.itineraryBindingSource.DataSource = this.itinerarySet;
            this.itineraryBindingSource.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.ValidationHandler);
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "DisplayName", true));
            this.txtDisplayName.Location = new System.Drawing.Point(97, 42);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(250, 21);
            this.txtDisplayName.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtDisplayName, "This overrides the Itinerary name so that you can show a \'friendly\' name to the c" +
        "lients (bookings, vouchers, etc)");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(12, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 141;
            this.label9.Text = "Display Name";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtArriveDate
            // 
            this.txtArriveDate.CustomFormat = "dd MMM yyyy,  HH:mm - dddd";
            this.txtArriveDate.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.itineraryBindingSource, "ArriveDate", true));
            this.txtArriveDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.txtArriveDate.Location = new System.Drawing.Point(64, 108);
            this.txtArriveDate.Name = "txtArriveDate";
            this.txtArriveDate.NullValue = "";
            this.txtArriveDate.Size = new System.Drawing.Size(283, 21);
            this.txtArriveDate.TabIndex = 3;
            this.txtArriveDate.Value = new System.DateTime(2007, 7, 26, 13, 48, 29, 843);
            this.txtArriveDate.CloseUp += new System.EventHandler(this.txtArriveDate_CloseUp);
            this.txtArriveDate.Validating += new System.ComponentModel.CancelEventHandler(this.txtArriveDate_Validating);
            // 
            // txtDepartDate
            // 
            this.txtDepartDate.CustomFormat = "dd MMM yyyy,  HH:mm - dddd";
            this.txtDepartDate.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.itineraryBindingSource, "DepartDate", true));
            this.txtDepartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.txtDepartDate.Location = new System.Drawing.Point(426, 108);
            this.txtDepartDate.Name = "txtDepartDate";
            this.txtDepartDate.NullValue = "";
            this.txtDepartDate.Size = new System.Drawing.Size(283, 21);
            this.txtDepartDate.TabIndex = 7;
            this.txtDepartDate.Value = new System.DateTime(2007, 7, 26, 13, 47, 7, 125);
            this.txtDepartDate.CloseUp += new System.EventHandler(this.txtDepartDate_CloseUp);
            this.txtDepartDate.Enter += new System.EventHandler(this.txtDepartDate_Enter);
            // 
            // cmbDepartCity
            // 
            this.cmbDepartCity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbDepartCity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDepartCity.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "DepartCityID", true));
            this.cmbDepartCity.DataSource = this.cityBindingSource2;
            this.cmbDepartCity.DisplayMember = "CityName";
            this.cmbDepartCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartCity.FormattingEnabled = true;
            this.cmbDepartCity.Location = new System.Drawing.Point(426, 134);
            this.cmbDepartCity.Name = "cmbDepartCity";
            this.cmbDepartCity.Size = new System.Drawing.Size(283, 21);
            this.cmbDepartCity.TabIndex = 8;
            this.cmbDepartCity.ValueMember = "CityID";
            // 
            // cityBindingSource2
            // 
            this.cityBindingSource2.DataMember = "City";
            this.cityBindingSource2.DataSource = this.toolSet;
            // 
            // toolSet
            // 
            this.toolSet.DataSetName = "ToolSet";
            this.toolSet.Locale = new System.Globalization.CultureInfo("en-US");
            this.toolSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtAddedBy
            // 
            this.txtAddedBy.Location = new System.Drawing.Point(459, 453);
            this.txtAddedBy.Name = "txtAddedBy";
            this.txtAddedBy.ReadOnly = true;
            this.txtAddedBy.Size = new System.Drawing.Size(250, 21);
            this.txtAddedBy.TabIndex = 15;
            // 
            // cmbBranch
            // 
            this.cmbBranch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbBranch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBranch.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "BranchID", true));
            this.cmbBranch.DataSource = this.branchBindingSource;
            this.cmbBranch.DisplayMember = "BranchName";
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(459, 531);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(250, 21);
            this.cmbBranch.TabIndex = 17;
            this.cmbBranch.ValueMember = "BranchID";
            // 
            // branchBindingSource
            // 
            this.branchBindingSource.DataMember = "Branch";
            this.branchBindingSource.DataSource = this.toolSet;
            // 
            // cmbDepartment
            // 
            this.cmbDepartment.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbDepartment.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDepartment.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "DepartmentID", true));
            this.cmbDepartment.DataSource = this.departmentBindingSource;
            this.cmbDepartment.DisplayMember = "DepartmentName";
            this.cmbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartment.FormattingEnabled = true;
            this.cmbDepartment.Location = new System.Drawing.Point(459, 505);
            this.cmbDepartment.Name = "cmbDepartment";
            this.cmbDepartment.Size = new System.Drawing.Size(250, 21);
            this.cmbDepartment.TabIndex = 16;
            this.cmbDepartment.ValueMember = "DepartmentID";
            // 
            // departmentBindingSource
            // 
            this.departmentBindingSource.DataMember = "Department";
            this.departmentBindingSource.DataSource = this.toolSet;
            // 
            // cmbAssignedTo
            // 
            this.cmbAssignedTo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbAssignedTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAssignedTo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "AssignedTo", true));
            this.cmbAssignedTo.DataSource = this.userBindingSource;
            this.cmbAssignedTo.DisplayMember = "UserName";
            this.cmbAssignedTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAssignedTo.FormattingEnabled = true;
            this.cmbAssignedTo.Location = new System.Drawing.Point(459, 479);
            this.cmbAssignedTo.Name = "cmbAssignedTo";
            this.cmbAssignedTo.Size = new System.Drawing.Size(250, 21);
            this.cmbAssignedTo.TabIndex = 15;
            this.cmbAssignedTo.ValueMember = "UserID";
            // 
            // userBindingSource
            // 
            this.userBindingSource.DataMember = "User";
            this.userBindingSource.DataSource = this.toolSet;
            // 
            // cmbSource
            // 
            this.cmbSource.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbSource.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSource.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "ItinerarySourceID", true));
            this.cmbSource.DataSource = this.itinerarySourceBindingSource;
            this.cmbSource.DisplayMember = "ItinerarySourceName";
            this.cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSource.FormattingEnabled = true;
            this.cmbSource.Location = new System.Drawing.Point(459, 389);
            this.cmbSource.Name = "cmbSource";
            this.cmbSource.Size = new System.Drawing.Size(250, 21);
            this.cmbSource.TabIndex = 14;
            this.cmbSource.ValueMember = "ItinerarySourceID";
            // 
            // itinerarySourceBindingSource
            // 
            this.itinerarySourceBindingSource.DataMember = "ItinerarySource";
            this.itinerarySourceBindingSource.DataSource = this.toolSet;
            // 
            // cmbStatus
            // 
            this.cmbStatus.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbStatus.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbStatus.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "ItineraryStatusID", true));
            this.cmbStatus.DataSource = this.itineraryStatusBindingSource;
            this.cmbStatus.DisplayMember = "ItineraryStatusName";
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(459, 363);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(250, 21);
            this.cmbStatus.TabIndex = 13;
            this.cmbStatus.ValueMember = "ItineraryStatusID";
            // 
            // itineraryStatusBindingSource
            // 
            this.itineraryStatusBindingSource.DataMember = "ItineraryStatus";
            this.itineraryStatusBindingSource.DataSource = this.toolSet;
            // 
            // cmbAgent
            // 
            this.cmbAgent.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbAgent.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAgent.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "AgentID", true));
            this.cmbAgent.DataSource = this.agentBindingSource;
            this.cmbAgent.DisplayMember = "AgentNameParentName";
            this.cmbAgent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAgent.FormattingEnabled = true;
            this.cmbAgent.Location = new System.Drawing.Point(459, 311);
            this.cmbAgent.Name = "cmbAgent";
            this.cmbAgent.Size = new System.Drawing.Size(250, 21);
            this.cmbAgent.TabIndex = 12;
            this.cmbAgent.ValueMember = "AgentID";
            // 
            // agentBindingSource
            // 
            this.agentBindingSource.DataMember = "Agent";
            this.agentBindingSource.DataSource = this.toolSet;
            // 
            // txtComments
            // 
            this.txtComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtComments.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "Comments", true));
            this.txtComments.Location = new System.Drawing.Point(7, 302);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(340, 528);
            this.txtComments.TabIndex = 11;
            // 
            // txtDepartNote
            // 
            this.txtDepartNote.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "DepartNote", true));
            this.txtDepartNote.Location = new System.Drawing.Point(426, 186);
            this.txtDepartNote.Multiline = true;
            this.txtDepartNote.Name = "txtDepartNote";
            this.txtDepartNote.Size = new System.Drawing.Size(283, 87);
            this.txtDepartNote.TabIndex = 10;
            // 
            // txtArriveNote
            // 
            this.txtArriveNote.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "ArriveNote", true));
            this.txtArriveNote.Location = new System.Drawing.Point(64, 186);
            this.txtArriveNote.Multiline = true;
            this.txtArriveNote.Name = "txtArriveNote";
            this.txtArriveNote.Size = new System.Drawing.Size(283, 87);
            this.txtArriveNote.TabIndex = 6;
            // 
            // cmbArriveCity
            // 
            this.cmbArriveCity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbArriveCity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbArriveCity.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "ArriveCityID", true));
            this.cmbArriveCity.DataSource = this.cityBindingSource1;
            this.cmbArriveCity.DisplayMember = "CityName";
            this.cmbArriveCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArriveCity.FormattingEnabled = true;
            this.cmbArriveCity.Location = new System.Drawing.Point(64, 134);
            this.cmbArriveCity.Name = "cmbArriveCity";
            this.cmbArriveCity.Size = new System.Drawing.Size(283, 21);
            this.cmbArriveCity.TabIndex = 4;
            this.cmbArriveCity.ValueMember = "CityID";
            // 
            // cityBindingSource1
            // 
            this.cityBindingSource1.DataMember = "City";
            this.cityBindingSource1.DataSource = this.toolSet;
            // 
            // txtDepartFlight
            // 
            this.txtDepartFlight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "DepartFlight", true));
            this.txtDepartFlight.Location = new System.Drawing.Point(426, 160);
            this.txtDepartFlight.Name = "txtDepartFlight";
            this.txtDepartFlight.Size = new System.Drawing.Size(283, 21);
            this.txtDepartFlight.TabIndex = 9;
            // 
            // txtArriveFlight
            // 
            this.txtArriveFlight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "ArriveFlight", true));
            this.txtArriveFlight.Location = new System.Drawing.Point(64, 160);
            this.txtArriveFlight.Name = "txtArriveFlight";
            this.txtArriveFlight.Size = new System.Drawing.Size(283, 21);
            this.txtArriveFlight.TabIndex = 5;
            // 
            // txtCustomId
            // 
            this.txtCustomId.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "CustomCode", true));
            this.txtCustomId.Location = new System.Drawing.Point(633, 42);
            this.txtCustomId.Name = "txtCustomId";
            this.txtCustomId.Size = new System.Drawing.Size(76, 21);
            this.txtCustomId.TabIndex = 140;
            // 
            // txtItineraryId
            // 
            this.txtItineraryId.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "ItineraryID", true));
            this.txtItineraryId.Location = new System.Drawing.Point(633, 15);
            this.txtItineraryId.Name = "txtItineraryId";
            this.txtItineraryId.ReadOnly = true;
            this.txtItineraryId.Size = new System.Drawing.Size(76, 21);
            this.txtItineraryId.TabIndex = 139;
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.itineraryBindingSource, "IsRecordActive", true));
            this.chkActive.Location = new System.Drawing.Point(376, 17);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(75, 17);
            this.chkActive.TabIndex = 138;
            this.chkActive.Text = "Is active ?";
            this.chkActive.UseVisualStyleBackColor = true;
            this.chkActive.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // cmbOrgin
            // 
            this.cmbOrgin.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbOrgin.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbOrgin.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.itineraryBindingSource, "CountryID", true));
            this.cmbOrgin.DataSource = this.countryBindingSource;
            this.cmbOrgin.DisplayMember = "CountryName";
            this.cmbOrgin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrgin.FormattingEnabled = true;
            this.cmbOrgin.Location = new System.Drawing.Point(418, 42);
            this.cmbOrgin.Name = "cmbOrgin";
            this.cmbOrgin.Size = new System.Drawing.Size(122, 21);
            this.cmbOrgin.TabIndex = 2;
            this.cmbOrgin.ValueMember = "CountryID";
            // 
            // countryBindingSource
            // 
            this.countryBindingSource.DataMember = "Country";
            this.countryBindingSource.DataSource = this.toolSet;
            // 
            // txtItineraryName
            // 
            this.txtItineraryName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.itineraryBindingSource, "ItineraryName", true));
            this.txtItineraryName.Location = new System.Drawing.Point(97, 15);
            this.txtItineraryName.Name = "txtItineraryName";
            this.txtItineraryName.Size = new System.Drawing.Size(250, 21);
            this.txtItineraryName.TabIndex = 1;
            // 
            // headerLabel4
            // 
            this.headerLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel4.Location = new System.Drawing.Point(372, 283);
            this.headerLabel4.Name = "headerLabel4";
            this.headerLabel4.Size = new System.Drawing.Size(424, 16);
            this.headerLabel4.TabIndex = 135;
            this.headerLabel4.TextHAlign = Infragistics.Win.HAlign.Default;
            this.headerLabel4.TextString = "Additional";
            this.headerLabel4.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // headerLabel3
            // 
            this.headerLabel3.Location = new System.Drawing.Point(7, 283);
            this.headerLabel3.Name = "headerLabel3";
            this.headerLabel3.Size = new System.Drawing.Size(340, 16);
            this.headerLabel3.TabIndex = 134;
            this.headerLabel3.TextHAlign = Infragistics.Win.HAlign.Default;
            this.headerLabel3.TextString = "Comments";
            this.headerLabel3.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // headerLabel2
            // 
            this.headerLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel2.Location = new System.Drawing.Point(372, 79);
            this.headerLabel2.Name = "headerLabel2";
            this.headerLabel2.Size = new System.Drawing.Size(424, 16);
            this.headerLabel2.TabIndex = 133;
            this.headerLabel2.TextHAlign = Infragistics.Win.HAlign.Default;
            this.headerLabel2.TextString = "Itinerary End";
            this.headerLabel2.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // headerLabel1
            // 
            this.headerLabel1.Location = new System.Drawing.Point(7, 79);
            this.headerLabel1.Name = "headerLabel1";
            this.headerLabel1.Size = new System.Drawing.Size(340, 16);
            this.headerLabel1.TabIndex = 132;
            this.headerLabel1.TextHAlign = Infragistics.Win.HAlign.Default;
            this.headerLabel1.TextString = "Itinerary Start";
            this.headerLabel1.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Location = new System.Drawing.Point(377, 137);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(26, 13);
            this.label19.TabIndex = 58;
            this.label19.Text = "City";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.Location = new System.Drawing.Point(12, 137);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(26, 13);
            this.label16.TabIndex = 56;
            this.label16.Text = "City";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.BackColor = System.Drawing.Color.Transparent;
            this.label63.Location = new System.Drawing.Point(377, 45);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(35, 13);
            this.label63.TabIndex = 29;
            this.label63.Text = "Origin";
            this.label63.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(570, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Custom ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Itinerary Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(12, 186);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 32);
            this.label5.TabIndex = 8;
            this.label5.Text = "Note";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(13, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Flight";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Date";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(377, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 32);
            this.label6.TabIndex = 8;
            this.label6.Text = "Note";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(377, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Flight";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(377, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Date";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(570, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 13);
            this.label17.TabIndex = 102;
            this.label17.Text = "Itinerary ID";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.BackColor = System.Drawing.Color.Transparent;
            this.label61.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.Location = new System.Drawing.Point(377, 314);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(36, 13);
            this.label61.TabIndex = 50;
            this.label61.Text = "Agent";
            this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.BackColor = System.Drawing.Color.Transparent;
            this.label69.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label69.Location = new System.Drawing.Point(377, 456);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(60, 13);
            this.label69.TabIndex = 47;
            this.label69.Text = "Entered by";
            this.label69.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.BackColor = System.Drawing.Color.Transparent;
            this.label67.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label67.Location = new System.Drawing.Point(377, 534);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(40, 13);
            this.label67.TabIndex = 37;
            this.label67.Text = "Branch";
            this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.BackColor = System.Drawing.Color.Transparent;
            this.label66.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label66.Location = new System.Drawing.Point(377, 508);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(64, 13);
            this.label66.TabIndex = 35;
            this.label66.Text = "Department";
            this.label66.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.BackColor = System.Drawing.Color.Transparent;
            this.label64.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label64.Location = new System.Drawing.Point(377, 482);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(63, 13);
            this.label64.TabIndex = 31;
            this.label64.Text = "Assigned to";
            this.label64.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.BackColor = System.Drawing.Color.Transparent;
            this.label62.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label62.Location = new System.Drawing.Point(377, 392);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(40, 13);
            this.label62.TabIndex = 27;
            this.label62.Text = "Source";
            this.label62.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.BackColor = System.Drawing.Color.Transparent;
            this.label38.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(377, 366);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(38, 13);
            this.label38.TabIndex = 6;
            this.label38.Text = "Status";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ultraTabPageControl18
            // 
            this.ultraTabPageControl18.AutoScroll = true;
            this.ultraTabPageControl18.Controls.Add(this.tabControlBookings);
            this.ultraTabPageControl18.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl18.Name = "ultraTabPageControl18";
            this.ultraTabPageControl18.Size = new System.Drawing.Size(900, 773);
            // 
            // tabControlBookings
            // 
            this.tabControlBookings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlBookings.BackColorInternal = System.Drawing.Color.Transparent;
            this.tabControlBookings.Controls.Add(this.ultraTabSharedControlsPage4);
            this.tabControlBookings.Controls.Add(this.ultraTabPageControl11);
            this.tabControlBookings.Controls.Add(this.ultraTabPageControl12);
            this.tabControlBookings.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlBookings.Location = new System.Drawing.Point(1, 3);
            this.tabControlBookings.MinTabWidth = 70;
            this.tabControlBookings.Name = "tabControlBookings";
            appearance15.FontData.BoldAsString = "True";
            this.tabControlBookings.SelectedTabAppearance = appearance15;
            this.tabControlBookings.SharedControlsPage = this.ultraTabSharedControlsPage4;
            this.tabControlBookings.Size = new System.Drawing.Size(898, 769);
            this.tabControlBookings.TabIndex = 3;
            ultraTab9.Key = "List";
            ultraTab9.TabPage = this.ultraTabPageControl12;
            ultraTab9.Text = "List";
            ultraTab1.Key = "Quote";
            ultraTab1.TabPage = this.ultraTabPageControl11;
            ultraTab1.Text = "Quote";
            this.tabControlBookings.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab9,
            ultraTab1});
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(894, 743);
            // 
            // ultraTabPageControl19
            // 
            this.ultraTabPageControl19.AutoScroll = true;
            this.ultraTabPageControl19.Controls.Add(this.clientEditor);
            this.ultraTabPageControl19.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl19.Name = "ultraTabPageControl19";
            this.ultraTabPageControl19.Size = new System.Drawing.Size(900, 773);
            // 
            // clientEditor
            // 
            this.clientEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clientEditor.BackColor = System.Drawing.Color.WhiteSmoke;
            this.clientEditor.EnableGroups = true;
            this.clientEditor.Location = new System.Drawing.Point(0, 3);
            this.clientEditor.Name = "clientEditor";
            this.clientEditor.Size = new System.Drawing.Size(899, 770);
            this.clientEditor.TabIndex = 0;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.tabControlAdditional);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(900, 773);
            // 
            // tabControlAdditional
            // 
            this.tabControlAdditional.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlAdditional.BackColorInternal = System.Drawing.Color.Transparent;
            this.tabControlAdditional.Controls.Add(this.ultraTabSharedControlsPage2);
            this.tabControlAdditional.Controls.Add(this.ultraTabPageControl3);
            this.tabControlAdditional.Controls.Add(this.ultraTabPageControl4);
            this.tabControlAdditional.Controls.Add(this.ultraTabPageControl6);
            this.tabControlAdditional.Controls.Add(this.tabAccounting);
            this.tabControlAdditional.Controls.Add(this.ultraTabPageControl8);
            this.tabControlAdditional.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlAdditional.Location = new System.Drawing.Point(0, 3);
            this.tabControlAdditional.MinTabWidth = 70;
            this.tabControlAdditional.Name = "tabControlAdditional";
            appearance16.FontData.BoldAsString = "True";
            this.tabControlAdditional.SelectedTabAppearance = appearance16;
            this.tabControlAdditional.SharedControlsPage = this.ultraTabSharedControlsPage2;
            this.tabControlAdditional.Size = new System.Drawing.Size(902, 769);
            this.tabControlAdditional.TabIndex = 0;
            ultraTab12.Key = "NewReports";
            ultraTab12.TabPage = this.ultraTabPageControl8;
            ultraTab12.Text = "Reports";
            ultraTab13.Key = "Reports";
            ultraTab13.TabPage = this.ultraTabPageControl6;
            ultraTab13.Text = "Reports (*old)";
            ultraTab2.Key = "Publishing";
            ultraTab2.TabPage = this.ultraTabPageControl3;
            ultraTab2.Text = "Publishing";
            ultraTab3.Key = "Accounting";
            ultraTab3.TabPage = this.tabAccounting;
            ultraTab3.Text = "Accounting";
            ultraTab4.Key = "Messages";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "Messages";
            this.tabControlAdditional.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab12,
            ultraTab13,
            ultraTab2,
            ultraTab3,
            ultraTab4});
            this.tabControlAdditional.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabControlAdditional_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(898, 743);
            // 
            // ultraTabPageControl7
            // 
            this.ultraTabPageControl7.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl7.Name = "ultraTabPageControl7";
            this.ultraTabPageControl7.Size = new System.Drawing.Size(715, 467);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(715, 467);
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance14.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            this.tabControl_Main.ClientAreaAppearance = appearance14;
            this.tabControl_Main.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabControl_Main.Controls.Add(this.ultraTabPageControl1);
            this.tabControl_Main.Controls.Add(this.ultraTabPageControl18);
            this.tabControl_Main.Controls.Add(this.ultraTabPageControl19);
            this.tabControl_Main.Controls.Add(this.ultraTabPageControl2);
            this.tabControl_Main.Location = new System.Drawing.Point(4, 4);
            this.tabControl_Main.MinTabWidth = 70;
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.ScrollButtons = Infragistics.Win.UltraWinTabs.TabScrollButtons.None;
            this.tabControl_Main.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabControl_Main.ShowTabListButton = Infragistics.Win.DefaultableBoolean.False;
            this.tabControl_Main.Size = new System.Drawing.Size(904, 796);
            this.tabControl_Main.TabIndex = 0;
            this.tabControl_Main.TabLayoutStyle = Infragistics.Win.UltraWinTabs.TabLayoutStyle.SingleRowFixed;
            ultraTab5.Key = "Itinerary";
            ultraTab5.TabPage = this.ultraTabPageControl1;
            ultraTab5.Text = "Itinerary";
            ultraTab6.Key = "Bookings";
            ultraTab6.TabPage = this.ultraTabPageControl18;
            ultraTab6.Text = "Bookings";
            ultraTab7.Key = "Clients";
            ultraTab7.TabPage = this.ultraTabPageControl19;
            ultraTab7.Text = "Clients";
            ultraTab8.Key = "Additional";
            ultraTab8.TabPage = this.ultraTabPageControl2;
            ultraTab8.Text = "Additional";
            this.tabControl_Main.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab5,
            ultraTab6,
            ultraTab7,
            ultraTab8});
            this.tabControl_Main.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            this.tabControl_Main.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabControl_Main_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(900, 773);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel3.Controls.Add(this.pnlMain);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 89);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(910, 806);
            this.panel3.TabIndex = 6;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlMain.Controls.Add(this.tabControl_Main);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(2);
            this.pnlMain.Size = new System.Drawing.Size(910, 806);
            this.pnlMain.TabIndex = 1;
            // 
            // itinerarySetLoaderThread
            // 
            this.itinerarySetLoaderThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.itinerarySetLoaderThread_DoWork);
            this.itinerarySetLoaderThread.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.itinerarySetLoaderThread_ProgressChanged);
            this.itinerarySetLoaderThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.itinerarySetLoaderThread_RunWorkerCompleted);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 40);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(910, 24);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.menuSave,
            this.menuRefresh,
            this.menuClose});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator2.MergeIndex = 1;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(135, 6);
            // 
            // menuSave
            // 
            this.menuSave.Image = ((System.Drawing.Image)(resources.GetObject("menuSave.Image")));
            this.menuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuSave.MergeIndex = 2;
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(138, 22);
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuRefresh
            // 
            this.menuRefresh.Image = global::TourWriter.Properties.Resources.Refresh;
            this.menuRefresh.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuRefresh.MergeIndex = 3;
            this.menuRefresh.Name = "menuRefresh";
            this.menuRefresh.Size = new System.Drawing.Size(138, 22);
            this.menuRefresh.Text = "&Refresh";
            this.menuRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
            // 
            // menuClose
            // 
            this.menuClose.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuClose.MergeIndex = 4;
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(138, 22);
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDebugItinerary,
            this.toolStripSeparator1});
            this.debugToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // menuDebugItinerary
            // 
            this.menuDebugItinerary.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuDebugItinerary.MergeIndex = 0;
            this.menuDebugItinerary.Name = "menuDebugItinerary";
            this.menuDebugItinerary.Size = new System.Drawing.Size(143, 22);
            this.menuDebugItinerary.Text = "Itinerary data";
            this.menuDebugItinerary.Click += new System.EventHandler(this.menuDebugItinerary_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator1.MergeIndex = 1;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp});
            this.helpToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuHelp
            // 
            this.menuHelp.Image = ((System.Drawing.Image)(resources.GetObject("menuHelp.Image")));
            this.menuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuHelp.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuHelp.MergeIndex = 1;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(176, 22);
            this.menuHelp.Text = "Help for this screen";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.toolSave,
            this.toolRefresh,
            this.toolSaveClose,
            this.toolHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 64);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(910, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolSave
            // 
            this.toolSave.Image = ((System.Drawing.Image)(resources.GetObject("toolSave.Image")));
            this.toolSave.Name = "toolSave";
            this.toolSave.Size = new System.Drawing.Size(51, 22);
            this.toolSave.Text = "&Save";
            this.toolSave.ToolTipText = "Save changes (CTRL+S)";
            this.toolSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // toolRefresh
            // 
            this.toolRefresh.Image = global::TourWriter.Properties.Resources.Refresh;
            this.toolRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRefresh.Name = "toolRefresh";
            this.toolRefresh.Size = new System.Drawing.Size(66, 22);
            this.toolRefresh.Text = "&Refresh";
            this.toolRefresh.ToolTipText = "Refresh data";
            this.toolRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
            // 
            // toolSaveClose
            // 
            this.toolSaveClose.Image = ((System.Drawing.Image)(resources.GetObject("toolSaveClose.Image")));
            this.toolSaveClose.Name = "toolSaveClose";
            this.toolSaveClose.Size = new System.Drawing.Size(96, 22);
            this.toolSaveClose.Text = "Save && &Close";
            this.toolSaveClose.ToolTipText = "Save and Close Itinerary";
            this.toolSaveClose.Click += new System.EventHandler(this.menuSaveClose_Click);
            // 
            // toolHelp
            // 
            this.toolHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolHelp.Image")));
            this.toolHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolHelp.Name = "toolHelp";
            this.toolHelp.Size = new System.Drawing.Size(52, 22);
            this.toolHelp.Text = "Help";
            this.toolHelp.ToolTipText = "Help for this screen";
            this.toolHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // itineraryTasks1
            // 
            this.itineraryTasks1.AutoScroll = true;
            this.itineraryTasks1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.itineraryTasks1.Location = new System.Drawing.Point(379, 550);
            this.itineraryTasks1.Name = "itineraryTasks1";
            this.itineraryTasks1.Size = new System.Drawing.Size(418, 286);
            this.itineraryTasks1.TabIndex = 148;
            // 
            // ItineraryMain
            // 
            this.AllowDrop = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(910, 895);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderVisible = true;
            this.MainToolStrip = this.toolStrip1;
            this.Name = "ItineraryMain";
            this.Text = "Itinerary Editor";
            this.Load += new System.EventHandler(this.ItineraryLoad);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            this.Controls.SetChildIndex(this.menuStrip1, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.ultraTabPageControl12.ResumeLayout(false);
            this.ultraTabPageControl11.ResumeLayout(false);
            this.ultraTabPageControl8.ResumeLayout(false);
            this.ultraTabPageControl6.ResumeLayout(false);
            this.ultraTabPageControl6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpReports.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.optReport)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.tabAccounting.ResumeLayout(false);
            this.ultraTabPageControl4.ResumeLayout(false);
            this.ultraTabPageControl4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itinerarySet)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.branchBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.departmentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itinerarySourceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countryBindingSource)).EndInit();
            this.ultraTabPageControl18.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControlBookings)).EndInit();
            this.tabControlBookings.ResumeLayout(false);
            this.ultraTabPageControl19.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControlAdditional)).EndInit();
            this.tabControlAdditional.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.currencyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl_Main)).EndInit();
            this.tabControl_Main.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private UltraTabPageControl ultraTabPageControl1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private UltraTabControl tabControl_Main;
        private Label label63;
        private Label label16;
        private Label label19;
        private UltraTabPageControl ultraTabPageControl18;
        private Label label61;
        private Label label69;
        private Label label67;
        private Label label66;
        private Label label64;
        private Label label62;
        private Label label38;
        private Label label17;
        private Panel panel3;
        private Panel pnlMain;
        private UltraTabPageControl ultraTabPageControl19;
        private Bookings.BookingsViewer bookingsViewer;
        public ClientEditor clientEditor;
        private System.ComponentModel.BackgroundWorker itinerarySetLoaderThread;
        private UserControls.HeaderLabel headerLabel1;
        private UserControls.HeaderLabel headerLabel4;
        private UserControls.HeaderLabel headerLabel3;
        private UserControls.HeaderLabel headerLabel2;
        private UltraTabPageControl ultraTabPageControl2;
        private UltraTabControl tabControlAdditional;
        private UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        private UltraTabPageControl ultraTabPageControl3;
        private UltraTabPageControl ultraTabPageControl4;
        private UltraTabPageControl ultraTabPageControl6;
        private UltraTabPageControl ultraTabPageControl7;
        private UltraGrid gridMessages;
        private UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnMessagesAdd;
        private ToolStripButton btnMessageDel;
        private Label label35;
        private PublisherMain publisher1;
        private GroupBox grpReports;
        private Panel pnlReports;
        private UltraOptionSet optReport;
        private Label label10;
        private ErrorProvider errorProvider1;
        private TextBox txtItineraryName;
        private BindingSource itineraryBindingSource;
        private ComboBox cmbOrgin;
        private BindingSource countryBindingSource;
        private CheckBox chkActive;
        private TextBox txtCustomId;
        private TextBox txtItineraryId;
        private ComboBox cmbBranch;
        private ComboBox cmbDepartment;
        private ComboBox cmbAssignedTo;
        private ComboBox cmbSource;
        private ComboBox cmbStatus;
        private ComboBox cmbAgent;
        private TextBox txtComments;
        private TextBox txtDepartNote;
        private TextBox txtArriveNote;
        private ComboBox cmbArriveCity;
        private TextBox txtDepartFlight;
        private TextBox txtArriveFlight;
        private TextBox txtAddedBy;
        private BindingSource cityBindingSource1;
        private ComboBox cmbDepartCity;
        private BindingSource agentBindingSource;
        private BindingSource itineraryStatusBindingSource;
        private BindingSource userBindingSource;
        private BindingSource branchBindingSource;
        private BindingSource departmentBindingSource;
        private Info.ItinerarySet itinerarySet;
        private Info.ToolSet toolSet;
        private TourWriter.UserControls.NullableDateTimePicker txtDepartDate;
        private TourWriter.UserControls.NullableDateTimePicker txtArriveDate;
        private BindingSource cityBindingSource2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem menuSave;
        private ToolStripMenuItem menuRefresh;
        private ToolStripMenuItem menuClose;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem menuDebugItinerary;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem menuHelp;
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolSave;
        private ToolStripButton toolRefresh;
        private ToolStripButton toolSaveClose;
        private ToolStripButton toolHelp;
        private ToolStripSeparator toolStripSeparator1;
        private BindingSource itinerarySourceBindingSource;
        private UltraTabPageControl tabAccounting;
        private UserControls.Accounting.Accounting accounting1;
        private TextBox txtDisplayName;
        private Label label9;
        private ToolTip toolTip1;
        private CheckBox chkReadOnly;
        private UltraTabPageControl ultraTabPageControl5;
        private GroupBox groupBox1;
        private Label label26;
        private Button btnExportToExcel;
        private TextBox txtTemplateFile;
        private Button btnChooseTemplate;
        private TourWriter.Modules.ItineraryModule.Bookings.BookingsQuote bookingsQuote;
        private UltraTabPageControl ultraTabPageControl8;
        private ReportManager reportControl;
        private Label label11;
        private UltraTabControl tabControlBookings;
        private UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private UltraTabPageControl ultraTabPageControl11;
        private UltraTabPageControl ultraTabPageControl12;
        private Label lblCurrency;
        private BindingSource currencyBindingSource;
        private Label label12;
        private ComboBox cmbAgentContact;
        private UserControls.NullableComboBox cmbCurrency;
        private ItineraryTasks itineraryTasks1;
    }
}
