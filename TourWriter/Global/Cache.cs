using TourWriter.Info;
using TourWriter.BusinessLogic;

namespace TourWriter.Global
{
	/// <summary>
	/// Summary description for Cache.
	/// </summary>
	public class Cache
    {
        #region ToolSet

        private static ToolSet _toolSet;

		internal static ToolSet ToolSet
		{
			get
			{
				if(_toolSet == null)
				{
				    LoadToolSet();					
				}
				return _toolSet;
			}
		}

		internal static bool SaveToolSet()
		{
			if(_toolSet != null && _toolSet.HasChanges())
			{						
				// save changes
				ToolSet changes = (ToolSet)_toolSet.GetChanges();
                ToolSet fresh = new Tools().SaveToolSet(changes);		

				// handle errors
				App.DataSet_CheckForErrors(fresh);
			
				// clear and merge to maintain databindings
				_toolSet.Clear();
                _toolSet.Merge(fresh, false);
			}
			return true;
		}

		internal static void RefreshToolSet(bool saveData)
        {
            if (_toolSet == null)
            {
                LoadToolSet();
            }
            else
            {
                if (saveData)
                {
                    // let save do the refresh
                    SaveToolSet();
                }
                else
                {
                    // clear and merge to maintain databindings
                    Tools t = new Tools();
                    _toolSet.Clear();
                    _toolSet.Merge(t.GetToolSet(), false);
                }
            }
        }

        private static void LoadToolSet()
        {
            _toolSet = new Tools().GetToolSet();
            _toolSet.DefaultViewManager.DataViewSettings[_toolSet.ServiceType.TableName].RowFilter
                = "IsDeleted = 'FALSE' OR IsDeleted IS NULL";


            _toolSet.AppSettings[0].CcyRateSource = "predefined";
            _toolSet.AppSettings[0].CcyDatePoint= "booking";
        }

        #endregion

        #region UserSet

        private static int _userId;

        private static UserSet _userSet;

        internal static void SetCurrentUser(UserSet userSet, int userId)
        {
            _userId = userId;
            _userSet = userSet;
        }

        internal static UserSet.UserRow User
        {
            get
            {
                return _userSet.User.FindByUserID(_userId);
            }
        }

        internal static UserSet UserSet
        {
            get { return _userSet; }
            set { _userSet = value; }
        }

	    internal static bool IsUserLoggedIn
	    {
            get { return _userSet != null && _userSet.User.Rows.Count > 0;  }
	    }

        internal static bool SaveUserSet()
        {
            if (_userSet != null && _userSet.HasChanges())
            {
                _userSet.Save();

                App.DataSet_CheckForErrors(_userSet);

                MergeUserSetWithToolSet();

            }
            return true;
        }

        internal static void RefreshUserSet(bool saveData)
        {
            if (saveData)
            {
                // let save do the refresh
                _userSet.Save();
            }
            else
            {
                _userSet.LoadSingle(User.UserID);
            }
        }

        private static void MergeUserSetWithToolSet()
        {
            if (_toolSet != null)
            {
                _toolSet.User.Clear();
                _toolSet.User.Merge(_userSet.User, false);
            }
        }

        #endregion

        #region AgentSet

        private static AgentSet _agentSet;

        internal static AgentSet AgentSet
        {
            get
            {
                if (_agentSet == null)
                {
                    Agent a = new Agent();
                    _agentSet = a.GetAgentSet();
                    _agentSet.Agent.RowDeleting += Agent_RowDeleting;
                }
                return _agentSet;
            }
            set { _agentSet = value; }
        }

        internal static bool SaveAgentSet()
        {
            if (_agentSet != null && _agentSet.HasChanges())
            {
                // save changes
                AgentSet changes = (AgentSet)_agentSet.GetChanges();
                AgentSet fresh = new Agent().SaveAgentSet(changes);

                // handle errors
                App.DataSet_CheckForErrors(fresh);

                // clear and merge to maintain databindings
                _agentSet.Clear();
                _agentSet.Merge(fresh, false);
                MergeAgentSetWithToolSet();
            }
            return true;
        }

        internal static void RefreshAgentSet(bool saveData)
        {
            if (saveData)
            {
                // let save do the refresh
                SaveAgentSet();
            }
            else
            {
                // clear and merge to maintain databindings
                Agent t = new Agent();
                _agentSet.Clear();
                _agentSet.Merge(t.GetAgentSet(), false);
                MergeAgentSetWithToolSet();
            }
        }

        private static void MergeAgentSetWithToolSet()
        {
            if (_toolSet != null)
            {
                _toolSet.Agent.Clear();
                _toolSet.Agent.Merge(_agentSet.Agent, false);
            }
        }

        private static void Agent_RowDeleting(object sender, System.Data.DataRowChangeEventArgs e)
        {
            int agentId = (int)e.Row["AgentID"];

            foreach (AgentSet.AgentRow row in _agentSet.Agent)
            {
                if (row.RowState == System.Data.DataRowState.Deleted) continue;
                if (!row.IsParentAgentIDNull() && row.ParentAgentID == agentId)
                {
                    row.SetParentAgentIDNull();
                }
            }
        }

        #endregion
    }
}
