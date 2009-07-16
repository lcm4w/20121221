namespace TourWriter.Modules.ContactModule
{
    partial class CategorySelector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.label5 = new System.Windows.Forms.Label();
            this.treeCategories = new Infragistics.Win.UltraWinTree.UltraTree();
            this.btnEmail = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.treeCategories)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Categories";
            // 
            // treeCategories
            // 
            this.treeCategories.FullRowSelect = true;
            this.treeCategories.Location = new System.Drawing.Point(12, 24);
            this.treeCategories.Name = "treeCategories";
            _override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended;
            this.treeCategories.Override = _override1;
            this.treeCategories.Size = new System.Drawing.Size(192, 205);
            this.treeCategories.TabIndex = 33;
            // 
            // btnEmail
            // 
            this.btnEmail.Location = new System.Drawing.Point(48, 235);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(75, 23);
            this.btnEmail.TabIndex = 35;
            this.btnEmail.Text = "Email";
            this.btnEmail.UseVisualStyleBackColor = true;
            this.btnEmail.Click += new System.EventHandler(this.btnEmail_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(129, 235);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 36;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // CategorySelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(216, 272);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnEmail);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.treeCategories);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CategorySelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Category Selector";
            ((System.ComponentModel.ISupportInitialize)(this.treeCategories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private Infragistics.Win.UltraWinTree.UltraTree treeCategories;
        private System.Windows.Forms.Button btnEmail;
        private System.Windows.Forms.Button btnClose;

    }
}