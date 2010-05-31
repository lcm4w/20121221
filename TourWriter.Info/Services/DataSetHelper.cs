using System.Data;
using System.Data.SqlClient;

namespace TourWriter.Info.Services
{
	/// <summary>
	/// Implements helper methods for handling datasets.
	/// </summary>
	public class DataSetHelper
	{
		private DataSetHelper() {}

        /// <summary>
        /// Fills a data table with data from a stored procedure.
        /// </summary>
        /// <param name="table">The data table to fill.</param>
        /// <param name="storedProcName">The stored procedure to get the data.</param>
        /// <param name="parameters">The sql parameters for the stored proc.</param>
        /// <returns>The filled data table.</returns>
        public static DataTable FillDataTable(DataTable table, string storedProcName, params SqlParameter[] parameters)
        {
            var ds = table.DataSet;
            if (ds == null)
            {
                ds = new DataSet();
                ds.Tables.Add(table);
            }

            if (string.IsNullOrEmpty(ds.Tables[0].TableName))
                ds.Tables[0].TableName = "temp"; // set default tablename

            SqlHelper.FillDataset(ConnectionString.GetConnectionString(), CommandType.StoredProcedure,
                storedProcName, ds, new[] { ds.Tables[0].TableName }, parameters);

            return ds.Tables[0];
        }

        /// <summary>
        /// Load a dataset with sql result set.
        /// </summary>
        /// <param name="ds">The dataset to load with result set</param>
        /// <param name="sql">The sql text</param>
        /// <returns>The dataset with sql result set loaded in first table</returns>
        public static DataSet FillDataset(DataSet ds, string sql)
        {
            SqlHelper.FillDataset(ConnectionString.GetConnectionString(), CommandType.Text,
                                  sql, ds, new[] { "table1" }, null);
            return ds;
        }

        /// <summary>
        /// Fills a dataset with the result set from sql command text.
        /// </summary>
        /// <param name="sql">The sql text used to select the data.</param>
        /// <param name="parameters">The sql parameters.</param>
        /// <returns>Dataset containing the sql text result set.</returns>
        public static DataSet FillDataSetFromSql(string sql, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            SqlHelper.FillDataset(ConnectionString.GetConnectionString(), CommandType.Text,
                sql, ds, new []{"table1"}, parameters);
            return ds;
        }

        /// <summary>
        /// Fills a dataset with the result set from a stored procedure.
        /// </summary>
        /// <param name="storedProcName">The stored procedure used to select the data.</param>
        /// <returns>Dataset containing the stored procedure result set.</returns>
        public static DataSet FillDataSet(string storedProcName)
        {
            SqlCommand selCommand = new SqlCommand();
            selCommand.CommandText = storedProcName;
            selCommand.CommandType = CommandType.StoredProcedure;
            selCommand.Connection = new SqlConnection(ConnectionString.GetConnectionString());

            using (SqlDataAdapter da = new SqlDataAdapter(selCommand))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

		/// <summary>
		/// Fill dataset from stored proc, into auto table names. 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <returns>Filled dataset</returns>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet,
			string storedProcName)
		{
			string[] tableNames = new string[dataSet.Tables.Count]; 

			// get table names
			for (int i = 0; i < dataSet.Tables.Count; i++) 
				tableNames[i] = dataSet.Tables[i].TableName;

			// pass to overloaded method
			return FillDataSet(connectionString, dataSet, storedProcName, tableNames);
		}
		
		/// <summary>
		/// Fill dataset from stored proc, into the provided table names.
		/// </summary>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <param name="tableNames">Names to name filled tables as</param>
		/// <returns>Filled dataset</returns>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet, 
			string storedProcName, string[] tableNames)
		{
			using(SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlHelper.FillDataset(conn, CommandType.StoredProcedure, storedProcName, dataSet, tableNames);
			}
			return dataSet;
		}        
		
		/// <summary>
		/// Fill dataset from stored proc, into auto table names. 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <param name="param">Input parameter to select data on</param>
		/// <returns>Filled dataset</returns>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet, 
			string storedProcName, SqlParameter param)
		{
			string[] tableNames = new string[dataSet.Tables.Count]; 

			// get table names
			for (int i = 0; i < dataSet.Tables.Count; i++) 
				tableNames[i] = dataSet.Tables[i].TableName;

			// pass to overloaded method
			return FillDataSet(connectionString, dataSet, storedProcName, param, tableNames);
		}
		
		/// <summary>
		/// Fill dataset from stored proc, into the provided table names.
		/// </summary>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <param name="param">Input parameter to select data on</param>
		/// <param name="tableNames">Names to name filled tables as</param>
		/// <returns>Filled dataset</returns>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet, 
			string storedProcName, SqlParameter param, string[] tableNames)
		{
			using(SqlConnection conn = new SqlConnection(connectionString))
			{
				if(param == null)
					SqlHelper.FillDataset(
						conn, CommandType.StoredProcedure, storedProcName, dataSet, tableNames);
				else					
					SqlHelper.FillDataset(
						conn, CommandType.StoredProcedure, storedProcName, dataSet, tableNames, param);
			}
			return dataSet;
		}        
		
		/// <summary>
		/// Fill dataset from stored proc, into auto table names. 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <returns>Filled dataset</returns>
		/// <param name="commandParameters"></param>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet, 
			string storedProcName, params SqlParameter[] commandParameters)
		{
			string[] tableNames = new string[dataSet.Tables.Count]; 

			// get table names
			for (int i = 0; i < dataSet.Tables.Count; i++) 
				tableNames[i] = dataSet.Tables[i].TableName;

			// pass to overloaded method
			return FillDataSet(connectionString, dataSet, storedProcName, tableNames, commandParameters);
		}

		/// <summary>
		/// Fill dataset from stored proc, into the provided table names.
		/// </summary>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="dataSet">DataSet to fill</param>
		/// <param name="storedProcName">Stored proc used to get data</param>
		/// <param name="tableNames">Names to name filled tables as</param>
		/// <returns>Filled dataset</returns>
		/// <param name="commandParameters"></param>
		internal static DataSet FillDataSet(string connectionString, DataSet dataSet, 
			string storedProcName, string[] tableNames, params SqlParameter[] commandParameters)
		{
			using(SqlConnection conn = new SqlConnection(connectionString))
			{				
				SqlHelper.FillDataset(
					conn, CommandType.StoredProcedure, storedProcName, dataSet, tableNames, commandParameters);
			}
			return dataSet;
		}        
		
		/// <summary>
		/// Save data in a dataset, with optional list of tablenames 
		/// </summary>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="dataSet">Source dataset to save</param>
		/// <param name="tableNames">optional ordered list of tables to save, otherwise saves all in dataset</param>
		internal static void SaveDataSet(string connectionString, DataSet dataSet, 
			params string[] tableNames)
		{
			if(dataSet != null && dataSet.HasChanges())
			{
				using(SqlConnection conn = new SqlConnection(connectionString))
				{
					// supplied table names
					if((tableNames != null) && (tableNames.Length > 0)) 
					{
						for (var i=0; i<tableNames.Length; i++)
							SaveTable(conn, dataSet, tableNames[i]);
					}
					// use table names from dataset
					else 
					{
					    var changes = dataSet.GetChanges();
                        if (changes != null)
                        {
                            foreach (DataTable table in changes.Tables)
                                SaveTable(conn, dataSet, table.TableName);
                        }
					}
				}
			}
		}		

		/// <summary>
		/// Saves a data table using table-name default stored procedures. 
		/// </summary>
		/// <param name="conn">Sql connection</param>
		/// <param name="dataSet">Source dataset</param>
		/// <param name="tableName">Table to save</param>
		internal static void SaveTable(SqlConnection conn, DataSet dataSet, string tableName)
		{
			if(dataSet.Tables[tableName].GetChanges() != null)
			{
				SqlCommand insCmd = SqlHelper.CreateCommand(conn, tableName + "_Ins", true);
				SqlCommand delCmd = SqlHelper.CreateCommand(conn, tableName + "_Del", true);
				SqlCommand updCmd = SqlHelper.CreateCommand(conn, tableName + "_Upd", true);

				SqlHelper.UpdateDataset(insCmd, delCmd, updCmd, dataSet, tableName);
			}
		}

		/// <summary>
		/// Handles common DataSet update issues after merging fresh database data into 
		/// the dataset of changes; eg changes.Merge(fresh, true);
		/// </summary>
		/// <param name="ds"></param>
		internal static void ProcessRefreshedDataSet(DataSet ds)
		{
			foreach(DataTable dt in ds.Tables)
			{
				for(int i=0; i<dt.Rows.Count; i++) 
				{
					DataRow dr = dt.Rows[i];
					switch(dr.RowState)
					{
						case DataRowState.Deleted:
						{
							#region Cascaded delete errors
							// If a DataRow delete cascades to child rows in the DataSet, and the
							// database does the same on update when it deletes the parent row, then
							// the DataSet will fail (concurrency violation) when it attempts to delete 
							// the already deleted childrows.
							if(dr.HasErrors && dr.RowError.StartsWith("Concurrency violation:"))
							{
								// The fact that there is no Current version of this row after merging
								// fresh data means that it is no longer in the database anyway, 
								// so we can forget it.
								dr.AcceptChanges();
								dr.ClearErrors();
								i--; // reduce count to reflect rowcount change after AcceptChanges 
							}
							break;
							#endregion
						}
						case DataRowState.Modified:
						{	
							#region Clean where no errors
							// After merging the latest database data into the dataset containing the changes,
							// DataRowVersion.Original contains the refreshed data with the new RowVersion value.
							if(!dr.HasErrors)
								// Row was modified without error, so reject the now old changes in
								// DataRowVersion.Current which will bring forward the refreshed 
								// DataRowVersion.Original data.
								dr.RejectChanges();
							break;
							#endregion
						}
					}
				}
			}
		}
	}
}
