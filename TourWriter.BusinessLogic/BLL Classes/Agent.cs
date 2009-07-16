using TourWriter.Info;
using TourWriter.DataAccess;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// Agent business logic layer, accesses the data layer.
	/// </summary>
	public class Agent : BLLBase
	{
        public AgentSet GetAgentSet()
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					AgentDB db = new AgentDB(ConnectionString);
					return db.GetAgentSet();
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}

		public AgentSet GetAgentSet(int id)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					AgentDB db = new AgentDB(ConnectionString);
					return db.GetAgentSet(id);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
		
		public AgentSet SaveAgentSet(AgentSet ds)
		{
			if(GetConnectionString())
			{
				if(TypeOfConnection == "LAN")
				{
					AgentDB db = new AgentDB(ConnectionString);
					return db.SaveAgentSet(ds);
				}				
				else if(TypeOfConnection == "WAN")
				{}
			}
			return null;
		}
	}
}
