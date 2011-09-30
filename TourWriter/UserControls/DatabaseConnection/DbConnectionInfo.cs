using System;
using System.Linq;
using System.Collections.Generic;

namespace TourWriter.UserControls.DatabaseConnection
{
    public class ConnectionInfo
    {
        public bool AutoLogin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SelectedConnection { get; set; }
        public DbConnections DbConnections { get; set; }

        public ConnectionInfo(string defaultUserName, string defaultPassword, string currentConnection, DbConnections connections)
        {
            UserName = defaultUserName;
            Password = defaultPassword;
            SelectedConnection = currentConnection;
            DbConnections = connections;
        }
    }

    [Serializable]
    public class DbConnections : List<DbConnection>
    {
        public DbConnection Add(string type, string name, string data)
        {
            // ensure unique connection name
            var count = this.Where(x => x.Name.StartsWith(name)).Count();
            if (count > 0) name = string.Format("{0} ({1})", name, count + 1);

            var conn = new DbConnection(type, name, data);
            Add(conn);
            return conn; // return connection so caller can check if name changed (above)
        }
    }

    [Serializable]
    public class DbConnection
    {
        public string Type { get; set; } // "local" or "remote"
        public string Name { get; set; }
        public string Data { get; set; }

        // required for settings serialization
        public DbConnection() { }

        public DbConnection(string type, string name, string data)
        {
            Type = type;
            Name = name; 
            Data = data;
        }
    }
}
