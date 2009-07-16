using System;
using System.Data;
using System.Data.SqlClient;
using TourWriter.Info.Services;

namespace TourWriter.Info
{
    public class UserSession
    {
        public DataSet GetSessions()
        {
            return SqlHelper.ExecuteDataset(
                ConnectionString.GetConnectionString(), "Login_Sel_All");
        }

        public Guid AddSession(Guid loginGuid, int userID, string computerName, int maxUsers)
        {
            SqlParameter param1 = new SqlParameter("@LoginID", SqlDbType.UniqueIdentifier);
            SqlParameter param2 = new SqlParameter("@UserID", SqlDbType.Int);
            SqlParameter param3 = new SqlParameter("@ComputerName", SqlDbType.VarChar);
            SqlParameter param4 = new SqlParameter("@MaxUsers", SqlDbType.Int);
            SqlParameter param5 = new SqlParameter("@NewGuid", SqlDbType.UniqueIdentifier);

            param1.Value = loginGuid;
            param2.Value = userID;
            param3.Value = computerName;
            param4.Value = maxUsers;
            param5.Direction = ParameterDirection.Output;

            object o = SqlHelper.ExecuteScalar(
                ConnectionString.GetConnectionString(), 
                "_User_CheckLogin", param1, param2, param3, param4, param5);

            if (o != DBNull.Value)
                return (Guid)o;
            return Guid.Empty;
        }

        public void RemoveSession(Guid loginGuid)
        {
            SqlParameter param1 = new SqlParameter("@LoginID", SqlDbType.UniqueIdentifier);
            param1.Value = loginGuid;

            SqlHelper.ExecuteScalar(
                ConnectionString.GetConnectionString()
                , "_User_RemoveLogin", param1);
        }
    }
}
