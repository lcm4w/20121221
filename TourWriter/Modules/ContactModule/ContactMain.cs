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
using Infragistics.Win.UltraWinGrid;

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

        private static ToolSet toolSet
        {
            get { return Cache.ToolSet; }
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
            txtPostcode.DataBindings.Add("Text", contactSet.Contact, "PostCode");

            // country
            cmbCountry.DataSource = toolSet.Country;
            cmbCountry.ValueMember = "CountryID";
            cmbCountry.DisplayMember = "CountryName";
            cmbCountry.DataBindings.Add("Value", contactSet, "Contact.CountryID");

            // state
            cmbState.DataSource = toolSet.State;
            cmbState.ValueMember = "StateID";
            cmbState.DisplayMember = "StateName";
            cmbState.DataBindings.Add("Value", contactSet, "Contact.StateID");

            // region
            cmbRegion.DataSource = toolSet.Region;
            cmbRegion.ValueMember = "RegionID";
            cmbRegion.DisplayMember = "RegionName";
            cmbRegion.DataBindings.Add("Value", contactSet, "Contact.RegionID");

            // city
            cmbCity.DataSource = toolSet.City;
            cmbCity.ValueMember = "CityID";
            cmbCity.DisplayMember = "CityName";
            cmbCity.DataBindings.Add("Value", contactSet, "Contact.CityID");

            // set selection
            txtDisplayName.Select();

            // add events			
            cmbCountry.ValueChanged += new System.EventHandler(this.cmbCountry_ValueChanged);
            cmbState.ValueChanged += new System.EventHandler(this.cmbState_ValueChanged);
            cmbRegion.ValueChanged += new System.EventHandler(this.cmbRegion_ValueChanged);
            cmbCity.ValueChanged += new System.EventHandler(this.cmbCity_ValueChanged);

            LoadContactCategories(contactSet.Contact[0].ContactID);
            CalculateAge();
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
                    if (contactSet.HasChanges())
                    {
                        SetDataCleanName();
                        return;
                    }
                    ContactSet fresh = contact.SaveContactSet(contactSet);

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

        private void txtBirthDate_ValueChanged(object sender, EventArgs e)
        {
            CalculateAge();
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

        private void CalculateAge()
        {
            if (txtBirthDate.Value != null)
            {
                DateTime birthDate = DateTime.Parse(txtBirthDate.Value.ToString());
                DateTime now = DateTime.Today; 
                
                int years = now.Year - birthDate.Year; 
                if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) 
                    --years;

                txtAge.Text = years.ToString();              
            }
        }

        #region Combo boxes event handlers
        private void cmbCity_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CityName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbRegion_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "RegionName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbState_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "StateName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbCountry_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CountryName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbCity_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCity.ActiveRow != null && (contactSet.Contact[0].IsCityIDNull() ||
                                                  (int)cmbCity.ActiveRow.Cells["CityID"].Value !=
                                                  contactSet.Contact[0].CityID))
                {
                    // push change to dataset
                    contactSet.Contact[0].CityID = (int)cmbCity.ActiveRow.Cells["CityID"].Value;
                    // auto-fill parent list
                    cmbRegion.Value = (int)cmbCity.ActiveRow.Cells["RegionID"].Value;
                }
                else
                    // clear parent list
                    cmbRegion.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbRegion_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbRegion.ActiveRow != null && (contactSet.Contact[0].IsRegionIDNull() ||
                                                    (int)cmbRegion.ActiveRow.Cells["RegionID"].Value !=
                                                    contactSet.Contact[0].RegionID))
                {
                    // push change to dataset
                    contactSet.Contact[0].RegionID = (int)cmbRegion.ActiveRow.Cells["RegionID"].Value;
                    // auto-fill parent list
                    cmbState.Value = (int)cmbRegion.ActiveRow.Cells["StateID"].Value;
                }
                else
                    // clear parent list
                    cmbState.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbState_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbState.ActiveRow != null && (contactSet.Contact[0].IsStateIDNull() ||
                                                   (int)cmbState.ActiveRow.Cells["StateID"].Value !=
                                                   contactSet.Contact[0].StateID))
                {
                    // push change to dataset
                    contactSet.Contact[0].StateID = (int)cmbState.ActiveRow.Cells["StateID"].Value;
                    // auto-fill parent list
                    cmbCountry.Value = (int)cmbState.ActiveRow.Cells["CountryID"].Value;
                }
                else
                    // clear parent list
                    cmbCountry.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbCountry_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCountry.ActiveRow != null && (contactSet.Contact[0].IsCountryIDNull() ||
                                                     (int)cmbCountry.ActiveRow.Cells["CountryID"].Value !=
                                                     contactSet.Contact[0].CountryID))
                {
                    // push change to dataset
                    contactSet.Contact[0].CountryID = (int)cmbCountry.ActiveRow.Cells["CountryID"].Value;
                }
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }
        #endregion
    }
}
