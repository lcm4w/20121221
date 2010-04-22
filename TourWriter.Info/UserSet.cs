using System.Data.SqlClient;
using TourWriter.Info.Services;

namespace TourWriter.Info 
{
    partial class UserSet
    {
        // Authenticates to default database server using the server name.
        public void AuthenticateLocal(string serverName, string username, string password)
        {
            ConnectionString.SetLocalConnectionString(serverName);
            Authenticate(username, password);
        }

        // Authenticates to a database server using the full connection string.
        public void AuthenticateRemote(string connectionString, string username, string password)
        {
            ConnectionString.SetRemoteConnectionString(connectionString);
            Authenticate(username, password);
        }

        private void Authenticate(string username, string password)
        {
            var param1 = new SqlParameter("@Username", username);
            var param2 = new SqlParameter("@Password", password);

            Clear();
            DataSetHelper.FillDataSet(ConnectionString.GetConnectionString(), this, "_UserSet_Sel_ByUsernamePassword", param1, param2);
        }

        public void LoadSingle(int id)
        {
            var param = new SqlParameter("@UserID", id);

            Clear();
            DataSetHelper.FillDataSet(ConnectionString.GetConnectionString(), this, "_UserSet_Sel_ByID", param);
        }

        public void LoadAll()
        {
            Clear();
            DataSetHelper.FillDataSet(
                ConnectionString.GetConnectionString(), this, "_UserSet_Sel_All");
        }

        public void Save()
        {
            int singleUserId = -1;
            bool isSingleUser = User.Count == 1;

            if(isSingleUser)
                singleUserId = User[0].UserID;

            var changes = GetChanges() as UserSet;

            DataSetHelper.SaveDataSet(ConnectionString.GetConnectionString(), changes);
            if (isSingleUser)
                LoadSingle(singleUserId);
            else
                LoadAll();

            DataSetHelper.ProcessRefreshedDataSet(this);
        }	
    }
}
