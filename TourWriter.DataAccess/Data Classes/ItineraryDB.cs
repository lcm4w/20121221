using System;
using System.Data;
using System.Data.SqlClient;
using TourWriter.Info;

namespace TourWriter.DataAccess
{
	/// <summary>
	/// ItineraryDB class accesses the database Itinerary data.
	/// </summary>
	public class ItineraryDB
	{
		private string connectionString;
		public ItineraryDB(string connectionString)
		{
			this.connectionString = connectionString;
		}
		

		/// <summary>
		/// Fills a ItinerarySet with data based on the input 'ID' parameter.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        public ItinerarySet GetItinerarySet(int id, ItinerarySet ds)
		{
			//ItinerarySet ds = new ItinerarySet();
			SqlParameter param = new SqlParameter("@ItineraryID", id);

			ds = (ItinerarySet)DataSetHelper.FillDataSet(connectionString, ds, "_ItinerarySet_Sel_ByID", param);

			// save input parameter in ExtendedProperties for later handling
			ds.ExtendedProperties.Add("ItineraryID", param.Value.ToString());

			return ds;
		}
		
		/// <summary>
		/// Saves changes to a ItinerarySet, updates any row errors, and refreshes the data
		/// based on the ExtendedProperties 'ID' key.
		/// </summary>
		/// <param name="ds">ItinerarySet with changes</param>
		/// <returns>Refreshed ItinerarySet</returns>
		public ItinerarySet SaveItinerarySet(ItinerarySet ds)
		{
			// don't persist lookup tables
			ds.SupplierLookup.AcceptChanges();
			ds.OptionLookup.AcceptChanges();
            ds.ServiceWarning.AcceptChanges();

			ItinerarySet changes = (ItinerarySet)ds.GetChanges();
			if(changes == null)
			    changes = ds; // ensure change not null after accepting changes above					

			// save changes
			DataSetHelper.SaveDataSet(connectionString, changes);
			
			// get id
			int id = changes.Itinerary.Rows.Count > 0 ? changes.Itinerary[0].ItineraryID :
				int.Parse(ds.ExtendedProperties["ItineraryID"].ToString());
					
			// refresh data
			ItinerarySet fresh = GetItinerarySet(id, new ItinerarySet());
			changes.Merge(fresh, true);
			DataSetHelper.ProcessRefreshedDataSet(changes);

			return changes;
		}
		

		/// <summary>
		/// Create a new Itinerary.
		/// </summary>
		/// <param name="id">Itinerary name</param>
		/// <returns>User ID</returns>
		public int Create(string name, DateTime arriveDate, int parentFolderID, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_Itinerary_New", name, arriveDate, parentFolderID, DateTime.Now, userID);

			return int.Parse(o.ToString());
		}
		
		/// <summary>
		/// Copies a Itinerary and associated records.
		/// </summary>
		/// <param name="id">Itinerary ID</param>
		/// <returns>New Itinerary ID</returns>
		public int Copy(int origID, string newName, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_ItinerarySet_Copy_ByID", origID, newName, DateTime.Now, userID);
                 			
			return int.Parse(o.ToString());
		}

		/// <summary>
		/// Update the Itinerary name.
		/// </summary>
		/// <param name="id">Itinerary name</param>
		public bool Rename(int id, string name)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "_Itinerary_Rename", id, name);
			return (int)o > 0;
		}
	
		/// <summary>
		/// Deletes a Itinerary and associated records.
		/// </summary>
		/// <param name="id">Itinerary ID</param>
		/// <returns>True if delete accepted</returns>
		public bool Delete(int id)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "Itinerary_Del", id, null);
			return (int)o > 0;
		}
		

		public DataTable SearchServices(int? cityID, int? regionID, int? gradeID, int? serviceTypeID)
		{
			// sp input
            SqlDataReader rd = SqlHelper.ExecuteReader(
                connectionString, CommandType.StoredProcedure, "_Itinerary_ServiceSearch",
                cityID.HasValue ? new SqlParameter("@CityID", cityID) : null,
                regionID.HasValue ? new SqlParameter("@RegionID", regionID) : null,
                gradeID.HasValue ? new SqlParameter("@GradeID", gradeID) : null,
                serviceTypeID.HasValue ? new SqlParameter("@ServiceTypeID", serviceTypeID) : null);

			// sp output
			DataTable dt = new DataTable();
			dt.Columns.AddRange(new DataColumn[]
				{
					new DataColumn("SupplierID",	typeof(int)),
					new DataColumn("ServiceID",		typeof(int)),
					new DataColumn("SupplierName",	typeof(string)),
					new DataColumn("ServiceName",	typeof(string)),
					new DataColumn("ServiceTypeID",	typeof(string)),
					new DataColumn("CityID",		typeof(string)),
					new DataColumn("GradeID",		typeof(string))
				} );
			
			while (rd.Read())
			{
				DataRow dr = dt.NewRow();				
				if(!rd.IsDBNull(0)) dr[0] = rd.GetInt32 (0);
				if(!rd.IsDBNull(1)) dr[1] = rd.GetInt32 (1);
				if(!rd.IsDBNull(2)) dr[2] = rd.GetString(2);
				if(!rd.IsDBNull(3)) dr[3] = rd.GetString(3);
				if(!rd.IsDBNull(4)) dr[4] = rd.GetInt32 (4).ToString();
				if(!rd.IsDBNull(5)) dr[5] = rd.GetInt32 (5).ToString();
				if(!rd.IsDBNull(6)) dr[6] = rd.GetInt32 (6).ToString();				
				dt.Rows.Add(dr);
			}
			return dt;
		}
	}
}