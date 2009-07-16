using System.Collections.Generic;
using TourWriter.DataAccess;
using TourWriter.Info;

namespace TourWriter.BusinessLogic
{
    /// <summary>
    /// User business logic layer, accesses the data layer.
    /// </summary>
    public class Supplier : BLLBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SupplierSet GetSupplierSet(int id)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.GetSupplierSet(id);
                }
                else if (TypeOfConnection == "WAN")
                { }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public SupplierSet SaveSupplierSet(SupplierSet ds)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.SaveSupplierSet(ds);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
            }
            return null;
        }


        /// <summary>
        /// Gets the notes from a supplier. These notes can be used to populate booking voucher notes.
        /// </summary>
        /// <param name="id">Supplier ID</param>
        /// <returns>List of notes as string values</returns>
        public List<string> GetSupplierNotes(int id)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.GetSupplierNotes(id);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
            }
            return null;
        }


        public int Create(string name, int parentFolderID, int userID)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.Create(name, parentFolderID, userID);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
            }
            return -1;
        }


        public int Copy(int origSupplierID, string newSupplierName, int userID)
        {
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.Copy(origSupplierID, newSupplierName, userID);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
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
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.Rename(id, name);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
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
            if (GetConnectionString())
            {
                if (TypeOfConnection == "LAN")
                {
                    SupplierDB db = new SupplierDB(ConnectionString);
                    return db.Delete(id);
                }
                else if (TypeOfConnection == "WAN")
                {
                }
            }
            return false;
        }
    }
}