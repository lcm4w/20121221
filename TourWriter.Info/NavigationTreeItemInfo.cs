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
