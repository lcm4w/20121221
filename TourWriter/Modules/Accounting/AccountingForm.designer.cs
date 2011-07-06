namespace TourWriter.Modules.Accounting
{
    partial class AccountingForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.accountingControl1 = new UserControls.Accounting.Accounting();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.accountingControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(664, 499);
            this.panel1.TabIndex = 77;
            // 
            // accountingControl1
            // 
            this.accountingControl1.AutoScroll = true;
            this.accountingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountingControl1.Location = new System.Drawing.Point(0, 0);
            this.accountingControl1.MinimumSize = new System.Drawing.Size(500, 300);
            this.accountingControl1.Name = "accountingControl1";
            this.accountingControl1.Size = new System.Drawing.Size(664, 499);
            this.accountingControl1.TabIndex = 0;
            // 
            // AccountingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(664, 539);
            this.Controls.Add(this.panel1);
            this.HeaderVisible = true;
            this.Name = "AccountingForm";
            this.Text = "Accounting Export";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private UserControls.Accounting.Accounting accountingControl1;
    }
}
