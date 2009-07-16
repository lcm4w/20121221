using TourWriter.Utilities.Xml;

namespace TourWriter.Forms
{
	/// <summary>
	/// Summary description for DataGrid.
	/// </summary>
	public class DataSourceViewer : System.Windows.Forms.Form
	{
		#region Designer
		private System.Windows.Forms.DataGrid dataGrid2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRefresh;
		private System.ComponentModel.Container components = null;
		
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
            this.dataGrid2 = new System.Windows.Forms.DataGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid2
            // 
            this.dataGrid2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid2.DataMember = "";
            this.dataGrid2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid2.Location = new System.Drawing.Point(4, 4);
            this.dataGrid2.Name = "dataGrid2";
            this.dataGrid2.Size = new System.Drawing.Size(824, 450);
            this.dataGrid2.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(735, 460);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save as XML";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(644, 460);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // DataSourceViewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(832, 494);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGrid2);
            this.Name = "DataSourceViewer";
            this.Text = "DataGrid";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DataSourceViewer_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
		#endregion

	    private const string defaultSaveFile = "Debug.xml";

		public DataSourceViewer()
		{
			InitializeComponent();
		}

        public DataSourceViewer(object dataSourceToView)
        {
            InitializeComponent();

            this.DataSourceToView = dataSourceToView;
        }


		public object DataSourceToView
		{
			set
			{
//				this.dataGrid1.DataSource = value;				
//				try{this.dataGrid1.Rows.ExpandAll(true);}
//				catch{}

				this.dataGrid2.DataSource = value;
				try{this.dataGrid2.Expand(-1);}
				catch{}
			}
		}

		private void DataSourceViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			App.ClearBindings(this);
		}

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SaveAsXml();
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshDataSource();
        }

        private void SaveAsXml()
        {
            string filename = App.PromptSaveFile(defaultSaveFile, "XML", ".xml");

            XmlHelper.SerialiseToFile(filename, dataGrid2.DataSource);
        }

        private void RefreshDataSource()
        {
            object ds = dataGrid2.DataSource;
            DataSourceToView = null;
            DataSourceToView = ds;
        }
	}
}
