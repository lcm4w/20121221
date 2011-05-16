using System;
using System.ComponentModel;
using System.Windows.Forms;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.ContactModule;
using TourWriter.Properties;
using TourWriter.Utilities.Encryption;

namespace TourWriter.Modules.AdminModule.UserControls
{
    /// <summary>
    /// Summary description for NewUser.
    /// </summary>
    public class UserDetail : UserControl
    {
        #region Designer

        private LinkLabel btnContact;
        private Label label5;
        private Label lblHeading;
        private Label label6;
        private Label label1;
        private Button btnReset;
        private Label label7;
        private TextBox txtDisplayName;
        private Label label4;
        private Button btnChangePassword;
        private TextBox txtEmail;
        private TextBox txtUsername;
        private Container components = null;

        public UserDetail()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.btnContact = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnContact
            // 
            this.btnContact.AutoSize = true;
            this.btnContact.Location = new System.Drawing.Point(311, 214);
            this.btnContact.Name = "btnContact";
            this.btnContact.Size = new System.Drawing.Size(77, 13);
            this.btnContact.TabIndex = 45;
            this.btnContact.TabStop = true;
            this.btnContact.Text = "Contact details";
            this.btnContact.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContact.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnContact_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(153, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 13);
            this.label5.TabIndex = 44;
            this.label5.Text = "View your contact details here:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 42;
            this.lblHeading.Text = "My account details";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 28);
            this.label6.TabIndex = 41;
            this.label6.Text = "Edit your login and contact details.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Username";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(256, 302);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(132, 23);
            this.btnReset.TabIndex = 49;
            this.btnReset.Text = "Reset default settings";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(98, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 51;
            this.label7.Text = "Display name";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(176, 94);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(212, 20);
            this.txtDisplayName.TabIndex = 52;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(98, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 53;
            this.label4.Text = "Email address";
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(256, 156);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(132, 23);
            this.btnChangePassword.TabIndex = 54;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(176, 120);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(212, 20);
            this.txtEmail.TabIndex = 55;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(176, 68);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(212, 20);
            this.txtUsername.TabIndex = 56;
            // 
            // UserDetail
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnContact);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Name = "UserDetail";
            this.Size = new System.Drawing.Size(520, 452);
            this.Load += new System.EventHandler(this.UserDetail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        private UserSet userSet
        {
            get { return (Tag as AdminMain).UserSet; }
        }

        private void UserDetail_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        private void DataBind()
        {
            txtUsername.DataBindings.Add("Text", Cache.User, "Username");
            txtDisplayName.DataBindings.Add("Text", Cache.User, "DisplayName");
            txtEmail.DataBindings.Add("Text", Cache.User, "Email");
        }

        private void btnContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Cache.User.IsContactIDNull())
            {
                if (!App.AskCreateRow())
                    return;

                ContactMain contact = new ContactMain();
                if (contact.ShowDialog(this) == DialogResult.OK)
                {
                    // get new Contact row
                    ContactSet.ContactRow c = (ContactSet.ContactRow) contact.ContactRow;

                    // load contact row into this userSet
                    userSet.Contact.BeginLoadData();
                    userSet.Contact.LoadDataRow(c.ItemArray, true);
                    userSet.Contact.EndLoadData();

                    // add FK value
                    Cache.User.ContactID = c.ContactID;
                }
            }
            else // open existing contact record
            {
                int contactID = Cache.User.ContactID;
                ContactMain contact = new ContactMain(contactID);

                // reload contact to reflect changes
                if (contact.ShowDialog(this) == DialogResult.OK)
                {
                    userSet.Contact.BeginLoadData();
                    userSet.Contact.LoadDataRow(contact.ContactRow.ItemArray, true);
                    userSet.Contact.EndLoadData();
                }
                contact.Dispose();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Settings.Default.Reset();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            PasswordForm passwordEditor = new PasswordForm();
            bool passwordChanged = passwordEditor.ShowDialog() == DialogResult.OK;

            if (passwordChanged)
            {
                var password = EncryptionHelper.EncryptString(passwordEditor.Password);
                if (password != Cache.User.Password) Cache.User.Password = password;
            }
        }
    }
}