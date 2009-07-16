using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.SupplierModule.ReportControls
{
	/// <summary>
	/// Summary description for WhoUsedSupplier.
	/// </summary>
	public class WhoUsedSupplier : System.Windows.Forms.UserControl
	{
		#region Member vars
		int supplierID;
		string supplierName;
		private ToolSet toolSet
		{
			get
			{
				return Cache.ToolSet;
			}
		}
		
		#endregion

		#region Designer

		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private Infragistics.Win.Misc.UltraButton btnReport;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtFrom;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtTo;
		private System.ComponentModel.Container components = null;
		#endregion

		public WhoUsedSupplier(int supplierID, string supplierName)
		{
			InitializeComponent();
			this.supplierID = supplierID;
			this.supplierName = supplierName;
		}
		
		private void WhoUsedSupplier_Load(object sender, System.EventArgs e)
		{
			this.txtFrom.Value = DateTime.Now.AddYears(-1);
			this.txtTo.Value   = DateTime.Now;
		}
						
		private void btnReport_Click(object sender, System.EventArgs e)
		{
			try
			{
				// get params
				int id = this.supplierID;
				DateTime dateFrom = (DateTime)this.txtFrom.Value;
				DateTime dateTo   = (DateTime)this.txtTo.Value;

				// create report				
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.Supplier.WhoUsed(supplierID, dateFrom, dateTo);	
				report.Document.Name = String.Format("{0} : {1} - {2}",
					this.supplierName, dateFrom.ToShortDateString(), dateTo.ToShortDateString());
				
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
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.txtTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.btnReport = new Infragistics.Win.Misc.UltraButton();
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).BeginInit();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 12);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(320, 28);
			this.label6.TabIndex = 41;
			this.label6.Text = "Report shows where this Supplier has been used in Itineraries.";
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(20, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 66;
			this.label4.Text = "Date to";
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(20, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 65;
			this.label3.Text = "Date from";
			// 
			// txtFrom
			// 
			this.txtFrom.Location = new System.Drawing.Point(104, 56);
			this.txtFrom.Name = "txtFrom";
			this.txtFrom.TabIndex = 63;
			// 
			// txtTo
			// 
			this.txtTo.Location = new System.Drawing.Point(104, 84);
			this.txtTo.Name = "txtTo";
			this.txtTo.TabIndex = 64;
			// 
			// btnReport
			// 
			this.btnReport.Location = new System.Drawing.Point(104, 124);
			this.btnReport.Name = "btnReport";
			this.btnReport.Size = new System.Drawing.Size(112, 23);
			this.btnReport.TabIndex = 69;
			this.btnReport.Text = "Open Report";
			this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
			// 
			// WhoUsedSupplier
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.btnReport);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtFrom);
			this.Controls.Add(this.txtTo);
			this.Controls.Add(this.label6);
			this.Name = "WhoUsedSupplier";
			this.Size = new System.Drawing.Size(348, 184);
			this.Load += new System.EventHandler(this.WhoUsedSupplier_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion		
	}
}
