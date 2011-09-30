namespace TourWriter.UserControls.DatabaseConnection
{
    partial class TrialDatabase
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtEmail2 = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblEmail2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.msgEmail = new System.Windows.Forms.Label();
            this.msgName = new System.Windows.Forms.Label();
            this.msgCompany = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.msgEmail2 = new System.Windows.Forms.Label();
            this.lnkInfo = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(113, 113);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(349, 20);
            this.txtCompany.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtCompany, "For your default Agent name");
            this.txtCompany.TextChanged += new System.EventHandler(this.txtCompany_TextChanged);
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(113, 142);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(349, 20);
            this.txtUser.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtUser, "For sending bookings, reports, and publishing");
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
            // 
            // txtEmail2
            // 
            this.txtEmail2.Location = new System.Drawing.Point(113, 201);
            this.txtEmail2.Name = "txtEmail2";
            this.txtEmail2.Size = new System.Drawing.Size(349, 20);
            this.txtEmail2.TabIndex = 55;
            this.toolTip1.SetToolTip(this.txtEmail2, "Enter email again to confirm");
            this.txtEmail2.Visible = false;
            this.txtEmail2.TextChanged += new System.EventHandler(this.txtEmail2_TextChanged);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(113, 172);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(349, 20);
            this.txtEmail.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtEmail, "For sending booking requests");
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            this.txtEmail.Enter += new System.EventHandler(this.txtEmail_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Email address:";
            this.toolTip1.SetToolTip(this.label4, "For sending booking requests");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Your Full name:";
            this.toolTip1.SetToolTip(this.label6, "For sending bookings, reports, and publishing");
            // 
            // lblEmail2
            // 
            this.lblEmail2.AutoSize = true;
            this.lblEmail2.Location = new System.Drawing.Point(20, 200);
            this.lblEmail2.Name = "lblEmail2";
            this.lblEmail2.Size = new System.Drawing.Size(75, 26);
            this.lblEmail2.TabIndex = 56;
            this.lblEmail2.Text = "Confirm your \r\nEmail address:";
            this.toolTip1.SetToolTip(this.lblEmail2, "Enter email again to confirm");
            this.lblEmail2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Company name:";
            this.toolTip1.SetToolTip(this.label1, "For your default Agent name");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(18, 198);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(448, 143);
            this.panel1.TabIndex = 54;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(117, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(214, 20);
            this.label3.TabIndex = 55;
            this.label3.Text = "Click Login for instant access";
            // 
            // msgEmail
            // 
            this.msgEmail.BackColor = System.Drawing.SystemColors.Window;
            this.msgEmail.ForeColor = System.Drawing.Color.Red;
            this.msgEmail.Location = new System.Drawing.Point(392, 175);
            this.msgEmail.Name = "msgEmail";
            this.msgEmail.Size = new System.Drawing.Size(66, 13);
            this.msgEmail.TabIndex = 53;
            this.msgEmail.Text = "required";
            this.msgEmail.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.msgEmail.Visible = false;
            // 
            // msgName
            // 
            this.msgName.BackColor = System.Drawing.SystemColors.Window;
            this.msgName.ForeColor = System.Drawing.Color.Red;
            this.msgName.Location = new System.Drawing.Point(392, 145);
            this.msgName.Name = "msgName";
            this.msgName.Size = new System.Drawing.Size(66, 13);
            this.msgName.TabIndex = 52;
            this.msgName.Text = "required";
            this.msgName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.msgName.Visible = false;
            // 
            // msgCompany
            // 
            this.msgCompany.BackColor = System.Drawing.SystemColors.Window;
            this.msgCompany.ForeColor = System.Drawing.Color.Red;
            this.msgCompany.Location = new System.Drawing.Point(392, 116);
            this.msgCompany.Name = "msgCompany";
            this.msgCompany.Size = new System.Drawing.Size(66, 13);
            this.msgCompany.TabIndex = 44;
            this.msgCompany.Text = "required";
            this.msgCompany.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.msgCompany.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(20, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 26);
            this.label2.TabIndex = 51;
            this.label2.Text = "Please enter the default settings for your new online database.";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label11.Location = new System.Drawing.Point(56, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 20);
            this.label11.TabIndex = 48;
            this.label11.Text = "Free Trial setup";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::TourWriter.Properties.Resources.group_add;
            this.pictureBox2.Location = new System.Drawing.Point(18, 15);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(33, 32);
            this.pictureBox2.TabIndex = 49;
            this.pictureBox2.TabStop = false;
            // 
            // msgEmail2
            // 
            this.msgEmail2.BackColor = System.Drawing.SystemColors.Window;
            this.msgEmail2.ForeColor = System.Drawing.Color.Red;
            this.msgEmail2.Location = new System.Drawing.Point(395, 204);
            this.msgEmail2.Name = "msgEmail2";
            this.msgEmail2.Size = new System.Drawing.Size(63, 14);
            this.msgEmail2.TabIndex = 59;
            this.msgEmail2.Text = "not match";
            this.msgEmail2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.msgEmail2.Visible = false;
            // 
            // lnkInfo
            // 
            this.lnkInfo.AutoSize = true;
            this.lnkInfo.Location = new System.Drawing.Point(287, 79);
            this.lnkInfo.Name = "lnkInfo";
            this.lnkInfo.Size = new System.Drawing.Size(56, 13);
            this.lnkInfo.TabIndex = 60;
            this.lnkInfo.TabStop = true;
            this.lnkInfo.Text = "(more info)";
            this.lnkInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkInfo_LinkClicked);
            // 
            // TrialDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkInfo);
            this.Controls.Add(this.msgEmail2);
            this.Controls.Add(this.lblEmail2);
            this.Controls.Add(this.msgEmail);
            this.Controls.Add(this.msgName);
            this.Controls.Add(this.msgCompany);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtEmail2);
            this.Controls.Add(this.panel1);
            this.Name = "TrialDatabase";
            this.Size = new System.Drawing.Size(477, 344);
            this.Load += new System.EventHandler(this.OnLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label msgCompany;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label msgName;
        private System.Windows.Forms.Label msgEmail;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEmail2;
        private System.Windows.Forms.Label lblEmail2;
        private System.Windows.Forms.Label msgEmail2;
        private System.Windows.Forms.LinkLabel lnkInfo;
    }
}
