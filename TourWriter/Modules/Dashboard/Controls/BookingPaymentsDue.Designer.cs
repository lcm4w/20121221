namespace TourWriter.Modules.Dashboard.Controls
{
    partial class BookingPaymentsDue
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
            this.dataExtractGrid1 = new TourWriter.UserControls.DataExtractGrid();
            this.SuspendLayout();
            // 
            // dataExtractGrid1
            // 
            this.dataExtractGrid1.DataSource = null;
            this.dataExtractGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataExtractGrid1.ExportFileName = "";
            this.dataExtractGrid1.GridEnabled = true;
            this.dataExtractGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataExtractGrid1.Name = "dataExtractGrid1";
            this.dataExtractGrid1.Size = new System.Drawing.Size(597, 414);
            this.dataExtractGrid1.TabIndex = 0;
            // 
            // ItineraryPaymentsDue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataExtractGrid1);
            this.Name = "ItineraryPaymentsDue";
            this.Size = new System.Drawing.Size(597, 414);
            this.Load += new System.EventHandler(this.BookingPaymentsDue_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.DataExtractGrid dataExtractGrid1;
    }
}
