using System;
using System.Data.SqlClient;
using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// Reports business logic layer, accesses the data layer.
	/// </summary>
	public class Reports : BLLBase
	{
		public SqlDataReader ClientLocations(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ReportsDB db = new ReportsDB(ConnectionString);
					return db.ClientLocations(dateFrom, dateTo, itineraryStatusList, requestStatusList);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

		public SqlDataReader SupplierPurchases(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ReportsDB db = new ReportsDB(ConnectionString);
					return db.SupplierPurchases(dateFrom, dateTo, itineraryStatusList, requestStatusList);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

		public SqlDataReader ItineraryYield(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string userList, string branchList, string departmentList)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ReportsDB db = new ReportsDB(ConnectionString);
					return db.ItineraryYield(dateFrom, dateTo, itineraryStatusList, userList, branchList, departmentList);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

        public SqlDataReader ItineraryExport(DateTime dateFrom, DateTime dateTo)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    ReportsDB db = new ReportsDB(ConnectionString);
                    return db.ItineraryExport(dateFrom, dateTo);
                }
                else if (TypeOfConnection == "WAN")
                { }
            }
            return null;
        }

		public SqlDataReader WhoUsedSupplier(int supplierID, DateTime dateFrom, DateTime dateTo)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ReportsDB db = new ReportsDB(ConnectionString);
					return db.WhoUsedSupplier(supplierID, dateFrom, dateTo);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
	}
}
