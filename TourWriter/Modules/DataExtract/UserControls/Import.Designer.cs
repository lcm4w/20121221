namespace TourWriter.Modules.DataExtract.UserControls
{
    partial class Import
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSave = new System.Windows.Forms.Button();
            this.gridData = new System.Windows.Forms.DataGridView();
            this.btnLoad = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblExplain1 = new System.Windows.Forms.Label();
            this.chkSaveToFolder = new System.Windows.Forms.CheckBox();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.lblExplain2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(84, 34);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.toolTip1.SetToolTip(this.btnSave, "Save the data to the TourWriter database");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridData
            // 
            this.gridData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridData.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gridData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridData.Location = new System.Drawing.Point(0, 124);
            this.gridData.Name = "gridData";
            this.gridData.ReadOnly = true;
            this.gridData.Size = new System.Drawing.Size(766, 386);
            this.gridData.TabIndex = 8;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(3, 34);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 7;
            this.btnLoad.Text = "Load";
            this.toolTip1.SetToolTip(this.btnLoad, "Choose the excel file that contains the data you wish to load");
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblExplain1
            // 
            this.lblExplain1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExplain1.Location = new System.Drawing.Point(3, 5);
            this.lblExplain1.Name = "lblExplain1";
            this.lblExplain1.Size = new System.Drawing.Size(766, 26);
            this.lblExplain1.TabIndex = 12;
            this.lblExplain1.Text = "The data will be saved to the database exactly as it appears below. Please manipu" +
                "late the data in the spreadsheet before loading, to remove unwanted rows and/or " +
                "re-sort columns.";
            this.lblExplain1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // chkSaveToFolder
            // 
            this.chkSaveToFolder.AutoSize = true;
            this.chkSaveToFolder.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSaveToFolder.Checked = true;
            this.chkSaveToFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveToFolder.Location = new System.Drawing.Point(3, 100);
            this.chkSaveToFolder.Name = "chkSaveToFolder";
            this.chkSaveToFolder.Size = new System.Drawing.Size(121, 17);
            this.chkSaveToFolder.TabIndex = 13;
            this.chkSaveToFolder.Text = "Save to new folder?";
            this.chkSaveToFolder.UseVisualStyleBackColor = true;
            this.chkSaveToFolder.CheckedChanged += new System.EventHandler(this.chkSaveToFolder_CheckedChanged);
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(206, 98);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.Size = new System.Drawing.Size(130, 20);
            this.txtFolderName.TabIndex = 14;
            // 
            // lblExplain2
            // 
            this.lblExplain2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExplain2.Location = new System.Drawing.Point(3, 69);
            this.lblExplain2.Name = "lblExplain2";
            this.lblExplain2.Size = new System.Drawing.Size(746, 26);
            this.lblExplain2.TabIndex = 16;
            this.lblExplain2.Text = "The imported {0}s will be saved to this folder. This allows you to better manage " +
                "the integration of the imported {0}s into the rest of your {0}s.";
            this.lblExplain2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(135, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Folder name";
            // 
            // Import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.chkSaveToFolder);
            this.Controls.Add(this.lblExplain1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gridData);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblExplain2);
            this.Name = "Import";
            this.Size = new System.Drawing.Size(772, 513);
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView gridData;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblExplain1;
        private System.Windows.Forms.CheckBox chkSaveToFolder;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.Label lblExplain2;
        private System.Windows.Forms.Label label1;
    }
}
