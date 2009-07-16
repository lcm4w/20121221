using System;
using System.Configuration;
using System.Data.SqlClient;
using TourWriter.Utilities.Encryption;


namespace TourWriter.Info.Services
{
    public class ConnectionString
    {
        /// <summary>
        /// TODO: Remove this public call. We want just the internal calls below, 
        /// once all db access methods have been moved into this Info namespace.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString_UNSAFE()
        {
            // TODO: Remove this public call. We want just the internal calls below, 
            // once all db access methods have been moved into this Info namespace.
            System.Diagnostics.Debug.WriteLine("TODO: REMOVE THIS METHOD CALL!");
            return GetConnectionString();
        }
        
        private static string serverName = "";
        private const string DbEncUser = "PlBe8RoPBQg=";
        private const string DbEncPass = "2xGuduvmNtY=";

        internal static string GetConnectionString()
        {
            if (serverName == "")
                throw new ArgumentNullException(serverName, "Server name is not initialised in connection string.");

            return GetConnectionString(serverName);
        }

        internal static string GetConnectionString(string servername)
        {
            serverName = servername;
            string connString = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;
            var conn = new SqlConnectionStringBuilder(connString)
                           {
                               DataSource = serverName,
                               UserID = EncryptionHelper.DecryptString(DbEncUser),
                               Password = EncryptionHelper.DecryptString(DbEncPass)
                           };
            return conn.ToString();
        }

        internal static string GetSaConnectionString()
        {
            var conn = new SqlConnectionStringBuilder(GetConnectionString())
                           {
                               UserID = EncryptionHelper.DecryptString("xW4cPUIBeM8="),
                               Password = EncryptionHelper.DecryptString("29clmrFuhqDIu4G6BjWQxA==")
                           };

            if (TestConnection(conn.ConnectionString))
                return conn.ConnectionString;

            // try old pass
            conn.Password = EncryptionHelper.DecryptString("tExqtjylKmY=");
            return conn.ConnectionString;
        }

        private static bool TestConnection(string connectionString)
        {
            using (var c = new SqlConnection(connectionString))
            {
                try
                {
                    c.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
