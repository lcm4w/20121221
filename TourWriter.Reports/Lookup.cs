using System;
using TourWriter.Info;

namespace TourWriter.Reports
{
	/// <summary>
	/// Summary description for Lookup.
	/// </summary>
	public class Lookup
	{
	    public const string Lic = "S Illingworth,Trio Software Development,DD-ARN-10-E000870,YXHM9B126KK6MX4ZP1BY";

		public static string GetSupplierCityName(int supplierID, ItinerarySet itinerarySet, ToolSet toolSet)
		{
			ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(supplierID);

			return GetSupplierCityName(supplier, toolSet);			
		}

		public static string GetSupplierCityName(ItinerarySet.SupplierLookupRow supplier, ToolSet toolSet)
		{
			if(!supplier.IsCityIDNull())
			{
				ToolSet.CityRow city = toolSet.City.FindByCityID(supplier.CityID);
				if(city != null)
					return city.CityName;
			}
			return "";
		}
		
		public static string GetServiceTypeName(int serviceTypeID, ToolSet toolSet)
		{
			ToolSet.ServiceTypeRow row = toolSet.ServiceType.FindByServiceTypeID(serviceTypeID);
			if(row != null)
				return row.ServiceTypeName;

			return "";
		}
		
		public static string GetArriveCity(ItinerarySet.ItineraryRow itinerary, ToolSet toolSet)
		{
			string s = "";
			if(itinerary.IsArriveCityIDNull())
				s += "(city not selected)";
			else
			{
				ToolSet.CityRow city = toolSet.City.FindByCityID(itinerary.ArriveCityID);
				s += (city != null) ? city.CityName : "(city not found)";
			}
			return s;
		}

		public static string GetDepartCity(ItinerarySet.ItineraryRow itinerary, ToolSet toolSet)
		{	
			string s = "";
			if(itinerary.IsDepartCityIDNull())
				s += "(city not selected)";
			else
			{
				ToolSet.CityRow city = toolSet.City.FindByCityID(itinerary.DepartCityID);
				s += city != null ? city.CityName : "(city not found)";
			}			
			return s;
		}
				

		/// <summary>
		/// Checks if a ServiceTypeId exists in the comma-separated list of Ids.
		/// </summary>
		public static bool ShowServiceTypeOnReport(object serviceTypeId, string serviceTypeIdList)
		{
            if (serviceTypeId != null && serviceTypeIdList.Length > 0)
            {
                foreach (string s in serviceTypeIdList.Split(','))
                    if (int.Parse(s) == int.Parse(serviceTypeId.ToString()))
                        return true;
            }
			return false;
		}



        internal static string ConvertToFullPath(string filePath, string externalFilesPath)
        {
            // copied from TourWriter.Services. New reports need to share this.

            // Is it a full network path?
            if (filePath.Length > 1 && filePath[1] == ':' && filePath[2] == '\\')// (filePath.StartsWith(@"\\"))
                return filePath;

            // Is it a full local or shared path?
            if (filePath.Length > 1 && filePath[1] == ':' && filePath[2] == '\\')
                return filePath;

            // subpath, so add external files path
            return externalFilesPath.TrimEnd('\\') + "\\" + filePath.TrimStart('\\');
        }


	}
}
