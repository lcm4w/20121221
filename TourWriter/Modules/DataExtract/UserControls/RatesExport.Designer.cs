namespace TourWriter.Modules.DataExtract.UserControls
{
    partial class RatesExport
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gridReport = new TourWriter.UserControls.DataExtractGrid();
            this.txtStartDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtEndDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndDate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(3, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(84, 4);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 23);
            this.btnExcel.TabIndex = 6;
            this.btnExcel.Text = "Excel";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(165, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 23);
            this.progressBar.TabIndex = 79;
            // 
            // gridReport
            // 
            this.gridReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridReport.DataSource = null;
            this.gridReport.ExportFileName = "";
            this.gridReport.GridEnabled = true;
            this.gridReport.Location = new System.Drawing.Point(3, 30);
            this.gridReport.Name = "gridReport";
            this.gridReport.Size = new System.Drawing.Size(766, 480);
            this.gridReport.TabIndex = 5;
            // 
            // txtStartDate
            // 
            this.txtStartDate.Location = new System.Drawing.Point(347, 5);
            this.txtStartDate.MaskInput = "{date}";
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.Size = new System.Drawing.Size(88, 21);
            this.txtStartDate.TabIndex = 81;
            // 
            // txtEndDate
            // 
            this.txtEndDate.Location = new System.Drawing.Point(514, 5);
            this.txtEndDate.MaskInput = "{date}";
            this.txtEndDate.Name = "txtEndDate";
            this.txtEndDate.Size = new System.Drawing.Size(88, 21);
            this.txtEndDate.TabIndex = 82;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(288, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 83;
            this.label1.Text = "Start date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(458, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 84;
            this.label2.Text = "End date";
            // 
            // RatesExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEndDate);
            this.Controls.Add(this.txtStartDate);
            this.Controls.Add(this.gridReport);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.btnLoad);
            this.Name = "RatesExport";
            this.Size = new System.Drawing.Size(772, 513);
            this.Load += new System.EventHandler(this.RatesExport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.ProgressBar progressBar;
        private TourWriter.UserControls.DataExtractGrid gridReport;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtStartDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtEndDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
