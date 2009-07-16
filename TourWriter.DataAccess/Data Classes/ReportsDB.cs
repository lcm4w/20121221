using System;
using System.Data.SqlClient;

namespace TourWriter.DataAccess
{
	public class ReportsDB
	{
		private string connectionString;
		public ReportsDB(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public SqlDataReader ClientLocations(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
			return SqlHelper.ExecuteReader(
				connectionString, "_Report_ClientLocations", dateFrom, dateTo, itineraryStatusList, requestStatusList);
		}

		public SqlDataReader SupplierPurchases(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
			return SqlHelper.ExecuteReader(
				connectionString, "_Report_SuppilerPurchases", dateFrom, dateTo, itineraryStatusList, requestStatusList);
		}

	    public SqlDataReader ItineraryYield(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string userList, string branchList, string departmentList)
		{
			return SqlHelper.ExecuteReader(
				connectionString, "_Report_ItineraryYield", dateFrom, dateTo, itineraryStatusList, userList, branchList, departmentList);
        }

        public SqlDataReader ItineraryExport(DateTime dateFrom, DateTime dateTo)
        {
            return SqlHelper.ExecuteReader(
                connectionString, "_DataExtract_ItineraryFinancials", dateFrom, dateTo);
        }

		public SqlDataReader WhoUsedSupplier(int supplierID, DateTime dateFrom, DateTime dateTo)
		{
			return SqlHelper.ExecuteReader(
				connectionString, "_Report_WhoUsedSupplier", supplierID, dateFrom, dateTo);
		}
		
	}
}