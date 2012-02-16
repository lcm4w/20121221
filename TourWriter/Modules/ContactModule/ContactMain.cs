using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.BusinessLogic;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ContactModule
{
    /// <summary>
    /// Summary description for ContactMain.
    /// </summary>
    public partial class ContactMain : ModuleBase
    {
        private ContactSet contactSet;

        public DataRow ContactRow
        {
            get { return contactSet.Contact[0]; }
        }

        public ContactMain()
        {
            InitializeComponent();
            displayTypeName = "Contact";
            HeaderVisible = false;

            // Create a new contact row
            contactSet = new ContactSet();
            ContactSet.ContactRow row = contactSet.Contact.NewContactRow();
            row.ContactName = "New Contact";
            row.AddedBy = TourWriter.Global.Cache.User.UserID;
            row.AddedOn = DateTime.Now;
            contactSet.Contact.AddContactRow(row);
            ApplyLicenseStatus();
        }

        public ContactMain(int contactID)
        {
            InitializeComponent();
            displayTypeName = "Contact";

            // get existing row
            Contact c = new Contact();
            contactSet = c.GetContactSet(contactID);
            ApplyLicenseStatus();
        }

        public void ApplyLicenseStatus()
        {
            if (IsReadOnly)
            {
                toolSave.Enabled = false;
            }
        }

        private void ContactMain_Load(object sender, EventArgs e)
        {
            Icon = TourWriter.Properties.Resources.TourWriter16;
            
            // bind controls

            // name
            txtDisplayName.DataBindings.Add("Text", contactSet.Contact, "ContactName");
            txtTitle.DataBindings.Add("Text", contactSet.Contact, "Title");
            txtFirstName.DataBindings.Add("Text", contactSet.Contact, "FirstName");
            txtLastName.DataBindings.Add("Text", contactSet.Contact, "LastName");
            txtJob.DataBindings.Add("Text", contactSet.Contact, "JobDescription");
            txtNotes.DataBindings.Add("Text", contactSet.Contact, "Notes");
            txtBirthDate.DataBindings.Add("Text", contactSet.Contact, "BirthDate");

            // phone
            txtFax.DataBindings.Add("Text", contactSet.Contact, "Fax");
            txtWorkPhone.DataBindings.Add("Text", contactSet.Contact, "WorkPhone");
            txtHomePhone.DataBindings.Add("Text", contactSet.Contact, "HomePhone");
            txtMobilePhone.DataBindings.Add("Text", contactSet.Contact, "CellPhone");

            // email			
            txtEmail1.DataBindings.Add("Text", contactSet.Contact, "Email1");
            txtEmail2.DataBindings.Add("Text", contactSet.Contact, "Email2");
            txtUrl.DataBindings.Add("Text", contactSet.Contact, "Website");

            // address
            txtPostName.DataBindings.Add("Text", contactSet.Contact, "PostName");
            txtPostAddress.DataBindings.Add("Text", contactSet.Contact, "PostAddress");
            txtStreetAddress.DataBindings.Add("Text", contactSet.Contact, "StreetAddress");
//			txtPostCode.DataBindings.Add("Text", contactSet.Contact, "PostCode");
//			// country
//			cmbCountry.DataSource = toolSet.Country;
//			cmbCountry.ValueMember = "CountryID";
//			cmbCountry.DisplayMember = "CountryName";
//			cmbCountry.DataBindings.Add("Value", contactSet, "Contact.CountryID");
//			// state
//			cmbState.DataSource = toolSet.State;
//			cmbState.ValueMember = "StateID";
//			cmbState.DisplayMember = "StateName";
//			cmbState.DataBindings.Add("Value", contactSet, "Contact.StateID");
//			// region
//			cmbRegion.DataSource = toolSet.Region;
//			cmbRegion.ValueMember = "RegionID";
//			cmbRegion.DisplayMember = "RegionName";
//			cmbRegion.DataBindings.Add("Value", contactSet, "Contact.RegionID");			
//			// city
//			cmbCity.DataSource = toolSet.City;
//			cmbCity.ValueMember = "CityID";
//			cmbCity.DisplayMember = "CityName";
//			cmbCity.DataBindings.Add("Value", contactSet, "Contact.CityID");

            // set selection
            txtDisplayName.Select();

            // add events			
//			this.cmbCountry.ValueChanged += new System.EventHandler(this.cmbCountry_ValueChanged);
//			this.cmbState.ValueChanged += new System.EventHandler(this.cmbState_ValueChanged);
//			this.cmbRegion.ValueChanged += new System.EventHandler(this.cmbRegion_ValueChanged);
//			this.cmbCity.ValueChanged += new System.EventHandler(this.cmbCity_ValueChanged);

            LoadContactCategories(contactSet.Contact[0].ContactID);
        }

        protected override bool IsDataDirty()
        {
            CommitOpenEdits();
            return contactSet != null && contactSet.HasChanges();
        }

        protected override void CommitOpenEdits()
        {
            Validate();

            App.CommitGridEdits(this);

            foreach (DataTable dt in contactSet.Tables)
                foreach (DataRow dr in dt.Rows)
                    dr.EndEdit();
        }

        protected override void SaveDataChanges()
        {
            if (!App.ShowCheckPermission(AppPermissions.Permissions.ContactEdit))
                return;

            if (IsDataDirty())
            {
                Cursor c = Cursor;
                Cursor = Cursors.WaitCursor;

                try
                {
                    // save changes
                    Contact contact = new Contact();
                    ContactSet changes = (ContactSet) contactSet.GetChanges();
                    if (changes == null)
                    {
                        SetDataCleanName();
                        return;
                    }
                    ContactSet fresh = contact.SaveContactSet(changes);

                    // handle errors
                    App.DataSet_CheckForErrors(fresh);
                    if (App.DataSet_AskSaveDeleteConstraints(fresh))
                    {
                        // clear and merge to maintain any databindings
                        contactSet.Clear();
                        contactSet.Merge(fresh, false);
                        // and save again
                        SaveDataChanges();
                        return;
                    }
                    // clear and merge to maintain any databindings
                    contactSet.Clear();
                    contactSet.Merge(fresh, false);

                    UpdateMainForm(App.MainForm.ContactMenu, contactSet.Contact[0].IsRecordActive);

                    SetDataCleanName();
                }
                finally
                {
                    Cursor = c;
                }
            }
        }

        protected override void CancelDataChanges()
        {
            return;
        }

        protected override string GetDisplayName()
        {
            string s = "TourWriter contact";
            
            if (contactSet != null && contactSet.Contact.Count > 0)
                return s + ": " + contactSet.Contact[0].ContactName;
            
            return s;
        }

        private void LoadContactCategories(int contactId)
        {
            treeCategories.Override.Sort = SortType.Default;
            treeCategories.Nodes.Clear();

            foreach (ToolSet.ContactCategoryRow row in Cache.ToolSet.ContactCategory)
            {
                UltraTreeNode node =
                    new UltraTreeNode(row.ContactCategoryID.ToString(), row.ContactCategoryName);
                
                int categoryId = row.ContactCategoryID;

                var rows = from r in contactSet.ContactContactCategory
                            where r.ContactID == contactId &&
                                  r.ContactCategoryID == categoryId
                            select r;

                node.CheckedState = (rows.Count() > 0) ? CheckState.Checked : CheckState.Unchecked;
                treeCategories.Nodes.Add(node);
            }
            treeCategories.Override.Sort = SortType.Ascending;
        }

        private void treeCategories_AfterCheck(object sender, NodeEventArgs e)
        {
            bool accept = (e.TreeNode.CheckedState == CheckState.Checked);
            
            ContactSet.ContactRow contact = contactSet.Contact[0];
            ToolSet.ContactCategoryRow category =
                Cache.ToolSet.ContactCategory.FindByContactCategoryID(int.Parse(e.TreeNode.Key));

            if (accept)
            {
                // create the new row
                ContactSet.ContactContactCategoryRow row =
                    contactSet.ContactContactCategory.NewContactContactCategoryRow();

                row.ContactID = contact.ContactID;
                row.ContactCategoryID = category.ContactCategoryID;

                // add the new ContactContactCategory
                contactSet.ContactContactCategory.AddContactContactCategoryRow(row);
            }
            else // unchecked so remove
            {
                var rows = from r in contactSet.ContactContactCategory
                           where r.ContactID == contact.ContactID &&
                                 r.ContactCategoryID == category.ContactCategoryID
                           select r;

                rows.ToArray()[0].Delete();
            }
        }

        private void txtBirthDate_Validating(object sender, CancelEventArgs e)
        {
            if (txtBirthDate.Value == null)
                contactSet.Contact[0].SetBirthDateNull();
        }

        private void txtDisplayName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDisplayName.Text.Trim()))
            {
                MessageBox.Show(String.Format(App.GetResourceString("ShowValueNotValid"), "Display Name"));
                e.Cancel = true;
            }
        }

        private void btnEmail1_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:" + txtEmail1.Text.Replace("mailto:", ""));
        }

        private void btnEmail2_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:" + txtEmail2.Text.Replace("mailto:", ""));
        }

        private void btnWebsite_Click(object sender, EventArgs e)
        {
            App.OpenUrl(txtUrl.Text);
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            SaveDataChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
