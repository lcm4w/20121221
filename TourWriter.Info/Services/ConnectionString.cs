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
        
        private static string _connectionString = "";

        // Sets the default connection string by building it up using the supplied server name and other default settings.
        internal static void SetLocalConnectionString(string servername)
        {
            var connString = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;
            var conn = new SqlConnectionStringBuilder(connString)
            {
                DataSource = servername,
                UserID = EncryptionHelper.DecryptString("PlBe8RoPBQg="),
                Password = EncryptionHelper.DecryptString("2xGuduvmNtY=")
            };
            _connectionString = conn.ToString();
        }
        
        internal static void SetRemoteConnectionString(string connectionString)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(connectionString, @"tw_\w{8}:"))
            {
                _connectionString = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(connectionString.Remove(0, 12)));
                return;
            }
            var conn = new SqlConnectionStringBuilder(connectionString);
            try { conn.DataSource = EncryptionHelper.DecryptString(conn.DataSource); } catch {}
            try { conn.UserID = EncryptionHelper.DecryptString(conn.UserID); } catch {}
            try { conn.Password = EncryptionHelper.DecryptString(conn.Password); } catch {}
            _connectionString = conn.ToString();
        }

        internal static string GetConnectionString()
        {
            if (_connectionString == "")
                throw new ArgumentNullException(_connectionString, "Database connection string is not initialized.");

            return _connectionString;
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
