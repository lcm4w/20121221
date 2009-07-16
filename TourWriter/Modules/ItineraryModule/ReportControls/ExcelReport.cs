using System;
using System.IO;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ItineraryModule.ReportControls
{
	/// <summary>
	/// Summary description for PricingDetailed.
	/// </summary>
	public class ExcelReport : UserControl
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
		
        public ExcelReport(ItinerarySet itinerarySet)
		{
			InitializeComponent();
			
			this.itinerarySet = itinerarySet;
		}
		
        private void LoadReport()
        {
            string templateFile = App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0);
            if (!String.IsNullOrEmpty(templateFile))
            {
                string saveFile = Path.GetTempFileName() + ".xls";

                var exportToExcel = new ExportToExcel(itinerarySet);
                exportToExcel.ExportFinancial(templateFile, saveFile);
            }
        }
				
		private void btnReport_Click(object sender, EventArgs e)
		{
		    LoadReport();
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
            // PricingDetail
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.btnReport);
            this.Name = "PricingDetail";
            this.Size = new System.Drawing.Size(486, 384);
            this.ResumeLayout(false);

		}
		#endregion		
	}
}
