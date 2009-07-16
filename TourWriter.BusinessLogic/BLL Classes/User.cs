namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// User business logic layer, accesses the data layer.
	/// </summary>
	public class User : BLLBase
	{
        // moved UserSet stuff to UserSet class
        //public UserSet Authenticate(string typeOfConnection, string url, string username, string password)
        //{
        //    // first point of database access for application, so read connection settings 
        //    // from user login rather than from settings file in case they have been changed
        //    // during login.
        //    if(typeOfConnection == "LAN")
        //    {
        //        AppSettings appSettings = TourWriter.Utilities.Xml.XmlHelper.DeserialiseFromFile(
        //            AppSettingPathAndFile, typeof(AppSettings)) as AppSettings;

        //        appSettings.LANLocation = url;

        //        UserDB db = new UserDB(appSettings.DbUser_ConnectionString);

        //        return db.GetUserSet(username, password); 
        //    }
        //    else if(typeOfConnection == "WAN")
        //        throw new ArgumentException("Web Services not implimented");
        //    else
        //        return null;
        //}

        //public UserSet GetUserSet()
        //{	
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            return db.GetUserSet(); 
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        {}
        //    }
        //    return null;
        //}

        //public UserSet GetUserSet(int id)
        //{
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            return db.GetUserSet(id);
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        {}
        //    }
        //    return null;
        //}

        //public UserSet SaveUserSet(UserSet ds)
        //{
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            return db.SaveUserSet(ds);
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        { }
        //    }
        //    return null;
        //}

        		
        //public DataSet GetUserLogins()
        //{
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            return db.GetUserLogins(); 
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        {}
        //    }
        //    return null;
        //}
		
        //public Guid AddUserLogin(Guid loginGuid, int userID, string computerName, int maxUsers)
        //{
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            return db.AddUserLogin(loginGuid, userID, computerName, maxUsers); 
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        {}
        //    }
        //    return Guid.Empty;
        //}
				
        //public void RemoveUserLogin(Guid loginGuid)
        //{
        //    if(GetConnectionString())
        //    {
        //        if(TypeOfConnection == "LAN")
        //        {
        //            UserDB db = new UserDB(ConnectionString);
        //            db.RemoveUserLogin(loginGuid); 
        //        }				
        //        else if(TypeOfConnection == "WAN") 
        //        {}
        //    }
        //}
	}
}
