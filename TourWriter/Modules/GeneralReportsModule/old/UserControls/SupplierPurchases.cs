using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ReportsModule.old.UserControls
{
	/// <summary>
	/// Summary description for ClientLocations.
	/// </summary>
	public class SupplierPurchases : System.Windows.Forms.UserControl
	{
		#region Member vars
		private ToolSet toolSet
		{
			get
			{
				return Cache.ToolSet;
			}
		}
		
		#endregion

		#region Designer

		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private Infragistics.Win.Misc.UltraButton btnReport;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtFrom;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtTo;
		private TourWriter.UserControls.CheckBoxSet chksetRequestStatus;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private TourWriter.UserControls.CheckBoxSet chksetItineraryStatus;
		private System.ComponentModel.Container components = null;
		#endregion

		public SupplierPurchases()
		{
			InitializeComponent();
		}
		
		private void SupplierPurchases_Load(object sender, System.EventArgs e)
		{
			// bind
			this.chksetItineraryStatus.Initialise(
				toolSet.ItineraryStatus, "ItineraryStatusID", "ItineraryStatusName", true);
			this.chksetRequestStatus.Initialise(
				toolSet.RequestStatus, "RequestStatusID", "RequestStatusName", true);			

			// set defaults
			this.txtFrom.Value = DateTime.Now;
			this.txtTo.Value   = DateTime.Now.AddDays(7);
			this.chksetRequestStatus.CheckAll(true);
			this.chksetItineraryStatus.CheckAll(true);
		}
						
		private void btnReport_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// get params
				DateTime dateFrom = (DateTime)this.txtFrom.Value;
				DateTime dateTo = (DateTime)this.txtTo.Value;
				string itineraryStatusList = this.chksetItineraryStatus.GetResultAsCsvString();
				string requestStatusList = this.chksetRequestStatus.GetResultAsCsvString();

				// create report
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.General.SupplierPurchases(
					dateFrom, dateTo, itineraryStatusList, requestStatusList);
				
				// open in viewer
				Modules.ReportViewer.ReportViewerForm viewer;
				viewer = new Modules.ReportViewer.ReportViewerForm(report);
				viewer.Show();
			}
			catch(Exception ex)
			{
				App.Error(ex);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}


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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.txtTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.btnReport = new Infragistics.Win.Misc.UltraButton();
			this.chksetRequestStatus = new TourWriter.UserControls.CheckBoxSet();
			this.label1 = new System.Windows.Forms.Label();
			this.chksetItineraryStatus = new TourWriter.UserControls.CheckBoxSet();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).BeginInit();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(8, 36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 66;
			this.label4.Text = "Date to";
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 65;
			this.label3.Text = "Date from";
			// 
			// txtFrom
			// 
			this.txtFrom.Location = new System.Drawing.Point(88, 4);
			this.txtFrom.Name = "txtFrom";
			this.txtFrom.TabIndex = 63;
			// 
			// txtTo
			// 
			this.txtTo.Location = new System.Drawing.Point(88, 32);
			this.txtTo.Name = "txtTo";
			this.txtTo.TabIndex = 64;
			// 
			// btnReport
			// 
			this.btnReport.Location = new System.Drawing.Point(368, 352);
			this.btnReport.Name = "btnReport";
			this.btnReport.Size = new System.Drawing.Size(92, 23);
			this.btnReport.TabIndex = 69;
			this.btnReport.Text = "Open Report";
			this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
			// 
			// chksetRequestStatus
			// 
			this.chksetRequestStatus.BackColor = System.Drawing.Color.WhiteSmoke;
			this.chksetRequestStatus.Location = new System.Drawing.Point(168, 80);
			this.chksetRequestStatus.Name = "chksetRequestStatus";
			this.chksetRequestStatus.Size = new System.Drawing.Size(144, 144);
			this.chksetRequestStatus.TabIndex = 71;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label1.Location = new System.Drawing.Point(164, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 73;
			this.label1.Text = "Purchase item request status";
			// 
			// chksetItineraryStatus
			// 
			this.chksetItineraryStatus.BackColor = System.Drawing.Color.WhiteSmoke;
			this.chksetItineraryStatus.Location = new System.Drawing.Point(12, 80);
			this.chksetItineraryStatus.Name = "chksetItineraryStatus";
			this.chksetItineraryStatus.Size = new System.Drawing.Size(144, 144);
			this.chksetItineraryStatus.TabIndex = 75;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 16);
			this.label2.TabIndex = 78;
			this.label2.Text = "Itinerary status";
			// 
			// SupplierPurchases
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.btnReport);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.chksetItineraryStatus);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chksetRequestStatus);
			this.Controls.Add(this.txtTo);
			this.Controls.Add(this.txtFrom);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Name = "SupplierPurchases";
			this.Size = new System.Drawing.Size(472, 384);
			this.Load += new System.EventHandler(this.SupplierPurchases_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion		
	}
}
