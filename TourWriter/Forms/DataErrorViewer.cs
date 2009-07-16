using System;
using System.Data;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.Forms
{
	/// <summary>
	/// Show data update conflict and concurrency errors.
	/// </summary>
	public class DataErrorViewer : Form
	{	
		#region Designer
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtMsg;
		private Label label1;
		private Infragistics.Win.Misc.UltraButton btnWriteDS;
		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.Misc.UltraButton btnViewDS;
		private Infragistics.Win.UltraWinEditors.UltraOptionSet optRule;
		private Label label2;
		private GroupBox groupBox1;
		private System.ComponentModel.Container components = null;

		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataErrorViewer));
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.txtMsg = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWriteDS = new Infragistics.Win.Misc.UltraButton();
            this.btnViewDS = new Infragistics.Win.Misc.UltraButton();
            this.optRule = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtMsg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optRule)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMsg
            // 
            resources.ApplyResources(this.txtMsg, "txtMsg");
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnWriteDS
            // 
            resources.ApplyResources(this.btnWriteDS, "btnWriteDS");
            this.btnWriteDS.Name = "btnWriteDS";
            this.btnWriteDS.Click += new System.EventHandler(this.btnWriteDS_Click);
            // 
            // btnViewDS
            // 
            resources.ApplyResources(this.btnViewDS, "btnViewDS");
            this.btnViewDS.Name = "btnViewDS";
            this.btnViewDS.Click += new System.EventHandler(this.btnViewDS_Click);
            // 
            // optRule
            // 
            this.optRule.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            valueListItem1.DataValue = "Accept";
            resources.ApplyResources(valueListItem1, "valueListItem1");
            valueListItem2.DataValue = "Reject";
            resources.ApplyResources(valueListItem2, "valueListItem2");
            this.optRule.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.optRule.ItemSpacingVertical = 9;
            resources.ApplyResources(this.optRule, "optRule");
            this.optRule.Name = "optRule";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.optRule);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // DataErrorViewer
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnViewDS);
            this.Controls.Add(this.btnWriteDS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DataErrorViewer";
            this.Load += new System.EventHandler(this.DataErrorViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtMsg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optRule)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion				
		#endregion
		
		private readonly DataSet ds;
		private bool dsProcessed;
				
		public DataErrorViewer(DataSet dataSet)
		{
			ds = dataSet;
			InitializeComponent();
            Icon = Resources.TourWriter16;
			PrepareDataSetErrorMessages();
		}

		private void DataErrorViewer_Load(object sender, EventArgs e)
		{
			if(!ds.HasErrors)
			{
				Close();
				return;
			}
			Visible = true;
			WriteDataSetErrors();	

			// debugging
			btnViewDS.Visible = App.IsDebugMode;
			btnWriteDS.Visible = App.IsDebugMode;
			Height = App.IsDebugMode ? 328 : 296;			
		}				

		internal bool HasErrors
		{
			get
			{
				if(!dsProcessed) PrepareDataSetErrorMessages();
				return ds.HasErrors;
			}
		}
			
		private void PrepareDataSetErrorMessages()
		{						
			foreach(DataTable dt in ds.Tables)
			{
				foreach(DataRow dr in dt.GetErrors())
				{
					if(dr.RowError.StartsWith(App.DataErrorCurrencyViolationText))
					{
						#region CONCURRENCY_VIOLATION

						bool errorColumnsFound = false;

						// find which column(s) caused the problem
						foreach(DataColumn dc in dr.Table.Columns)
						{
							// only set error for non-calculated fields
							if(dc.Expression == "")
							{
								// if either one is null or they are not the same...
								if (!((dr.IsNull(dc, DataRowVersion.Current) || dr.IsNull(dc, DataRowVersion.Original)) ||
									(dr[dc, DataRowVersion.Current].ToString() == dr[dc, DataRowVersion.Original].ToString())))
								{
									// set the row error
									dr.RowError = String.Format(
										"Failed to modify row in table {0}, as it was modified by another user since you opened it.", dr.Table.TableName);
											
									// set the col error
									string origVal = dr[dc, DataRowVersion.Original].ToString();
									dr.SetColumnError(dc, "Column was modified by another user. Their value is: " + origVal);
									
									errorColumnsFound = true;
								}
							}
						}
						if(!errorColumnsFound)
						{
							// no columns found to have CURRENCY_VIOLATION so flood fresh data into datarow
							dr.ClearErrors();							
							dr.RejectChanges();
						}
						#endregion
					}
					else if(dr.RowError.StartsWith(App.DataErrorPKDeleteConflictText))
					{						
						#region PK_DELETE_CONFLICT

                        if (dr.Table.Columns.Contains("IsDeleted")) // set IsDeleted = true
                        {
                            dr.RejectChanges(); // reset
                            dr.ClearErrors();
                            dr["IsDeleted"] = true; // update, this sets rowstate = modified
                        }
                        else // set row error
                        {
                            const string test = ", table '";
                            int start = dr.RowError.IndexOf(", table '") + test.Length;
                            int end = dr.RowError.IndexOf("'", start);
                            string fkTable = dr.RowError.Substring(start, end - start);
                            string pkTable = dr.Table.TableName;

                            dr.RowError = String.Format(
                                "Failed to delete row from table '{0}', as it is used in table '{1}'.", pkTable, fkTable);
                        }
					    #endregion
					}
				}
			}
			dsProcessed = true;
		}
		
		private void WriteDataSetErrors()
		{
			int i = 1;
			foreach(DataTable dt in ds.Tables)
			{
				foreach(DataRow dr in dt.GetErrors())
				{
					txtMsg.Text += String.Format("{0}.  {1}\r\n", i++, dr.RowError);
					foreach(DataColumn dc in dr.GetColumnsInError())
						txtMsg.Text += String.Format("\tColumn {0}: {1}\r\n", dc.ColumnName, dr.GetColumnError(dc));
				}
			}
		}

		private void HandleDataSetRowErrors()
		{	
			foreach(DataTable dt in ds.Tables)
			{
				foreach(DataRow dr in dt.GetErrors())
				{
					if(!dr.HasVersion(DataRowVersion.Current))
					{
						// PK delete conflict, roll back change
						dr.RejectChanges();
						dr.ClearErrors();
					}
					else
					{
					    var hasRowVersionColumn = dr.HasVersion(DataRowVersion.Current) &&
					                               dr.HasVersion(DataRowVersion.Original) &&
					                               dr.Table.Columns.Contains("RowVersion");

						// update RowVersions ready for future updates
                        if (hasRowVersionColumn)
						{
							dr.Table.Columns["RowVersion"].ReadOnly = false;
							dr["RowVersion"] = dr["RowVersion", DataRowVersion.Original];
							dr.Table.Columns["RowVersion"].ReadOnly = true;
							dr.ClearErrors();
						}

						// accept new changes
						if(optRule.Value.ToString() == "Accept")
						{
							dr.AcceptChanges();
							// re-mark as modified so we can save it again
							dr.BeginEdit();
							dr.EndEdit();
						}
						// reject new changes
						if(optRule.Value.ToString() == "Reject")
						{
							dr.RejectChanges();
						}						
					}
				}
			}
		}
		

		private void btnOK_Click(object sender, EventArgs e)
		{
			if(ds != null && ds.HasErrors && optRule.Value == null)
				App.ShowInfo("Please select from the options.");
			else
			{
				if(ds != null) HandleDataSetRowErrors();
				Close();
			}
		}
		
		private void btnViewDS_Click(object sender, EventArgs e)
		{
			if(ds != null) App.ViewDataSet(ds);
		}

		private void btnWriteDS_Click(object sender, EventArgs e)
		{
		    if (ds == null) return;
		    var dlg = new SaveFileDialog();
		    dlg.FileName = "TW_DataSet_Error.xml";
		    dlg.Filter = ".xml files (*.xml)|*.xml|All files (*.*)|*.*";
		    dlg.FilterIndex = 2;
		    dlg.RestoreDirectory = true;

		    if(dlg.ShowDialog() == DialogResult.OK)
		        ds.WriteXml(dlg.FileName, XmlWriteMode.DiffGram);
		}
				
	}
}
