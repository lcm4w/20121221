using System;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Global;

namespace TourWriter.Modules.ReportsModule.old.UserControls
{
	/// <summary>
	/// Summary description for ClientLocations.
	/// </summary>
	public class ItineraryYield : System.Windows.Forms.UserControl
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private TourWriter.UserControls.CheckBoxSet csItineraryStatus;
		private TourWriter.UserControls.CheckBoxSet csAssignedTo;
		private System.Windows.Forms.Label label7;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbGroupBy;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private TourWriter.UserControls.CheckBoxSet csDepartment;
		private TourWriter.UserControls.CheckBoxSet csBranch;
		private System.ComponentModel.Container components = null;
		#endregion

		public ItineraryYield()
		{
			InitializeComponent();
		}
		
		private void ItineraryYield_Load(object sender, System.EventArgs e)
		{
			// bind
			this.csItineraryStatus.Initialise(
				toolSet.ItineraryStatus, "ItineraryStatusID", "ItineraryStatusName", true);
			this.csAssignedTo.Initialise(
				toolSet.User, "UserID", "UserName", true);
			this.csBranch.Initialise(
				toolSet.Branch, "BranchID", "BranchName", true);
			this.csDepartment.Initialise(
				toolSet.Department, "DepartmentID", "DepartmentName", true);

//			this.cmbGroupBy.Items.Add(TourWriter.Reports.General.ItineraryYield.GroupBy.AssignedTo, "AssignedTo");
//			this.cmbGroupBy.Items.Add(TourWriter.Reports.General.ItineraryYield.GroupBy.Department, "Department");
//			this.cmbGroupBy.Items.Add(TourWriter.Reports.General.ItineraryYield.GroupBy.Branch, "Branch");
			
			// set defaults
			this.txtFrom.Value = DateTime.Now;
			this.txtTo.Value   = DateTime.Now.AddMonths(6);
			//this.cmbGroupBy.SelectedIndex = 0;
			this.csItineraryStatus.CheckAll(true);
			this.csAssignedTo.CheckAll(true);
			this.csBranch.CheckAll(true);
			this.csDepartment.CheckAll(true);
		}
						
		private void btnReport_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// get params
				DateTime dateFrom		= (DateTime)this.txtFrom.Value;
				DateTime dateTo			= (DateTime)this.txtTo.Value;
				string statusList		= this.csItineraryStatus.GetResultAsCsvString();
				string userList			= this.csAssignedTo.GetResultAsCsvString();
				string branchList		= this.csBranch.GetResultAsCsvString();
				string departmentList	= this.csDepartment.GetResultAsCsvString();

				TourWriter.Reports.General.ItineraryYield.GroupBy groupBy =
					TourWriter.Reports.General.ItineraryYield.GroupBy.AssignedTo;
					//(TourWriter.Reports.General.ItineraryYield.GroupBy)cmbGroupBy.Value;

				// create report
				DataDynamics.ActiveReports.ActiveReport report;
				report = new TourWriter.Reports.General.ItineraryYield(
					dateFrom, dateTo, statusList, userList, groupBy, branchList, departmentList); 
					
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
			this.csAssignedTo = new TourWriter.UserControls.CheckBoxSet();
			this.label1 = new System.Windows.Forms.Label();
			this.csItineraryStatus = new TourWriter.UserControls.CheckBoxSet();
			this.label2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.cmbGroupBy = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
			this.label5 = new System.Windows.Forms.Label();
			this.csDepartment = new TourWriter.UserControls.CheckBoxSet();
			this.label6 = new System.Windows.Forms.Label();
			this.csBranch = new TourWriter.UserControls.CheckBoxSet();
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbGroupBy)).BeginInit();
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
			// csAssignedTo
			// 
			this.csAssignedTo.BackColor = System.Drawing.Color.WhiteSmoke;
			this.csAssignedTo.Location = new System.Drawing.Point(168, 80);
			this.csAssignedTo.Name = "csAssignedTo";
			this.csAssignedTo.Size = new System.Drawing.Size(144, 140);
			this.csAssignedTo.TabIndex = 71;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label1.Location = new System.Drawing.Point(164, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 16);
			this.label1.TabIndex = 73;
			this.label1.Text = "Assigned To";
			// 
			// csItineraryStatus
			// 
			this.csItineraryStatus.BackColor = System.Drawing.Color.WhiteSmoke;
			this.csItineraryStatus.Location = new System.Drawing.Point(12, 80);
			this.csItineraryStatus.Name = "csItineraryStatus";
			this.csItineraryStatus.Size = new System.Drawing.Size(144, 140);
			this.csItineraryStatus.TabIndex = 75;
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
			// label7
			// 
			this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label7.Location = new System.Drawing.Point(320, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 16);
			this.label7.TabIndex = 79;
			this.label7.Text = "Group by";
			this.label7.Visible = false;
			// 
			// cmbGroupBy
			// 
			this.cmbGroupBy.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
			this.cmbGroupBy.Location = new System.Drawing.Point(316, 36);
			this.cmbGroupBy.Name = "cmbGroupBy";
			this.cmbGroupBy.Size = new System.Drawing.Size(148, 21);
			this.cmbGroupBy.TabIndex = 80;
			this.cmbGroupBy.Visible = false;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label5.Location = new System.Drawing.Point(164, 228);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(148, 16);
			this.label5.TabIndex = 82;
			this.label5.Text = "Department";
			// 
			// csDepartment
			// 
			this.csDepartment.BackColor = System.Drawing.Color.WhiteSmoke;
			this.csDepartment.Location = new System.Drawing.Point(168, 244);
			this.csDepartment.Name = "csDepartment";
			this.csDepartment.Size = new System.Drawing.Size(144, 132);
			this.csDepartment.TabIndex = 81;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(8, 228);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(84, 16);
			this.label6.TabIndex = 84;
			this.label6.Text = "Branch";
			// 
			// csBranch
			// 
			this.csBranch.BackColor = System.Drawing.Color.WhiteSmoke;
			this.csBranch.Location = new System.Drawing.Point(12, 244);
			this.csBranch.Name = "csBranch";
			this.csBranch.Size = new System.Drawing.Size(144, 132);
			this.csBranch.TabIndex = 83;
			// 
			// ItineraryYield
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.csDepartment);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.csBranch);
			this.Controls.Add(this.btnReport);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.csAssignedTo);
			this.Controls.Add(this.txtTo);
			this.Controls.Add(this.txtFrom);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cmbGroupBy);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.csItineraryStatus);
			this.Name = "ItineraryYield";
			this.Size = new System.Drawing.Size(472, 384);
			this.Load += new System.EventHandler(this.ItineraryYield_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtFrom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbGroupBy)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion		
	}
}
