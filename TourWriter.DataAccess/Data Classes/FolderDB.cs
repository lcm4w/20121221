using System;

namespace TourWriter.DataAccess
{
	/// <summary>
	/// ItineraryDB class accesses the database Itinerary data.
	/// </summary>
	public class FolderDB
	{
		private string connectionString;

		public FolderDB(string connectionString)
		{
			this.connectionString = connectionString;
		}
		
		
		/// <summary>
		/// Deletes a Folder.
		/// </summary>
		/// <param name="id">Folder ID</param>
		/// <returns>True if delete accepted</returns>
		public bool DeleteFolder(int id)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "Folder_Del", id, null);
			return (int)o > 0;
		}
		
		/// <summary>
		/// Create a new Folder.
		/// </summary>
		/// <param name="id">Folder name</param>
		/// <returns>User ID</returns>
		public int NewFolder(string name, int parentFolderID, int menuTypeID, int userID)
		{
			object o = SqlHelper.ExecuteScalar(connectionString, 
				"_Folder_New", name, parentFolderID, menuTypeID, DateTime.Now, userID);

			return int.Parse(o.ToString());			
		}
		
		/// <summary>
		/// Update the Folder name.
		/// </summary>
		/// <param name="id">Folder name</param>
		public bool SetFolderName(int id, string name)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "_Folder_Rename", id, name);
			return (int)o > 0;
		}
	
		
		public bool SetParentFolderID(int id, int parentFolderID)
		{
			object o = SqlHelper.ExecuteNonQuery(connectionString, "_Folder_SetParentFolderID", id, parentFolderID);
			return (int)o > 0;
		}
	}
}