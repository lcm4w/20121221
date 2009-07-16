using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Modules.ContactModule.Email;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ContactModule
{
    public partial class CategorySelector : Form
    {
        public CategorySelector()
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            LoadContactCategories();
        }        

        private void LoadContactCategories()
        {
            treeCategories.Nodes.Clear();

            foreach (ToolSet.ContactCategoryRow row in Cache.ToolSet.ContactCategory)
            {
                UltraTreeNode node =
                    new UltraTreeNode(row.ContactCategoryID.ToString(), row.ContactCategoryName);

                treeCategories.Nodes.Add(node);
            }
        }

        private void OpenEmailForm()
        {
            string categoryIds = String.Empty;
            ContactSet.ContactDataTable table = new ContactSet.ContactDataTable();

            foreach (UltraTreeNode node in treeCategories.Nodes)
            {
                if (node.CheckedState == CheckState.Checked)
                    categoryIds += node.Key + ",";
            }

            if (categoryIds.Length > 0)
            {
                // chop the ',' off the end
                categoryIds = categoryIds.Remove(categoryIds.Length - 1);

                using (SqlConnection conn = new SqlConnection(ConnectionString.GetConnectionString_UNSAFE()))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format(
                        "SELECT DISTINCT ContactName,ISNULL(Title, ''),ISNULL(FirstName,''),ISNULL(LastName,''),ISNULL(Email1,'') " +
                        "FROM Contact " +
                        "WHERE ContactID IN (SELECT ContactID FROM ContactContactCategory WHERE ContactCategoryID IN ({0})) " +
                        "ORDER BY ContactName", categoryIds);
                    cmd.Connection = conn;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContactSet.ContactRow row = table.NewContactRow();
                        row.ContactName = reader.GetString(0);
                        row.Title = reader.GetString(1);
                        row.FirstName = reader.GetString(2);
                        row.LastName = reader.GetString(3);
                        row.Email1 = reader.GetString(4);

                        table.AddContactRow(row);
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                ContactEmailForm emailForm = new ContactEmailForm(table);
                emailForm.ShowDialog();
            }
            else
            {
                App.ShowWarning("There are no contacts in the selected categories.");
            }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            OpenEmailForm();
        }
    }
}
