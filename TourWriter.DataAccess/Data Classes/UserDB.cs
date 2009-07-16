namespace TourWriter.DataAccess
{
	/// <summary>
	/// UserDB class accesses the database User data.
	/// </summary>
	public class UserDB
	{
		private string connectionString;
		public UserDB(string connectionString)
		{
			this.connectionString = connectionString;
		}
	

        ///// <summary>
        ///// Fills a UserSet with data.
        ///// </summary>
        ///// <returns></returns>
        //public UserSet GetUserSet()
        //{ 
        //    UserSet ds = new UserSet();
        //    return (UserSet)DataSetHelper.FillDataSet(connectionString, ds, "_UserSet_Sel_All");
        //}

        ///// <summary>
        ///// Fills a UserSet with data based on the username password parameters.
        ///// </summary>
        //public UserSet GetUserSet(string username, string password)
        //{
        //    UserSet ds = new UserSet();
        //    SqlParameter param1 = new SqlParameter("@Username", username);
        //    SqlParameter param2 = new SqlParameter("@Password", password);

        //    ds = (UserSet)DataSetHelper.FillDataSet(
        //        connectionString, ds, "_UserSet_Sel_ByUsernamePassword", param1, param2);
																				
        //    return ds;
        //}

        ///// <summary>
        ///// Fills a UserSet with data based on the input 'ID' parameter.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public UserSet GetUserSet(int id)
        //{
        //    UserSet ds = new UserSet();
        //    SqlParameter param = new SqlParameter("@UserID", id);

        //    ds = (UserSet)DataSetHelper.FillDataSet(connectionString, ds, "_UserSet_Sel_ByID", param);

        //    // save input parameter in ExtendedProperties for later handling
        //    ds.ExtendedProperties.Add("UserID", param.Value.ToString());
																				
        //    return ds;
        //}
		
        ///// <summary>
        ///// Saves changes to a UserSet, updates any row errors, and refreshes the data
        ///// based on the ExtendedProperties 'ID' key.
        ///// </summary>
        ///// <param name="ds">UserSet with changes</param>
        ///// <returns>Refreshed UserSet</returns>
        //public UserSet SaveUserSet(UserSet ds)
        //{			
        //    // save changes
        //    UserSet changes = ds.GetChanges() as UserSet;
        //    DataSetHelper.SaveDataSet(connectionString, changes);

        //    // refresh data with single or all user(s)
        //    UserSet fresh = null;
        //    if(ds.ExtendedProperties.ContainsKey("UserID"))
        //        fresh = GetUserSet((int.Parse(ds.ExtendedProperties["UserID"].ToString()))); 
        //    else	
        //        fresh = GetUserSet();
			
        //    // merge
        //    changes.Merge(fresh, true);
        //    DataSetHelper.ProcessRefreshedDataSet(changes);

        //    return changes;
        //}	




        ///// <summary>
        ///// Get all logins from the database
        ///// </summary>
        ///// <returns></returns>
        //public DataSet GetUserLogins()
        //{						
        //    return SqlHelper.ExecuteDataset(connectionString, "Login_Sel_All");		
        //}

        ///// <summary>
        ///// Adds a login to the database, or if it already exists updates it.
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="loginGuid"></param>
        ///// <param name="maxUsers"></param>
        ///// <returns></returns>
        //public Guid AddUserLogin(Guid loginGuid, int userID, string computerName, int maxUsers)
        //{									  
        //    SqlParameter param1 = new SqlParameter("@LoginID",      SqlDbType.UniqueIdentifier);			
        //    SqlParameter param2 = new SqlParameter("@UserID",       SqlDbType.Int);				
        //    SqlParameter param3 = new SqlParameter("@ComputerName", SqlDbType.VarChar);			
        //    SqlParameter param4 = new SqlParameter("@MaxUsers",     SqlDbType.Int); 			
        //    SqlParameter param5 = new SqlParameter("@NewGuid",      SqlDbType.UniqueIdentifier); 

        //    param1.Value = loginGuid;
        //    param2.Value = userID;
        //    param3.Value = computerName;
        //    param4.Value = maxUsers; 
        //    param5.Direction = ParameterDirection.Output ;

        //    object o = SqlHelper.ExecuteScalar(connectionString, "_User_CheckLogin", param1, param2, param3, param4, param5);
				
        //    if(o != DBNull.Value)
        //        return (Guid)o;
        //    return Guid.Empty;
        //}

        ///// <summary>
        ///// Removes a login from the database.
        ///// </summary>
        ///// <param name="loginGuid"></param>
        //public void RemoveUserLogin(Guid loginGuid)
        //{
        //    SqlParameter param1 = new SqlParameter("@LoginID",  SqlDbType.UniqueIdentifier);
        //    param1.Value = loginGuid;
			
        //    SqlHelper.ExecuteScalar(connectionString, "_User_RemoveLogin", param1);			
        //}
		
	}
}
