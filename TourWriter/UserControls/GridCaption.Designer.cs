namespace TourWriter.UserControls
{
    partial class GridCaption
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.btnCopy = new Infragistics.Win.Misc.UltraButton();
            this.btnAdd = new Infragistics.Win.Misc.UltraButton();
            this.btnDelete = new Infragistics.Win.Misc.UltraButton();
            this.pnlDelete = new System.Windows.Forms.Panel();
            this.pnlAdd = new System.Windows.Forms.Panel();
            this.pnlCopy = new System.Windows.Forms.Panel();
            this.txtCaption = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlDelete.SuspendLayout();
            this.pnlAdd.SuspendLayout();
            this.pnlCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance1.Image = global::TourWriter.Properties.Resources.PageCopy;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            this.btnCopy.Appearance = appearance1;
            this.btnCopy.ButtonStyle = Infragistics.Win.UIElementButtonStyle.OfficeXPToolbarButton;
            this.btnCopy.Location = new System.Drawing.Point(0, 0);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.ShowOutline = false;
            this.btnCopy.Size = new System.Drawing.Size(25, 25);
            this.btnCopy.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnCopy.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnCopy, "Copy selected record");
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance2.Image = global::TourWriter.Properties.Resources.Plus;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            this.btnAdd.Appearance = appearance2;
            this.btnAdd.ButtonStyle = Infragistics.Win.UIElementButtonStyle.OfficeXPToolbarButton;
            this.btnAdd.Location = new System.Drawing.Point(0, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 25);
            this.btnAdd.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnAdd.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnAdd, "Add new record");
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance3.Image = global::TourWriter.Properties.Resources.Remove;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            this.btnDelete.Appearance = appearance3;
            this.btnDelete.ButtonStyle = Infragistics.Win.UIElementButtonStyle.OfficeXPToolbarButton;
            this.btnDelete.Location = new System.Drawing.Point(0, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(25, 25);
            this.btnDelete.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnDelete.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnDelete, "Delete selected record");
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pnlDelete
            // 
            this.pnlDelete.Controls.Add(this.btnDelete);
            this.pnlDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDelete.Location = new System.Drawing.Point(250, 0);
            this.pnlDelete.Name = "pnlDelete";
            this.pnlDelete.Size = new System.Drawing.Size(25, 25);
            this.pnlDelete.TabIndex = 35;
            // 
            // pnlAdd
            // 
            this.pnlAdd.Controls.Add(this.btnAdd);
            this.pnlAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlAdd.Location = new System.Drawing.Point(225, 0);
            this.pnlAdd.Name = "pnlAdd";
            this.pnlAdd.Size = new System.Drawing.Size(25, 25);
            this.pnlAdd.TabIndex = 36;
            // 
            // pnlCopy
            // 
            this.pnlCopy.Controls.Add(this.btnCopy);
            this.pnlCopy.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlCopy.Location = new System.Drawing.Point(200, 0);
            this.pnlCopy.Name = "pnlCopy";
            this.pnlCopy.Size = new System.Drawing.Size(25, 25);
            this.pnlCopy.TabIndex = 37;
            // 
            // txtCaption
            // 
            this.txtCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCaption.Location = new System.Drawing.Point(0, 0);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.txtCaption.Size = new System.Drawing.Size(200, 25);
            this.txtCaption.TabIndex = 4;
            this.txtCaption.Text = "Caption text";
            this.txtCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GridCaption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.txtCaption);
            this.Controls.Add(this.pnlCopy);
            this.Controls.Add(this.pnlAdd);
            this.Controls.Add(this.pnlDelete);
            this.MinimumSize = new System.Drawing.Size(0, 25);
            this.Name = "GridCaption";
            this.Size = new System.Drawing.Size(275, 25);
            this.pnlDelete.ResumeLayout(false);
            this.pnlAdd.ResumeLayout(false);
            this.pnlCopy.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnCopy;
        private Infragistics.Win.Misc.UltraButton btnAdd;
        private Infragistics.Win.Misc.UltraButton btnDelete;
        private System.Windows.Forms.Panel pnlDelete;
        private System.Windows.Forms.Panel pnlAdd;
        private System.Windows.Forms.Panel pnlCopy;
        private System.Windows.Forms.Label txtCaption;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
