using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Services
{
	/// <summary>
	/// Summary description for Permissions.
	/// </summary>
	public class AppPermissions
	{
		public enum Permissions
		{
			// these integer values must match exactly the data in TourWriter.Permission table 
			SupplierView			    = 1,
			SupplierEdit			    = 2,
			SupplierDelete			    = 3,
			ItineraryView			    = 4,
			ItineraryEdit			    = 5,
			ItineraryDelete			    = 6,
			ContactView				    = 7,
			ContactEdit				    = 8,
			ContactDelete			    = 9,
			UserAccounts			    = 10,
			DataLists				    = 11,
			Administration			    = 12,
			SupplierBankDetail		    = 13,
			ReportSuppierPurchases	    = 14,
			ReportClientLocations	    = 15,
			ReportItineraryYield	    = 16,
            DataExtractionItineraries   = 17,
            DataExtractionSuppliers     = 18,
            DataExtractionPayments      = 19,
            DataImport                  = 20,
            AccountingView              = 21,
            AccountingEdit              = 22,
            ItineraryReadOnly           = 23
		}

		
		internal static bool UserHasPermission(Permissions permission)
		{
            UserSet.UserRoleRow[] rows = (UserSet.UserRoleRow[])
		        Cache.UserSet.UserRole.Select(
		            string.Format("{0}={1}", Cache.UserSet.UserRole.UserIDColumn.ColumnName, Cache.User.UserID));
            
            if (rows != null)
            {
                foreach (UserSet.UserRoleRow row in rows)
                {
                    if (Cache.UserSet.RolePermission.FindByRoleIDPermissionID(row.RoleID, (int)permission) != null)
                    {
                        return true;
                    }
                }
            }
		    return false;
		}

	}
}
