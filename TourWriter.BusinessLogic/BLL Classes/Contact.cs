using TourWriter.Info;
using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// User business logic layer, accesses the data layer.
	/// </summary>
	public class Contact : BLLBase
	{
		public ContactSet GetContactSet(int id)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.GetContactSet(id);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

		public ContactSet SaveContactSet(ContactSet ds)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.SaveContactSet(ds);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int Create(string name, int parentFolderID, int userID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.Create(name, parentFolderID, userID);
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
		public int Copy(int origContactID, string newContactName, int userID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.Copy(origContactID, newContactName, userID);
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
		public bool Rename(int id, string name)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.Rename(id, name);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}
				
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactID"></param>
		/// <returns></returns>
		public bool Delete(int contactID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ContactDB db = new ContactDB(ConnectionString);
					return db.Delete(contactID);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}

	}
}
