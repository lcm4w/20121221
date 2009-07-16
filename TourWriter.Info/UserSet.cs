using System.Data.SqlClient;
using TourWriter.Info.Services;

namespace TourWriter.Info 
{
    partial class UserSet
    {
        public void LoadSingle(string serverName, string username, string password)
        {
            SqlParameter param1 = new SqlParameter("@Username", username);
            SqlParameter param2 = new SqlParameter("@Password", password);

            Clear();
            DataSetHelper.FillDataSet(
                ConnectionString.GetConnectionString(serverName),
                this, "_UserSet_Sel_ByUsernamePassword", param1, param2);
        }

        public void LoadSingle(int id)
        {
            SqlParameter param = new SqlParameter("@UserID", id);

            Clear();
            DataSetHelper.FillDataSet(
                ConnectionString.GetConnectionString(),
                this, "_UserSet_Sel_ByID", param);
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

            UserSet changes = GetChanges() as UserSet;

            DataSetHelper.SaveDataSet(ConnectionString.GetConnectionString(), changes);
            if (isSingleUser)
                LoadSingle(singleUserId);
            else
                LoadAll();

            DataSetHelper.ProcessRefreshedDataSet(this);
        }	
    }
}
