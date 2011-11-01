using System.Data.SqlClient;
using TourWriter.Info;

namespace TourWriter.DataAccess
{
	/// <summary>
	/// ToolsDB class accesses the database Tool tools data.
	/// </summary>
	public class AgentDB
	{
		private string connectionString;
		public AgentDB(string connectionString)
		{
			this.connectionString = connectionString;
		}


		public AgentSet GetAgentSet()
		{ 
			AgentSet ds = new AgentSet();
			return (AgentSet)DataSetHelper.FillDataSet(connectionString, ds, "_AgentSet_Sel_All");			
		}

		/// <summary>
		/// Fills an AgentSet with data based on the input 'ID' parameter.
		/// </summary>
		/// <param name="id">Agent ID</param>
		/// <returns></returns>
		public AgentSet GetAgentSet(int id)
		{
			AgentSet ds = new AgentSet();
			SqlParameter param = new SqlParameter("@AgentID", id);

			ds = (AgentSet)DataSetHelper.FillDataSet(connectionString, ds, "_AgentSet_Sel_ByID", param);

			// save input parameter in ExtendedProperties for later handling
			ds.ExtendedProperties.Add("AgentID", param.Value.ToString());

			return ds;
		}
        
		/// <summary>
		/// Saves changes to a AgentSet, updates any row errors, and refreshes the data
		/// based on the ExtendedProperties 'ID' key.
		/// </summary>
		/// <param name="ds">AgentSet with changes</param>
		/// <returns>Refreshed AgentSet</returns>
		public AgentSet SaveAgentSet(AgentSet ds)
		{	
			// save changes
			AgentSet changes = (AgentSet)ds.GetChanges();
			DataSetHelper.SaveDataSet(connectionString, changes);

			// get fresh data
		    AgentSet fresh;
            if(ds.ExtendedProperties["AgentID"] != null)
            {
                fresh = GetAgentSet(int.Parse(ds.ExtendedProperties["AgentID"].ToString()));   
            }
            else
            {
			    fresh = GetAgentSet();
            }

            // process				
			changes.Merge(fresh, true);
			DataSetHelper.ProcessRefreshedDataSet(changes);

			return changes;
		}
		
	}
}