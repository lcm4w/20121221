using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using TourWriter.BusinessLogic;
using TourWriter.Info.Services;

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class Import : UserControl
    {
        #region Member variables
        private const string SqlInsertFormat = @"INSERT INTO [{0}] ({1}) VALUES ({2})";

        private readonly string tableName;
        private Dictionary<string, List<string>> columnLists;
        #endregion

        public Import(string tableName)
        {
            InitializeComponent();

            this.tableName = tableName;
            InitializeColumnLists();

            // fill in the table name
            lblExplain2.Text = String.Format(lblExplain2.Text, tableName);

            // set the default folder name
            txtFolderName.Text = "Imported " + DateTime.Today.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Initializes the column lists and the Data Type combo box.
        /// </summary>
        private void InitializeColumnLists()
        {
            columnLists = new Dictionary<string, List<string>>();

            // add the supplier columns
            List<string> supplierColumns = new List<string>();
            supplierColumns.Add("SupplierName");
            supplierColumns.Add("HostName");
            supplierColumns.Add("StreetAddress");
            supplierColumns.Add("PostAddress");
            supplierColumns.Add("Phone");
            supplierColumns.Add("Fax");
            supplierColumns.Add("Email");
            supplierColumns.Add("Website");
            supplierColumns.Add("Description");
            supplierColumns.Add("Comments");
            supplierColumns.Add("CancellationPolicy");
            columnLists.Add("Supplier", supplierColumns);

            // add the contact columns
            List<string> contactColumns = new List<string>();
            contactColumns.Add("ContactName");
            contactColumns.Add("Title");
            contactColumns.Add("FirstName");
            contactColumns.Add("LastName");
            contactColumns.Add("StreetAddress");
            contactColumns.Add("PostAddress");
            contactColumns.Add("WorkPhone");
            contactColumns.Add("HomePhone");
            contactColumns.Add("CellPhone");
            contactColumns.Add("Fax");
            contactColumns.Add("Email1");
            contactColumns.Add("Email2");
            contactColumns.Add("Website");
            contactColumns.Add("Notes");
            columnLists.Add("Contact", contactColumns);
        }

        /// <summary>
        /// Rename the grid columns so the user can see which order things should go.
        /// </summary>
        private void SetColumnNames()
        {
            for (int i = 0; i < gridData.Columns.Count &&  i < columnLists[tableName].Count; i++)
            {
                gridData.Columns[i].Name = columnLists[tableName][i];
                gridData.Columns[i].HeaderText = gridData.Columns[i].Name;
            }

            // remove columns from the end until there only exists columns that match the column list
            while (gridData.Columns.Count > columnLists[tableName].Count)
            {
                gridData.Columns.Remove(gridData.Columns[gridData.Columns.Count - 1]);
            }
        }

        /// <summary>
        /// Prompts the user to select an excel file.
        /// </summary>
        /// <returns>The selected filename.</returns>
        private static string ChooseFile()
        {
            string fileName = App.SelectExternalFile(
                false, "Choose file to import", "Excel (*.XLS)|*.XLS|All files (*.*)|*.*", 1);

            return fileName;
        }

        /// <summary>
        /// Loads data into the grid, from an excel file.
        /// </summary>
        /// <param name="fileName">The excel file to load.</param>
        private void LoadFromExcel(string fileName)
        {
            Cursor = Cursors.WaitCursor;
            gridData.DataSource = null;

            string connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                fileName + ";Extended Properties=Excel 8.0;";

            DataSet ds = new DataSet();

            using (OleDbConnection connection = new OleDbConnection())
            {
                connection.ConnectionString = connectString;
                connection.Open();

                // use the first sheet
                DataTable excelSheets = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                string sheetName = excelSheets.Rows[0]["TABLE_NAME"].ToString();

                string cmdString = String.Format("SELECT * FROM [{0}]", sheetName);
                OleDbDataAdapter command = new OleDbDataAdapter(cmdString, connection);
                command.Fill(ds);
            }

            gridData.DataSource = ds;
            gridData.DataMember = "Table";
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Saves the data from the grid to the database.
        /// </summary>
        private void SaveData()
        {
            if (gridData.Rows.Count < 0)
                return;

            Cursor = Cursors.WaitCursor;
            
            int? folderId = null;
            if (chkSaveToFolder.Checked && txtFolderName.Text.Length > 0)
                folderId = CreateFolder();

            using (SqlConnection conn = new SqlConnection(ConnectionString.GetConnectionString_UNSAFE()))
            {
                conn.Open();

                foreach (DataGridViewRow row in gridData.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    ExecuteSqlInsert(row, conn, folderId);
                }
            }

            Cursor = Cursors.Default;
            App.RefreshMenu(tableName);
            App.ShowInfo("Data saved successfully.");
        }

        /// <summary>
        /// Creates a new folder and returns the folder ID.
        /// </summary>
        /// <returns>The folder ID, or null if failure.</returns>
        private int? CreateFolder()
        {
            int menuTypeId;
            switch (tableName)
            {
                case "Supplier":
                    menuTypeId = 2;
                    break;
                case "Contact":
                    menuTypeId = 1;
                    break;
                default:
                    return null;
            }

            Folder f = new Folder();
            return f.NewFolder(txtFolderName.Text, 0, menuTypeId, TourWriter.Global.Cache.User.UserID);
        }

        /// <summary>
        /// Creates and executes a SQL insert command.
        /// </summary>
        /// <param name="row">The data grid row to get the data from.</param>
        /// <param name="conn">The SQL connection.</param>
        /// <param name="folderId">The ID of the folder to store the data in.</param>
        private void ExecuteSqlInsert(DataGridViewRow row, SqlConnection conn, int? folderId)
        {
            string sqlColumns = String.Empty;
            string sqlParams = String.Empty;

            SqlCommand cmd = new SqlCommand();

            foreach (string columnName in columnLists[tableName])
            {
                if (row.DataGridView.Columns.Contains(columnName))
                {
                    object value = row.Cells[columnName].Value;
                    if (value == null || value.ToString().Length == 0)
                        continue;

                    sqlColumns += String.Format("[{0}],", columnName);
                    sqlParams += String.Format("@{0},", columnName);
                    cmd.Parameters.AddWithValue("@" + columnName, value);
                }
            }

            if (sqlColumns.Length == 0)
                return;
            
            if (folderId != null)
            {
                // store the data in a new folder
                sqlColumns += "[ParentFolderID],";
                sqlParams += "@ParentFolderID,";
                cmd.Parameters.AddWithValue("@ParentFolderID", folderId);
            }

            // chop the ',' off the end of the string
            sqlColumns = sqlColumns.Remove(sqlColumns.Length - 1);
            sqlParams = sqlParams.Remove(sqlParams.Length - 1);
            
            cmd.CommandText = String.Format(SqlInsertFormat, tableName, sqlColumns, sqlParams);
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
        }

        #region Events
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string fileName = ChooseFile();
            if (fileName == null)
                return;

            LoadFromExcel(fileName);
            SetColumnNames();
            btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            btnSave.Enabled = false;
        }

        private void chkSaveToFolder_CheckedChanged(object sender, EventArgs e)
        {
            txtFolderName.Enabled = chkSaveToFolder.Checked;
        }
        #endregion
    }
}
