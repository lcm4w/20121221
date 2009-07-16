using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// User business logic layer, accesses the data layer.
	/// </summary>
	public class Folder : BLLBase
	{
		public bool DeleteFolder(int id)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					FolderDB db = new FolderDB(ConnectionString);
					return db.DeleteFolder(id);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int NewFolder(string name, int parentFolderID, int menuTypeID, int userID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					FolderDB db = new FolderDB(ConnectionString);
					return db.NewFolder(name, parentFolderID, menuTypeID, userID);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return -1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool SetFolderName(int id, string name)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					FolderDB db = new FolderDB(ConnectionString);
					return db.SetFolderName(id, name);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}
				

		public bool SetParentFolderID(int id, int parentFolderID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					FolderDB db = new FolderDB(ConnectionString);
					return db.SetParentFolderID(id, parentFolderID);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}
	}
}
