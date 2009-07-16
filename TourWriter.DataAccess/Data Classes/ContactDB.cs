using System;
using System.Data.SqlClient;
using TourWriter.Info;

namespace TourWriter.DataAccess
{
	/// <summary>
	/// ContactDB class accesses the database Contact data.
	/// </summary>
	public class ContactDB
	{
		private string connectionString;
		public ContactDB(string connectionString)
		{
			this.connectionString = connectionString;
		}


		/// <summary>
		/// Fills a ContactSet with data based on the input 'ID' parameter.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ContactSet GetContactSet(int id)
		{
			ContactSet ds = new ContactSet();
			SqlParameter param = new SqlParameter("@ContactID", id);

			ds = (ContactSet)DataSetHelper.FillDataSet(connectionString, ds, "_ContactSet_Sel_ByID", param);

			// save input parameter in ExtendedProperties for later handling
			ds.ExtendedProperties.Add("ContactID", param.Value.ToString());

			return ds;
		}
		
		/// <summary>
		/// Saves changes to a ContactSet, updates any row errors, and refreshes the data
		/// based on the ExtendedProperties 'ID' key.
		/// </summary>
		/// <param name="ds">ContactSet with changes</param>
		/// <returns>Refreshed ContactSet</returns>
		public ContactSet SaveContactSet(ContactSet ds)
		{		
			// save changes
			ContactSet changes = (ContactSet)ds.GetChanges();
			DataSetHelper.SaveDataSet(connectionString, changes);

            // get id
            int id = changes.Contact.Rows.Count > 0 ? changes.Contact[0].ContactID :
                int.Parse(ds.ExtendedProperties["ContactID"].ToString());

			// get fresh data
			ContactSet fresh = GetContactSet(id);
			changes.Merge(fresh, true);
			DataSetHelper.ProcessRefreshedDataSet(changes);

			return changes;

		}
		
		
		/// <summary>
		/// Create a new Contact.
		/// </summary>
		/// <param name="id">Contact name</param>
		/// <returns>User ID</returns>
		public int Create(string name, int parentFolderID, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_Contact_New", name, parentFolderID, DateTime.Now, userID);

			return int.Parse(o.ToString());
		}
				
		/// <summary>
		/// Copies a Contact and associated records.
		/// </summary>
		/// <param name="id">Contact ID</param>
		/// <returns>New Contact ID</returns>
		public int Copy(int origID, string newName, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_ContactSet_Copy_ByID", origID, newName, DateTime.Now, userID);
                 			
			return int.Parse(o.ToString());
		}

		/// <summary>
		/// Update the Contact name.
		/// </summary>
		/// <param name="id">Contact name</param>
		public bool Rename(int id, string name)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "_Contact_Rename", id, name);
			return (int)o > 0;
		}
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactID"></param>
        /// <returns></returns>
		public bool Delete(int contactID)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "Contact_Del", contactID, null);
			return (int)o > 0;
		}

	}
}