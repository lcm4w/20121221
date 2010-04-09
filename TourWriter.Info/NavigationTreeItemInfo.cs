using System.Collections;
using System.Data;
using System.Data.SqlClient;
using TourWriter.Info.Services;

namespace TourWriter.Info
{
	public class NavigationTreeItemInfo
	{
		public enum ItemTypes
		{
			Folder    = 0,
			Itinerary = 1,
			Supplier  = 2,
			Contact   = 3,
            PurchaseLine = 4
		}

		public int       ItemID;
		public string    ItemName;
		public ItemTypes ItemType;
		public int       ParentFolderID;
		public bool      IsActive;
        
        #region Menu item
        public NavigationTreeItemInfo()
		{
		}

		public NavigationTreeItemInfo(int itemID, string itemName, ItemTypes itemType, int parentFolderID, bool isActive)
		{
			ItemID         = itemID;
			ItemName       = itemName;
			ItemType       = itemType;
			ParentFolderID = parentFolderID;
			IsActive       = isActive;
		}
        		
		/// <summary>
		/// Create an copy of this NavigationTreeItemInfo object
		/// </summary>
		/// <returns></returns>
		public NavigationTreeItemInfo Copy()
		{
			NavigationTreeItemInfo info = new NavigationTreeItemInfo();

			info.ItemID         = ItemID;
			info.ItemName       = ItemName;
			info.ItemType       = ItemType;
			info.ParentFolderID = ParentFolderID;
			info.IsActive       = IsActive;

			return info;
		}

		public static ItemTypes GetItemType_FromString(string itemType)
		{			
			if(itemType.Equals(ItemTypes.Itinerary.ToString()))
				return ItemTypes.Itinerary;
			
			if(itemType.Equals(ItemTypes.Supplier.ToString()))
				return ItemTypes.Supplier;
			
			if(itemType.Equals(ItemTypes.Contact.ToString()))
				return ItemTypes.Contact;

			return ItemTypes.Folder;
        }
        #endregion

        #region Menu items

        /// <summary>
        /// Gets child items of a folder in the menu tree.
        /// </summary>
        public NavigationTreeItemInfo[] GetChildList(int parentFolderID, ItemTypes itemType)
        {
            string s = ConnectionString.GetConnectionString();
            SqlDataReader dr = SqlHelper.ExecuteReader(s,
                "_Menu_GetChildList", parentFolderID, itemType.ToString());

            return BuildMenuItemInfoArray(dr);
        }

        /// <summary>
        /// Gets child items of a folder plus items of the preceding levels in the menu tree.
        /// </summary>
        public NavigationTreeItemInfo[] GetChildListAndParentTree(int parentFolderID, ItemTypes itemType)
        {
            SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionString.GetConnectionString(),
                "_Menu_GetChildListAndParentTree", parentFolderID, itemType.ToString());

            return BuildMenuItemInfoArray(dr);
        }

        /// <summary>
        /// Set the parent folder for a menu item.
        /// </summary>
        public bool SetParentFolderID(int itemID, int parentFolderID, ItemTypes itemType)
        {
            object o = SqlHelper.ExecuteNonQuery(ConnectionString.GetConnectionString(), "_Menu_SetParentFolderID", itemID, parentFolderID, itemType.ToString());
            ParentFolderID = parentFolderID;
            return (int)o > 0;
        }
        
        /// <summary>
        /// Gets child items of a folder plus items of the preceding levels in the menu tree.
        /// </summary>
        public DataSet Search(string phrase, ItemTypes itemType)
        {
            DataSet ds = new DataSet();

            SqlParameter param1 = new SqlParameter("@Phrase", phrase);
            SqlParameter param2 = new SqlParameter("@Table", itemType.ToString());

            if (itemType == ItemTypes.Itinerary)
            {
                const string sql =
@"select ItineraryID [ID], ItineraryName [Name], ArriveDate, ItineraryStatusName [Status], CountryName [Origin], UserName [AssignedTo], DepartmentName [Department],
ParentFolderID, i.IsRecordActive [IsActive], i.AddedOn
from Itinerary i
left join ItineraryStatus s on i.ItineraryStatusID = s.ItineraryStatusID
left join Country c on i.CountryID = c.CountryID
left join [User] u on i.AssignedTo = u.UserID
left join Department d on i.DepartmentId = d.DepartmentId
where ItineraryName like @phrase + '%' and (IsDeleted = 0 or IsDeleted is null)";

                ds = DatabaseHelper.ExecuteDataset(sql, new SqlParameter("@phrase", phrase));
            }
            else if (itemType == ItemTypes.Supplier)
            {
                const string sql =
@"select SupplierID [ID], SupplierName [Name], CityName [City], CountryName [Country], ParentFolderID, IsRecordActive [IsActive], AddedOn 
from Supplier s
left join City c on s.CityID = c.CityID
left join Country t on s.CountryID = t.CountryID
where SupplierName like @phrase + '%' and (IsDeleted = 0 or IsDeleted is null)";

                ds = DatabaseHelper.ExecuteDataset(sql, new SqlParameter("@phrase", phrase));
            }
            else if (itemType == ItemTypes.Contact)
            {
                const string sql =
@"select ContactId [ID], ContactName [Name], CityName [City], CountryName [Country], ParentFolderID, IsRecordActive [IsActive], AddedOn 
from Contact co
left join City ci on co.CityID = ci.CityID
left join Country cn on co.CountryID = cn.CountryID
where ContactName like @phrase + '%' and (IsDeleted = 0 or IsDeleted is null)";

                ds = DatabaseHelper.ExecuteDataset(sql, new SqlParameter("@phrase", phrase));
            }
            else
            {
                ds = DataSetHelper.FillDataSet(
                    ConnectionString.GetConnectionString(), ds, "_Menu_Search", param1, param2);
            }
            return ds;
        }

        /// <summary>
        /// Gets child items of a folder plus items of the preceding levels in the menu tree.
        /// </summary>
        public DataSet SearchByID(int id, ItemTypes itemType)
        {
            DataSet ds = new DataSet();

            if (itemType == ItemTypes.PurchaseLine)
            {
                SqlParameter param = new SqlParameter("@PurchaseLineId", id);

                ds = DataSetHelper.FillDataSet(ConnectionString.GetConnectionString(), ds, "_Search_PurchaseLine_ByID", param);
            }
            else
            {
                SqlParameter param1 = new SqlParameter("@Id", id);
                SqlParameter param2 = new SqlParameter("@Table", itemType.ToString());

                ds = DataSetHelper.FillDataSet(
                    ConnectionString.GetConnectionString(), ds, "_Menu_Search_ByID", param1, param2);
            }

            return ds;
        }

        private static NavigationTreeItemInfo[] BuildMenuItemInfoArray(SqlDataReader dr)
        {
            ArrayList ar = new ArrayList();
            while (dr.Read())
            {
                NavigationTreeItemInfo item = new NavigationTreeItemInfo();

                item.ItemID = dr.GetInt32(0);
                item.ItemName = dr.GetString(1);
                item.ParentFolderID = dr.GetInt32(2);
                item.IsActive = dr.GetBoolean(3);
                item.ItemType = GetItemType_FromString(dr.GetString(4));

                ar.Add(item);
            }
            return (NavigationTreeItemInfo[])ar.ToArray(typeof(NavigationTreeItemInfo));
        }

        #endregion
    }
}
