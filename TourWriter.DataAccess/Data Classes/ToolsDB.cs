using TourWriter.Info;

namespace TourWriter.DataAccess
{
    /// <summary>
    /// ToolsDB class accesses the database Tool tools data.
    /// </summary>
    public class ToolsDB
    {
        private string connectionString;

        public ToolsDB(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public ToolSet GetToolSet()
        {
            ToolSet ds = new ToolSet();
            return (ToolSet) DataSetHelper.FillDataSet(connectionString, ds, "_ToolSet_Sel_All");
        }

        /// <summary>
        /// Saves changes to a ToolSet, updates any row errors, and refreshes the data
        /// based on the ExtendedProperties 'ID' key.
        /// </summary>
        /// <param name="ds">ToolSet with changes</param>
        /// <returns>Refreshed ToolSet</returns>
        public ToolSet SaveToolSet(ToolSet ds)
        {
            // save changes
            ToolSet changes = (ToolSet) ds.GetChanges();
            DataSetHelper.SaveDataSet(connectionString, changes);

            // get fresh data
            ToolSet fresh = GetToolSet();
            changes.Merge(fresh, true);
            DataSetHelper.ProcessRefreshedDataSet(changes);

            return changes;
        }
    }
}