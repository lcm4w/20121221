using System.Configuration;
using System.Data.SqlClient;
using TourWriter.Utilities.Encryption;

namespace TourWriter.Tests
{
    class TestHelpers
    {
        internal static string GetLocalConnectionString(string servername)
        {
            var connString = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;
            var conn = new SqlConnectionStringBuilder(connString) {
                               DataSource = servername,
                               UserID = EncryptionHelper.DecryptString("PlBe8RoPBQg="),
                               Password = EncryptionHelper.DecryptString("2xGuduvmNtY=")
                           };
            return conn.ToString();
        }
    }
}
