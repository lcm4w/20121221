using System;
using System.Collections.Generic;

namespace TourWriter.UserControls.DatabaseConfig
{
    [Serializable]
    public class DbConnections : List<DbConnection>
    {
        public void Add(string type, string name, string data)
        {
            Add(new DbConnection(type, name, data));
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
