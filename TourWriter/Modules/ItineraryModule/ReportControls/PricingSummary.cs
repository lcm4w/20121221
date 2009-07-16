using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ItineraryModule.ReportControls
{
	/// <summary>
	/// Summary description for PricingSummary.
	/// </summary>
	public class PricingSummary : System.Windows.Forms.UserControl
	{
		#region Member vars
		private ItinerarySet itinerarySet;
		private ToolSet toolSet
		{
			get
			{
				return Cache.ToolSet;
			}
		}
		
		#endregion

		#region Designer

		private Infragistics.Win.Misc.UltraButton btnReport;
		private System.ComponentModel.Container components = null;
		#endregion				
		
		public PricingSummary(ItinerarySet itinerarySet)
		{
			InitializeComponent();
			
			this.itinerarySet = itinerarySet;
		}
		
		private void PricingSummary_Load(object sender, System.EventArgs e)
		{
		}
						
		private void btnReport_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// create report
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.Itinerary.PricingSummary(itinerarySet, toolSet);
				
				// open in viewer
				Modules.ReportViewer.ReportViewerForm viewer;
				viewer = new Modules.ReportViewer.ReportViewerForm(report);
				
				viewer.Zoom = 0.8f;
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
            this.btnReport = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(392, 215);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(92, 23);
            this.btnReport.TabIndex = 69;
            this.btnReport.Text = "Open Report";
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // PricingSummary
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.btnReport);
            this.Name = "PricingSummary";
            this.Size = new System.Drawing.Size(486, 384);
            this.Load += new System.EventHandler(this.PricingSummary_Load);
            this.ResumeLayout(false);

		}
		#endregion		
	}
}
