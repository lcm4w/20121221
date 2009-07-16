using System;
using System.Data;
using TourWriter.Info;
using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// User business logic layer, accesses the data layer.
	/// </summary>
	public class Itinerary : BLLBase
	{
		public ItinerarySet GetItinerarySet(int id, ItinerarySet ds)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.GetItinerarySet(id, ds);
				}				
				else if(TypeOfConnection == "WAN")
				{ }
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		public ItinerarySet SaveItinerarySet(ItinerarySet ds)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.SaveItinerarySet(ds);
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
		public int Create(string name, DateTime arriveDate, int parentFolderID, int userID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.Create(name, arriveDate, parentFolderID, userID);
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
		public int Copy(int origItineraryID, string newItineraryName, int userID)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.Copy(origItineraryID, newItineraryName, userID);
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
					ItineraryDB db = new ItineraryDB(ConnectionString);
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
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Delete(int id)
		{				
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.Delete(id);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return false;
		}


		public DataTable SearchServices(int? cityID, int? regionID, int? gradeID, int? serviceTypeID)
		{			
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ItineraryDB db = new ItineraryDB(ConnectionString);
					return db.SearchServices(cityID, regionID, gradeID, serviceTypeID);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
	}
}
