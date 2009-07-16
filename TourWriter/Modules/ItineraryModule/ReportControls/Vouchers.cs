using System;
using System.Data;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ItineraryModule.ReportControls
{
	/// <summary>
	/// Summary description for SummaryNoPricing.
	/// </summary>
	public class Vouchers : System.Windows.Forms.UserControl
	{
		#region Member vars
		private ItinerarySet itinerarySet;
		private AgentSet agentSet;
		private UserSet userSet;
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
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFooter;
		private System.Windows.Forms.Label label34;
		private TourWriter.UserControls.CheckBoxSet chksetSupplierLines;
        private CheckBox chkIncEmptyBookings;
		private System.ComponentModel.Container components = null;
		#endregion				
		
		public Vouchers(ItinerarySet itinerarySet, AgentSet agentSet, UserSet userSet)
		{
			InitializeComponent();
			
			this.itinerarySet = itinerarySet;
			this.agentSet = agentSet;
			this.userSet = userSet;

		}
		
		private void SummaryNoPricing_Load(object sender, System.EventArgs e)
		{
            CreateBookingList(chkIncEmptyBookings.Checked);
		}
						

		private void OpenReport(string purchaseLineList)
		{				
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// create report
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.Itinerary.Voucher(
					purchaseLineList, itinerarySet, Cache.User, agentSet, toolSet, txtFooter.Text);

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

        private void CreateBookingList(bool includeEmptyBookings)
        {
            DataTable table = new ItinerarySet.PurchaseLineDataTable();
            foreach (ItinerarySet.PurchaseLineRow line in itinerarySet.PurchaseLine)
            {
                if (line.RowState == DataRowState.Deleted)
                    continue;
                
                if (!includeEmptyBookings && line.GetPurchaseItemRows().Length == 0)
                    continue;
                table.Rows.Add(table.NewRow().ItemArray = line.ItemArray);
            }
            // load request status IDs
            chksetSupplierLines.Initialise(table, "PurchaseLineID", "PurchaseLineName", false);

            chksetSupplierLines.CheckAll(true);
        }

		
	    private void btnReport_Click(object sender, System.EventArgs e)
	    {
	        // check input params
	        string purchaseLineList = this.chksetSupplierLines.GetResultAsCsvString();
	        
	        if(purchaseLineList.Trim().Length == 0)
	        {
	            App.ShowWarning("No Supplier Lines are selected.");
	            return;
	        }
	        OpenReport(purchaseLineList);	
	    }
        

	    private void chkIncEmptyBookings_CheckedChanged(object sender, EventArgs e)
        {
            CreateBookingList(chkIncEmptyBookings.Checked);
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
            this.chksetSupplierLines = new TourWriter.UserControls.CheckBoxSet();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFooter = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label34 = new System.Windows.Forms.Label();
            this.chkIncEmptyBookings = new System.Windows.Forms.CheckBox();
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
            // chksetSupplierLines
            // 
            this.chksetSupplierLines.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chksetSupplierLines.Location = new System.Drawing.Point(12, 20);
            this.chksetSupplierLines.Name = "chksetSupplierLines";
            this.chksetSupplierLines.Size = new System.Drawing.Size(200, 204);
            this.chksetSupplierLines.TabIndex = 74;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 16);
            this.label1.TabIndex = 75;
            this.label1.Text = "Supplier Lines";
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
            // chkIncEmptyBookings
            // 
            this.chkIncEmptyBookings.AutoSize = true;
            this.chkIncEmptyBookings.Location = new System.Drawing.Point(244, 20);
            this.chkIncEmptyBookings.Name = "chkIncEmptyBookings";
            this.chkIncEmptyBookings.Size = new System.Drawing.Size(138, 17);
            this.chkIncEmptyBookings.TabIndex = 78;
            this.chkIncEmptyBookings.Text = "Include empty bookings";
            this.chkIncEmptyBookings.UseVisualStyleBackColor = true;
            this.chkIncEmptyBookings.CheckedChanged += new System.EventHandler(this.chkIncEmptyBookings_CheckedChanged);
            // 
            // Vouchers
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.chkIncEmptyBookings);
            this.Controls.Add(this.txtFooter);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.chksetSupplierLines);
            this.Controls.Add(this.label1);
            this.Name = "Vouchers";
            this.Size = new System.Drawing.Size(486, 384);
            this.Load += new System.EventHandler(this.SummaryNoPricing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtFooter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	    }
	    #endregion		
	}
}
