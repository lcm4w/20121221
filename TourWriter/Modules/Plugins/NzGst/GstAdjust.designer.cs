namespace TourWriter.Modules.Plugins.NzGst
{
    partial class GstAdjust
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
            this.txtTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grid = new TourWriter.UserControls.DataExtractGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnItinerary = new System.Windows.Forms.Button();
            this.btnSupplier = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(58, 128);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(87, 21);
            this.txtTo.TabIndex = 68;
            this.txtTo.Visible = false;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(58, 101);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(87, 21);
            this.txtFrom.TabIndex = 67;
            this.txtFrom.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 478);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(807, 13);
            this.label3.TabIndex = 69;
            this.label3.Text = "Calculation used to adjust Net and Gross price: ((Amount*100) / (100+12.5)) + (((" +
                "Amount*100) / (100+12.5)) * 15/100), eg remove old GST 12.5% then add new GST 15" +
                "%.";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(290, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 24);
            this.btnLoad.TabIndex = 73;
            this.btnLoad.Text = "Refresh";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(72, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(212, 21);
            this.comboBox1.TabIndex = 75;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 76;
            this.label1.Text = "View data";
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.DataSource = null;
            this.grid.ExportFileName = "";
            this.grid.GridEnabled = true;
            this.grid.Location = new System.Drawing.Point(16, 20);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(805, 435);
            this.grid.TabIndex = 74;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnItinerary);
            this.panel1.Controls.Add(this.btnSupplier);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Controls.Add(this.txtFrom);
            this.panel1.Controls.Add(this.txtTo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.grid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(836, 499);
            this.panel1.TabIndex = 77;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 462);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(578, 13);
            this.label2.TabIndex = 81;
            this.label2.Text = "Filters used to select data: Dates as selected above, Supplier country contains \'" +
                "zealand\', Gross price is a positive number.";
            // 
            // btnItinerary
            // 
            this.btnItinerary.Location = new System.Drawing.Point(572, 12);
            this.btnItinerary.Name = "btnItinerary";
            this.btnItinerary.Size = new System.Drawing.Size(75, 24);
            this.btnItinerary.TabIndex = 80;
            this.btnItinerary.Text = "Itinerary";
            this.btnItinerary.UseVisualStyleBackColor = true;
            this.btnItinerary.Click += new System.EventHandler(this.btnItinerary_Click);
            // 
            // btnSupplier
            // 
            this.btnSupplier.Location = new System.Drawing.Point(491, 12);
            this.btnSupplier.Name = "btnSupplier";
            this.btnSupplier.Size = new System.Drawing.Size(75, 24);
            this.btnSupplier.TabIndex = 79;
            this.btnSupplier.Text = "Supplier";
            this.btnSupplier.UseVisualStyleBackColor = true;
            this.btnSupplier.Click += new System.EventHandler(this.btnSupplier_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(413, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "Open selected";
            // 
            // GstAdjust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(836, 539);
            this.Controls.Add(this.panel1);
            this.HeaderVisible = true;
            this.Name = "GstAdjust";
            this.Text = "NZ Gst Adjust";
            this.Controls.SetChildIndex(this.panel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.txtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtTo;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLoad;
        private TourWriter.UserControls.DataExtractGrid grid;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnItinerary;
        private System.Windows.Forms.Button btnSupplier;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
    }
}
