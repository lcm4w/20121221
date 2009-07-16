using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ItineraryModule.ReportControls
{
	/// <summary>
	/// Summary description for ClientNoPricing.
	/// </summary>
	public class ClientNoPricing : System.Windows.Forms.UserControl
	{
		#region Member vars
		private ItinerarySet itinerarySet;
		private AgentSet agentSet;
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
		private System.Windows.Forms.Label label1;
		private TourWriter.UserControls.CheckBoxSet chksetServiceTypes;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFooter;
		private System.Windows.Forms.Label label34;
		private System.ComponentModel.Container components = null;
		#endregion				
		
		public ClientNoPricing(ItinerarySet itinerarySet, AgentSet agentSet)
		{
			InitializeComponent();
			
			this.itinerarySet = itinerarySet;
			this.agentSet = agentSet;
		}
		
		private void ClientNoPricing_Load(object sender, System.EventArgs e)
		{
			// load request status IDs
			this.chksetServiceTypes.Initialise(
				toolSet.ServiceType, "ServiceTypeID", "ServiceTypeName", false);

			// set defaults
			this.chksetServiceTypes.CheckAll(true);
		}
						
		private void btnReport_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// check input params
				string serviceTypesList = this.chksetServiceTypes.GetResultAsCsvString();
				if(serviceTypesList.Trim().Length == 0)
				{
					App.ShowWarning("No Service Types are selected.");
					return;
				}

				// create report
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.Itinerary.ClientNoPricing(
					itinerarySet, toolSet, agentSet, txtFooter.Text, serviceTypesList);
				
				// open in viewer
				Modules.ReportViewer.ReportViewerForm viewer;
				viewer = new Modules.ReportViewer.ReportViewerForm(report);
				
				viewer.Zoom = 1.0f;
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtFooter = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label34 = new System.Windows.Forms.Label();
            this.chksetServiceTypes = new TourWriter.UserControls.CheckBoxSet();
            ((System.ComponentModel.ISupportInitialize)(this.txtFooter)).BeginInit();
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
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 16);
            this.label1.TabIndex = 75;
            this.label1.Text = "Included service types";
            // 
            // txtFooter
            // 
            this.txtFooter.AcceptsReturn = true;
            this.txtFooter.Location = new System.Drawing.Point(8, 244);
            this.txtFooter.Multiline = true;
            this.txtFooter.Name = "txtFooter";
            this.txtFooter.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFooter.Size = new System.Drawing.Size(476, 112);
            this.txtFooter.TabIndex = 77;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(4, 228);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(192, 16);
            this.label34.TabIndex = 76;
            this.label34.Text = "Add note to report:  (not saved)";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chksetServiceTypes
            // 
            this.chksetServiceTypes.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chksetServiceTypes.Location = new System.Drawing.Point(12, 20);
            this.chksetServiceTypes.Name = "chksetServiceTypes";
            this.chksetServiceTypes.Size = new System.Drawing.Size(200, 204);
            this.chksetServiceTypes.TabIndex = 74;
            // 
            // ClientNoPricing
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.txtFooter);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.chksetServiceTypes);
            this.Controls.Add(this.label1);
            this.Name = "ClientNoPricing";
            this.Size = new System.Drawing.Size(486, 384);
            this.Load += new System.EventHandler(this.ClientNoPricing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtFooter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion		
	}
}
