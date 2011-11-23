using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Export.Html;
using DataDynamics.ActiveReports.Export.Pdf;
using DataDynamics.ActiveReports.Export.Rtf;
using DataDynamics.ActiveReports.Export.Text;
using DataDynamics.ActiveReports.Toolbar;
using DataDynamics.ActiveReports.Viewer;
using Infragistics.Win.UltraWinToolbars;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.Reports;
using TourWriter.Reports.Itinerary;
using TourWriter.Services;
using TourWriter.UserControls;
using Resources=TourWriter.Properties.Resources;
using ToolClickEventArgs = Infragistics.Win.UltraWinToolbars.ToolClickEventArgs;

namespace TourWriter.Modules.ReportViewer
{
	public class ReportViewerForm : Form
	{
		#region Designer
		internal ImageList imageList1;
		internal ImageList imageList2;
		public UltraToolbarsManager ToolbarMain;
		internal UltraToolbarsDockArea _MainForm_Toolbars_Dock_Area_Top;
		internal UltraToolbarsDockArea _MainForm_Toolbars_Dock_Area_Bottom;
		internal UltraToolbarsDockArea _MainForm_Toolbars_Dock_Area_Left;
		internal UltraToolbarsDockArea _MainForm_Toolbars_Dock_Area_Right;
        private Viewer viewer;
		private IContainer components;
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportViewerForm));
            Infragistics.Win.UltraWinToolbars.OptionSet optionSet1 = new Infragistics.Win.UltraWinToolbars.OptionSet("Folders");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar1");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Send");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Export");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Help");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("SaveEdit Menu");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Close");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Close");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Save");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Attach");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AboutPage");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("About Tour Writer...");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool6 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Help");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AboutPage");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("DataViewer");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool7 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Debug");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool8 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Send");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Email");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Fax");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool9 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Export");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("HTML");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PDF");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RTF");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Text");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RTF");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("HTML");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PDF");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Email");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Fax");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Text");
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.ToolbarMain = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._MainForm_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MainForm_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MainForm_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MainForm_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.viewer = new DataDynamics.ActiveReports.Viewer.Viewer();
            ((System.ComponentModel.ISupportInitialize)(this.ToolbarMain)).BeginInit();
            this.SuspendLayout();
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
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Silver;
            this.imageList2.Images.SetKeyName(0, "");
            this.imageList2.Images.SetKeyName(1, "");
            this.imageList2.Images.SetKeyName(2, "");
            this.imageList2.Images.SetKeyName(3, "");
            this.imageList2.Images.SetKeyName(4, "");
            this.imageList2.Images.SetKeyName(5, "");
            this.imageList2.Images.SetKeyName(6, "");
            this.imageList2.Images.SetKeyName(7, "");
            this.imageList2.Images.SetKeyName(8, "");
            this.imageList2.Images.SetKeyName(9, "");
            this.imageList2.Images.SetKeyName(10, "");
            this.imageList2.Images.SetKeyName(11, "");
            this.imageList2.Images.SetKeyName(12, "");
            this.imageList2.Images.SetKeyName(13, "");
            this.imageList2.Images.SetKeyName(14, "");
            this.imageList2.Images.SetKeyName(15, "");
            this.imageList2.Images.SetKeyName(16, "");
            this.imageList2.Images.SetKeyName(17, "");
            this.imageList2.Images.SetKeyName(18, "");
            this.imageList2.Images.SetKeyName(19, "");
            this.imageList2.Images.SetKeyName(20, "");
            this.imageList2.Images.SetKeyName(21, "");
            this.imageList2.Images.SetKeyName(22, "");
            this.imageList2.Images.SetKeyName(23, "");
            this.imageList2.Images.SetKeyName(24, "");
            this.imageList2.Images.SetKeyName(25, "");
            this.imageList2.Images.SetKeyName(26, "");
            this.imageList2.Images.SetKeyName(27, "");
            this.imageList2.Images.SetKeyName(28, "");
            this.imageList2.Images.SetKeyName(29, "");
            this.imageList2.Images.SetKeyName(30, "");
            this.imageList2.Images.SetKeyName(31, "");
            this.imageList2.Images.SetKeyName(32, "");
            this.imageList2.Images.SetKeyName(33, "");
            this.imageList2.Images.SetKeyName(34, "");
            this.imageList2.Images.SetKeyName(35, "");
            this.imageList2.Images.SetKeyName(36, "");
            this.imageList2.Images.SetKeyName(37, "");
            this.imageList2.Images.SetKeyName(38, "");
            this.imageList2.Images.SetKeyName(39, "");
            this.imageList2.Images.SetKeyName(40, "");
            this.imageList2.Images.SetKeyName(41, "");
            // 
            // ToolbarMain
            // 
            this.ToolbarMain.DesignerFlags = 1;
            this.ToolbarMain.DockWithinContainer = this;
            this.ToolbarMain.ImageListLarge = this.imageList1;
            this.ToolbarMain.ImageListSmall = this.imageList1;
            this.ToolbarMain.OptionSets.Add(optionSet1);
            this.ToolbarMain.ShowFullMenusDelay = 500;
            this.ToolbarMain.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2003;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.Text = "Main Menu";
            ultraToolbar1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            popupMenuTool2,
            popupMenuTool3,
            popupMenuTool4});
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 1;
            ultraToolbar2.Text = "SaveEdit Menu";
            ultraToolbar2.Visible = false;
            this.ToolbarMain.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2});
            popupMenuTool5.SharedProps.Caption = "&File";
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            popupMenuTool5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1});
            buttonTool2.SharedProps.Caption = "Close";
            appearance1.Image = 17;
            buttonTool3.SharedProps.AppearancesSmall.Appearance = appearance1;
            buttonTool3.SharedProps.Caption = "&Save";
            buttonTool3.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool3.SharedProps.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            appearance2.Image = 16;
            buttonTool4.SharedProps.AppearancesSmall.Appearance = appearance2;
            buttonTool4.SharedProps.Caption = "Attach";
            buttonTool4.SharedProps.ToolTipText = "Attach a file";
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            buttonTool5.SharedProps.AppearancesSmall.Appearance = appearance3;
            buttonTool5.SharedProps.Caption = "Delete";
            buttonTool5.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool6.SharedProps.Caption = "About this page ...";
            buttonTool7.SharedProps.Caption = "About Tour Writer...";
            popupMenuTool6.SharedProps.Caption = "&Help";
            popupMenuTool6.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool8.InstanceProps.IsFirstInGroup = true;
            popupMenuTool6.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8});
            appearance4.Image = 27;
            appearance4.TextHAlign = Infragistics.Win.HAlign.Right;
            buttonTool9.SharedProps.AppearancesSmall.Appearance = appearance4;
            buttonTool9.SharedProps.Caption = "Open Data&Viewer";
            buttonTool9.SharedProps.Shortcut = System.Windows.Forms.Shortcut.F9;
            popupMenuTool7.SharedProps.Caption = "Debug";
            popupMenuTool8.SharedProps.Caption = "Send";
            popupMenuTool8.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool10,
            buttonTool11,
            buttonTool12});
            popupMenuTool9.SharedProps.Caption = "Export";
            popupMenuTool9.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16});
            buttonTool17.SharedProps.Caption = "RTF (Word document)";
            buttonTool18.SharedProps.Caption = "HTML (Web page)";
            buttonTool19.SharedProps.Caption = "PDF (Adobe)";
            appearance5.Image = 30;
            buttonTool20.SharedProps.AppearancesSmall.Appearance = appearance5;
            buttonTool20.SharedProps.Caption = "Email...";
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            buttonTool21.SharedProps.AppearancesSmall.Appearance = appearance6;
            buttonTool21.SharedProps.Caption = "Fax...";
            buttonTool21.SharedProps.Visible = false;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            buttonTool22.SharedProps.AppearancesSmall.Appearance = appearance7;
            buttonTool22.SharedProps.Caption = "Print...";
            buttonTool23.SharedProps.Caption = "Text (Text file)";
            this.ToolbarMain.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool5,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool6,
            buttonTool9,
            popupMenuTool7,
            popupMenuTool8,
            popupMenuTool9,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            buttonTool23});
            this.ToolbarMain.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ToolbarMain_ToolClick);
            // 
            // _MainForm_Toolbars_Dock_Area_Top
            // 
            this._MainForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MainForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._MainForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._MainForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MainForm_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._MainForm_Toolbars_Dock_Area_Top.Name = "_MainForm_Toolbars_Dock_Area_Top";
            this._MainForm_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(892, 27);
            this._MainForm_Toolbars_Dock_Area_Top.ToolbarsManager = this.ToolbarMain;
            // 
            // _MainForm_Toolbars_Dock_Area_Bottom
            // 
            this._MainForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MainForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._MainForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._MainForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MainForm_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 566);
            this._MainForm_Toolbars_Dock_Area_Bottom.Name = "_MainForm_Toolbars_Dock_Area_Bottom";
            this._MainForm_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(892, 0);
            this._MainForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ToolbarMain;
            // 
            // _MainForm_Toolbars_Dock_Area_Left
            // 
            this._MainForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MainForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._MainForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._MainForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MainForm_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 27);
            this._MainForm_Toolbars_Dock_Area_Left.Name = "_MainForm_Toolbars_Dock_Area_Left";
            this._MainForm_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 539);
            this._MainForm_Toolbars_Dock_Area_Left.ToolbarsManager = this.ToolbarMain;
            // 
            // _MainForm_Toolbars_Dock_Area_Right
            // 
            this._MainForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MainForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(190)))), ((int)(((byte)(245)))));
            this._MainForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._MainForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MainForm_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(892, 27);
            this._MainForm_Toolbars_Dock_Area_Right.Name = "_MainForm_Toolbars_Dock_Area_Right";
            this._MainForm_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 539);
            this._MainForm_Toolbars_Dock_Area_Right.ToolbarsManager = this.ToolbarMain;
            // 
            // viewer
            // 
            this.viewer.BackColor = System.Drawing.SystemColors.Control;
            this.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewer.Location = new System.Drawing.Point(0, 27);
            this.viewer.Name = "viewer";
            this.viewer.ReportViewer.CurrentPage = 0;
            this.viewer.ReportViewer.DisplayUnits = DataDynamics.ActiveReports.Viewer.DisplayUnits.Metric;
            this.viewer.ReportViewer.MultiplePageCols = 3;
            this.viewer.ReportViewer.MultiplePageRows = 2;
            //this.viewer.ReportViewer.ViewType = DataDynamics.ActiveReports.Viewer.ViewType.Normal;
            this.viewer.Size = new System.Drawing.Size(892, 539);
            this.viewer.TabIndex = 4;
            this.viewer.TableOfContents.Text = "Contents";
            this.viewer.TableOfContents.Width = 200;
            this.viewer.Toolbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // ReportViewerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(892, 566);
            this.Controls.Add(this.viewer);
            this.Controls.Add(this._MainForm_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._MainForm_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._MainForm_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._MainForm_Toolbars_Dock_Area_Bottom);
            this.Name = "ReportViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TourWriter Report Viewer";
            this.Load += new System.EventHandler(this.ReportViewerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ToolbarMain)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
		#endregion

		private ComboBox cmbLanguage;
		private ActiveReport[] childReports;
		private ItinerarySet itinerarySet;
		
		public float Zoom
		{
			get{ return viewer.ReportViewer.Zoom; }
			set{ viewer.ReportViewer.Zoom = value; }
		}


		public ReportViewerForm(params ActiveReport[] childReports)
		{
			InitializeComponent();
            Icon = Resources.TourWriter16;
			this.childReports = childReports;			
		}

        public ReportViewerForm(ItinerarySet itinerarySet, params ActiveReport[] childReports)
		{
			InitializeComponent();

			this.itinerarySet = itinerarySet;
			this.childReports = childReports;			
		}

        private void ReportViewerForm_Load(object sender, EventArgs e)
		{
			InitialiseLanguageControlAddins();
			BuildReports();			
		}


		private void InitialiseLanguageControlAddins()
		{
			// load available languages for this report
			cmbLanguage = new ComboBox();
			cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbLanguage.DataSource = 
				Localization.GetLanguagesInDictionaryEntryArray(GetReportTemplateFileName(childReports[0].GetType()));
			cmbLanguage.DisplayMember = "Key";
			cmbLanguage.ValueMember = "Value";
			cmbLanguage.SelectedIndexChanged += cmbLanguage_SelectedIndexChanged;

			// add language control to report viewers toolbar
            PlaceHolder ph = new PlaceHolder();
			ph.Enabled = true;
			ph.ToolTip = "Choose the language for the report.";
			ph.Visible = true;
			ph.Id = 777;
			ph.Control = cmbLanguage;
			viewer.Toolbar.Tools.Add(ph);
		}

		private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			BuildReports();
		}


        private static string GetReportLayoutFileName(Type reportType)
        {
            return String.Format("{0}\\Templates\\{1}.rpx",
                                 Cache.ToolSet.AppSettings[0].ExternalFilesPath.TrimEnd('\\'),
                                 reportType);
        }

        private static string GetReportTemplateFileName(Type reportType)
        {
            return String.Format("{0}\\Templates\\{1}.txt",
                                 Cache.ToolSet.AppSettings[0].ExternalFilesPath.TrimEnd('\\'),
                                 reportType);
        }


	    private void BuildReports()
		{
			// process batch of reports into single report for viewing
			ActiveReport batchReport;
			batchReport = new BatchReportBlank();
			batchReport.Run();
			batchReport.Document.Pages.Clear();

			for(int i=0; i<childReports.Length; i++)
			{
				// get custom layout
				ReportLayout(childReports[i]);

				// set localisation
				if(cmbLanguage.SelectedValue != null)
					ReportLocalise(childReports[i], cmbLanguage.SelectedValue.ToString());
				
				// initialise the report
				childReports[i].Run();

				// add the report
				for(int j=0; j<childReports[i].Document.Pages.Count; j++)
					batchReport.Document.Pages.Add(childReports[i].Document.Pages[j]);
			}
			viewer.Document = batchReport.Document;
			Text +=  ": " + (childReports.Length > 0 ? childReports[0].Document.Name : "");
		}

		private void ReportLayout(ActiveReport rpt)
		{
			// Loads a custom layout for the report, if the layout file is found. 
			// Otherwise uses default layout.

            string fileName = GetReportLayoutFileName(rpt.GetType());

			if(File.Exists(fileName))
			{
				try
				{
					rpt.LoadLayout(fileName);
				}
				catch(Exception ex)
				{
					throw new Exception("Error reading from report layout file.", ex);
				}
			}
		}
				
		private static void ReportLocalise(ActiveReport rpt, string culture)
		{
			// Loads localisation settings for a report.

			try
			{
                Localization.Localize(rpt, GetReportTemplateFileName(rpt.GetType()), new CultureInfo(culture));
			}
			catch(ArgumentException ex)
			{
				throw new Exception(String.Format("'{0}' is not a correct language name.", culture), ex);
			}
			catch(Exception ex)
			{
				throw new Exception("Error reading from language file.", ex);
			}
		}


		private void SavePdf()
		{
			string saveToFileName = App.PromptSaveFile("Report.pdf", "Portable Document Format", ".pdf");
			if(saveToFileName != "")
			{
				try
				{
					PdfExport pdf = new PdfExport();
					pdf.Export(viewer.Document, saveToFileName);
                }
                catch (Exception ex)
                {
                    if (ErrorHelper.IsFileAccessError(ex))
                        App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                    else
                        throw;
                }
			}
		}		
		
		private void SaveRtf()
		{
			string saveToFileName = App.PromptSaveFile("Report.rtf", "Rich Text Format", ".rtf");
				
			if(saveToFileName != "")
			{
				try
				{
					RtfExport rtf = new RtfExport();
					rtf.Export(viewer.Document, saveToFileName);
				}
				catch (Exception ex)
                {
                    if (ErrorHelper.IsFileAccessError(ex))
                        App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                    else
                        throw;
				}
			}
		}

		private void SaveHtml()
		{
			string saveToFileName = App.PromptSaveFile("Report.html", "Internet Web Page", ".html");
				
			if(saveToFileName != "")
			{				
				try
				{
					HtmlExport html = new HtmlExport();
					html.Export(viewer.Document, saveToFileName);
                }
                catch (Exception ex)
                {
                    if (ErrorHelper.IsFileAccessError(ex))
                        App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                    else
                        throw;
                }
			}
		}

		private void SaveText()
		{
			string saveToFileName = App.PromptSaveFile("Report.text", "Text file", ".txt");
				
			if(saveToFileName != "")
			{				
				try
				{
					TextExport text = new TextExport();
					text.Export(viewer.Document, saveToFileName);
                }
                catch (Exception ex)
                {
                    if (ErrorHelper.IsFileAccessError(ex))
                        App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                    else
                        throw;
                }
			}
		}


		private void SendEmail()
		{
			// Open the emailer module with an array of reports to email
			ReportEmailBuilder emailBuilder = new ReportEmailBuilder(childReports);
			// Set itinerary set so that ReportEmailBuilder can record the sent
			// email against the itinerary.
			if(itinerarySet != null)
				emailBuilder.ItinerarySet = itinerarySet;

            Wizard wizard = new Wizard(Settings.Default.EmailEditorSize);
			wizard.Text = "TourWriter report emails";
			wizard.Params = emailBuilder;
			wizard.AddPage(new Emailer.TemplateForm());
			wizard.AddPage(new Emailer.EmailForm());
			wizard.AddPage(new Emailer.SendForm());
            wizard.FormClosing += delegate
            {
                Settings.Default.EmailEditorSize = wizard.Size;
                Settings.Default.Save();
            };
            wizard.ShowDialog();
            wizard.Dispose();
		}

		private void SendFax()
		{			
			MessageBox.Show("Fax service not yet implimented.");
			return;

			/*
			// preload
			
			string faxNumber; 
			faxNumber = report.Fields.Contains("FaxTo") ? report.Fields["FaxTo"].Value.ToString() : "";			
 
			FaxForm f = new FaxForm();
			DialogResult result = f.ShowDialog(this);
			faxNumber = f.FaxNumber;
			f.Close();
			if(result == DialogResult.Cancel)
				return;

			try
			{	
				// create file				
				string fileName = Path.Combine(Path.GetTempPath(), "temp.tiff");
				DataDynamics.ActiveReports.Export.Tiff.TiffExport tiff = new DataDynamics.ActiveReports.Export.Tiff.TiffExport();
				tiff.Export(this.reportViewer.Document, fileName);

				// send fax
				FAXCOMLib.FaxServer faxServer = new FAXCOMLib.FaxServerClass(); 
				faxServer.Connect(Environment.MachineName); 
				
				FAXCOMLib.FaxDoc faxDoc = (FAXCOMLib.FaxDoc)faxServer.CreateDocument(fileName); 
				faxDoc.RecipientName = "";
				faxDoc.RecipientCompany = "";
				faxDoc.FaxNumber = faxNumber
				faxDoc.DisplayName = "";
				faxDoc.DiscountSend = 0;
				faxDoc.SendCoverpage = 0;

				int Response = faxDoc.Send(); 
				faxServer.Disconnect();	
			
				faxDoc = null;
				faxServer = null;
			}
			catch (DataDynamics.ActiveReports.ReportException ex)
			{
				MessageBox.Show(this, "Error creating Fax file.\n"+ex.ToString());
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Error sending Fax.\n" + ex.ToString());
			}
			*/
		}

		private void SendPrinter()
		{
			viewer.Document.Print();
		}		
		

//		private ItinerarySet.SupplierLookupRow emailingSupplier;
//		private void Email_OnEmailFileSaved(EmailFileSaved emailFile)
//		{
////			ItinerarySet.SupplierLookupRow supplier = null;
////			if(currentEmailReport.Fields.Contains("SupplierID"))
////				supplier = itinerarySet.SupplierLookup.FindBySupplierID(
////					(int)currentEmailReport.Fields["SupplierID"].Value);
//
//			// record the email in the message table
//			ItinerarySet.MessageRow messageRow = itinerarySet.Message.NewMessageRow();
//			messageRow.MessageType = "Email";
//			messageRow.MessageTo   = emailFile.EmailTo;
//			messageRow.MessageFrom = emailFile.EmailFrom;
//			messageRow.MessageName = emailFile.EmailSubject + (emailingSupplier != null ? " - " + emailingSupplier.SupplierName : "");
//			messageRow.MessageFile = emailFile.FileName;
//			messageRow.AddedOn     = DateTime.Now;
//			messageRow.AddedBy     = TourWriter.Global.Cache.User.UserID;
//			itinerarySet.Message.AddMessageRow(messageRow);
//
//			// join the ItineraryMessage table
//			itinerarySet.ItineraryMessage.AddItineraryMessageRow(
//				itinerarySet.Itinerary[0], messageRow, DateTime.Now, TourWriter.Global.Cache.User.UserID);
//
//			// join the SupplierMessage table
//			if(emailingSupplier != null)
//				itinerarySet.SupplierMessage.AddSupplierMessageRow(
//					messageRow, emailingSupplier.SupplierID, DateTime.Now, TourWriter.Global.Cache.User.UserID);
//		}


		private string GetTextContentFromReport(ActiveReport report)
		{
			string body = "";
			try
			{
				TextExport text = new TextExport();
				MemoryStream stream = new MemoryStream();
				text.Export(report.Document, stream);
				body = Encoding.ASCII.GetString(stream.ToArray());
				stream.Close();	
			}
			catch (Exception ex)
			{
				App.Error(ex);
			}
			return body;
		}
		
		private string GetRtfContentFromReport(ActiveReport report)
		{
			string body = "";
			try
			{
				RtfExport rtf = new RtfExport();
				MemoryStream stream = new MemoryStream();
				rtf.Export(report.Document, stream);
				body = Encoding.ASCII.GetString(stream.ToArray());
				stream.Close();	
			}
			catch (Exception ex)
			{
				App.Error(ex);
			}
			return body;
		}

		private string GetHtmlContentFromReport(ActiveReport report)
		{
			string htmlBody = "";
			try
			{
				HtmlExport html = new HtmlExport();
				MemoryStream stream = new MemoryStream();
				html.Export(report.Document, stream);
				htmlBody = Encoding.ASCII.GetString(stream.ToArray());
				stream.Close();	

				// replace rubbish
				htmlBody = htmlBody.Replace("="+Environment.NewLine, "");
				htmlBody = htmlBody.Replace("3D", "");
				htmlBody = htmlBody.Replace("=0D", "");
				htmlBody = htmlBody.Replace("\r","").Replace("\n","").Replace("\t","").Replace("\\\"","\"");

				// trim
				int startPoint = htmlBody.IndexOf("<html>");
				int endPoint = htmlBody.IndexOf("</html>") + 7;
				htmlBody = htmlBody.Substring(startPoint, endPoint - startPoint);	
				
				// add warning
				string s = "<p>If the text below is unreadable, please refer to attachment.<p>";
				startPoint = htmlBody.IndexOf("<body");
				endPoint = htmlBody.IndexOf(">", startPoint);
				htmlBody = htmlBody.Insert(endPoint+1, s);
			}
			catch (Exception ex)
			{
				App.Error(ex);
			}
			return htmlBody;
		}
		
		private byte[] GetPdfAttachmentContentFromReport(ActiveReport report)
		{
			MemoryStream memStream = new MemoryStream();
	
			PdfExport pdf;
			pdf = new PdfExport();

			pdf.Export(report.Document, memStream);
			return memStream.ToArray();
		}

		private void ToolbarMain_ToolClick(object sender, ToolClickEventArgs e)
		{
			switch(e.Tool.Key)
			{
				case "Email" :					
					this.SendEmail();
					break;
				case "Fax" :
					this.SendFax();
					break;
				case "Print" : 
					this.SendPrinter();
					break;
				case "PDF" :
					this.SavePdf();
					break;
				case "HTML" :
					this.SaveHtml();
					break;
				case "RTF" :
					this.SaveRtf();
					break;
				case "Text" :
					this.SaveText();
					break;
				case "Close" :
					this.Close();
					break;
			}
		}
		
	}
}
