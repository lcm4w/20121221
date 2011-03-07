namespace TourWriter.UserControls.DatabaseConfig
{
    partial class InstallOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallOptions));
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblRestoreFile = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnRestoreFile = new System.Windows.Forms.Button();
            this.btnInstallFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlVersion = new System.Windows.Forms.Panel();
            this.rd86 = new System.Windows.Forms.RadioButton();
            this.rd32 = new System.Windows.Forms.RadioButton();
            this.rd64 = new System.Windows.Forms.RadioButton();
            this.lblInstallFile = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOptions = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(242, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Install the database server software on this Server";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TourWriter.Properties.Resources.database_add;
            this.pictureBox1.Location = new System.Drawing.Point(15, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 32);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(51, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 20);
            this.label3.TabIndex = 29;
            this.label3.Text = "Install Server softare";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 31);
            this.label1.TabIndex = 31;
            this.label1.Text = "This computer will act as the Database Server. Other staff will connect to this s" +
                "erver to share the database.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(14, 157);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 185);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblRestoreFile);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.btnRestoreFile);
            this.panel1.Controls.Add(this.btnInstallFile);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.pnlVersion);
            this.panel1.Controls.Add(this.lblInstallFile);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(446, 166);
            this.panel1.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 284);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "-";
            // 
            // lblRestoreFile
            // 
            this.lblRestoreFile.ForeColor = System.Drawing.Color.Blue;
            this.lblRestoreFile.Location = new System.Drawing.Point(149, 227);
            this.lblRestoreFile.Name = "lblRestoreFile";
            this.lblRestoreFile.Size = new System.Drawing.Size(277, 57);
            this.lblRestoreFile.TabIndex = 24;
            this.lblRestoreFile.Text = "<custom>";
            this.lblRestoreFile.Visible = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(418, 32);
            this.label7.TabIndex = 1;
            this.label7.Text = "Download and install a different version? TourWriter has preselected the best opt" +
                "ion for you based on your operating system, but you choose a different version h" +
                "ere.";
            // 
            // btnRestoreFile
            // 
            this.btnRestoreFile.Location = new System.Drawing.Point(19, 229);
            this.btnRestoreFile.Name = "btnRestoreFile";
            this.btnRestoreFile.Size = new System.Drawing.Size(124, 23);
            this.btnRestoreFile.TabIndex = 23;
            this.btnRestoreFile.Text = "Choose my database";
            this.btnRestoreFile.UseVisualStyleBackColor = true;
            this.btnRestoreFile.Click += new System.EventHandler(this.OnRestoreFileClick);
            // 
            // btnInstallFile
            // 
            this.btnInstallFile.Location = new System.Drawing.Point(19, 146);
            this.btnInstallFile.Name = "btnInstallFile";
            this.btnInstallFile.Size = new System.Drawing.Size(124, 23);
            this.btnInstallFile.TabIndex = 0;
            this.btnInstallFile.Text = "Choose my download";
            this.btnInstallFile.UseVisualStyleBackColor = true;
            this.btnInstallFile.Click += new System.EventHandler(this.OnInstallFileClick);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(11, 185);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(415, 42);
            this.label5.TabIndex = 22;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // pnlVersion
            // 
            this.pnlVersion.Controls.Add(this.rd86);
            this.pnlVersion.Controls.Add(this.rd32);
            this.pnlVersion.Controls.Add(this.rd64);
            this.pnlVersion.Location = new System.Drawing.Point(19, 35);
            this.pnlVersion.Name = "pnlVersion";
            this.pnlVersion.Size = new System.Drawing.Size(188, 57);
            this.pnlVersion.TabIndex = 3;
            // 
            // rd86
            // 
            this.rd86.AutoSize = true;
            this.rd86.Location = new System.Drawing.Point(3, 37);
            this.rd86.Name = "rd86";
            this.rd86.Size = new System.Drawing.Size(113, 17);
            this.rd86.TabIndex = 4;
            this.rd86.TabStop = true;
            this.rd86.Text = "32-bit for 64 bit OS";
            this.rd86.UseVisualStyleBackColor = true;
            this.rd86.CheckedChanged += new System.EventHandler(this.OnVersionChanged);
            // 
            // rd32
            // 
            this.rd32.AutoSize = true;
            this.rd32.Location = new System.Drawing.Point(3, 3);
            this.rd32.Name = "rd32";
            this.rd32.Size = new System.Drawing.Size(51, 17);
            this.rd32.TabIndex = 0;
            this.rd32.TabStop = true;
            this.rd32.Text = "32-bit";
            this.rd32.UseVisualStyleBackColor = true;
            this.rd32.CheckedChanged += new System.EventHandler(this.OnVersionChanged);
            // 
            // rd64
            // 
            this.rd64.AutoSize = true;
            this.rd64.Location = new System.Drawing.Point(3, 20);
            this.rd64.Name = "rd64";
            this.rd64.Size = new System.Drawing.Size(51, 17);
            this.rd64.TabIndex = 1;
            this.rd64.TabStop = true;
            this.rd64.Text = "64-bit";
            this.rd64.UseVisualStyleBackColor = true;
            this.rd64.CheckedChanged += new System.EventHandler(this.OnVersionChanged);
            // 
            // lblInstallFile
            // 
            this.lblInstallFile.ForeColor = System.Drawing.Color.Blue;
            this.lblInstallFile.Location = new System.Drawing.Point(149, 146);
            this.lblInstallFile.Name = "lblInstallFile";
            this.lblInstallFile.Size = new System.Drawing.Size(277, 36);
            this.lblInstallFile.TabIndex = 21;
            this.lblInstallFile.Text = "<custom>";
            this.lblInstallFile.Visible = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(11, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(415, 40);
            this.label4.TabIndex = 7;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // btnOptions
            // 
            this.btnOptions.AutoSize = true;
            this.btnOptions.Location = new System.Drawing.Point(20, 155);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(95, 13);
            this.btnOptions.TabIndex = 34;
            this.btnOptions.TabStop = true;
            this.btnOptions.Text = "Advanced Options";
            this.btnOptions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnOptionsLinkClicked);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(20, 117);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(446, 31);
            this.label8.TabIndex = 33;
            this.label8.Text = "Click the Install button to continue. TourWriter will automatically perform all t" +
                "he steps for you. This process might take 10 - 20 mintues to complete.";
            // 
            // InstallOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "InstallOptions";
            this.Size = new System.Drawing.Size(477, 342);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlVersion.ResumeLayout(false);
            this.pnlVersion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblInstallFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlVersion;
        private System.Windows.Forms.RadioButton rd86;
        private System.Windows.Forms.RadioButton rd32;
        private System.Windows.Forms.RadioButton rd64;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnInstallFile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel btnOptions;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblRestoreFile;
        private System.Windows.Forms.Button btnRestoreFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
