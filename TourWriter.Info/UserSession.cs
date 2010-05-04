using System;
using System.Data;
using System.Data.SqlClient;
using TourWriter.Info.Services;

namespace TourWriter.Info
{
    public class UserSession
    {
        public static void AddOrUpdate(int userId, string computerName, int sessionTimeout, ref Guid sessionId, ref int sessionIndex, ref int sessionCount)
        {
            var param1 = new SqlParameter("@LoginID", SqlDbType.UniqueIdentifier);
            var param2 = new SqlParameter("@UserID", SqlDbType.Int);
            var param3 = new SqlParameter("@ComputerName", SqlDbType.VarChar);
            var param4 = new SqlParameter("@SessionTimeout", SqlDbType.Int);

            param1.Value = sessionId;
            param2.Value = userId;
            param3.Value = computerName;
            param4.Value = sessionTimeout;

            var dr = SqlHelper.ExecuteReader(ConnectionString.GetConnectionString(), "_Login_AddOrUpdate", param1, param2, param3, param4);

            if (dr.HasRows)
            {
                dr.Read();
                sessionId = dr.GetGuid(0);
                sessionIndex = Convert.ToInt32(dr.GetInt64(1));
                sessionCount = dr.GetInt32(2);
            }
        }

        public static void Quit(string computerName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString.GetConnectionString(), CommandType.Text,
                                      "update [Login] set Ended = 'true' where ComputerName = @ComputerName",
                                      new SqlParameter("@ComputerName", SqlDbType.VarChar) {Value = computerName});
        }
    }
}
