using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TourWriter.Info;

namespace TourWriter.DataAccess
{
	/// <summary>
	/// SupplierDB class accesses the database Supplier data.
	/// </summary>
	public class SupplierDB
	{
		private string connectionString;
		public SupplierDB(string connectionString)
		{
			this.connectionString = connectionString;
		}


		/// <summary>
		/// Fills a SupplierSet with data based on the input 'ID' parameter.
		/// </summary>
		/// <param name="id">Supplier ID</param>
		/// <returns></returns>
		public SupplierSet GetSupplierSet(int id)
		{
			SupplierSet ds = new SupplierSet();
			SqlParameter param = new SqlParameter("@SupplierID", id);

			ds = (SupplierSet)DataSetHelper.FillDataSet(connectionString, ds, "_SupplierSet_Sel_ByID", param);

			// save input parameter in ExtendedProperties for later handling
			ds.ExtendedProperties.Add("SupplierID", param.Value.ToString());

			return ds;
		}
		
		/// <summary>
		/// Saves changes to a SupplierSet, updates any row errors, and refreshes the data
		/// based on the ExtendedProperties 'ID' key.
		/// </summary>
		/// <param name="ds">SupplierSet with changes</param>
		/// <returns>Refreshed SupplierSet</returns>
		public SupplierSet SaveSupplierSet(SupplierSet ds)
		{	
			// save changes
			SupplierSet changes = ds;
			DataSetHelper.SaveDataSet(connectionString, changes);
			
			// get id
			int id = changes.Supplier.Rows.Count > 0 ? changes.Supplier[0].SupplierID :
				int.Parse(ds.ExtendedProperties["SupplierID"].ToString());

			// refresh data
			SupplierSet fresh = GetSupplierSet(id);
			changes.Merge(fresh, true);
			DataSetHelper.ProcessRefreshedDataSet(changes);

			return changes;
		}
	    
	    /// <summary>
	    /// Gets the notes from a supplier. These notes can be used to populate booking voucher notes.
	    /// </summary>
	    /// <param name="id">Supplier ID</param>
	    /// <returns>List of notes as string values</returns>
	    public List<string> GetSupplierNotes(int id)
	    {
            SqlDataReader rd = SqlHelper.ExecuteReader(
                connectionString, "_Supplier_Notes", new SqlParameter("@SupplierID", id));

			List<string> list = new List<string>();
	        
			while (rd.Read())
				if(!rd.IsDBNull(0)) 
				    list.Add(rd.GetString(0));
	        
			return list;
	    }

		
		/// <summary>
		/// Create a new Supplier.
		/// </summary>
		public int Create(string name, int parentFolderID, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_Supplier_New", name, parentFolderID, DateTime.Now, userID);

			return int.Parse(o.ToString());
		}
		
		/// <summary>
		/// Copies a Supplier and associated records.
		/// </summary>
		public int Copy(int origID, string newName, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_SupplierSet_Copy_ByID", origID, newName, DateTime.Now, userID);

			return int.Parse(o.ToString());
		}
		
		/// <summary>
		/// Update the supplier name.
		/// </summary>
		/// <param name="id">Supplier name</param>
		public bool Rename(int id, string name)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "_Supplier_Rename", id, name);
			return (int)o > 0;
		}
		
		/// <summary>
		/// Deletes a Supplier and associated records.
		/// </summary>
		/// <param name="id">Supplier ID</param>
		/// <returns>True if delete accepted</returns>
		public bool Delete(int id)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "Supplier_Del", id, null);
			return (int)o > 0;
		}
				
	}
}