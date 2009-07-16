using TourWriter.Info;
using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// Tools business logic layer, accesses the data layer.
	/// </summary>
	public class Tools : BLLBase
	{
		public ToolSet GetToolSet()
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ToolsDB db = new ToolsDB(ConnectionString);
					return db.GetToolSet();
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

		
		public ToolSet SaveToolSet(ToolSet ds)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					ToolsDB db = new ToolsDB(ConnectionString);
					return db.SaveToolSet(ds);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
		
	}
}
