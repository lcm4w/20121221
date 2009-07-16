using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace TourWriter.Info.Services
{
    /// <summary>
    /// Provides various database maintenance functions.
    /// </summary>
    public class DatabaseHelper
    {
        /// <summary>
        /// Performs database backup to a default temp file.
        /// </summary>
        public static void Backup()
        {
            Backup(new SqlConnection(ConnectionString.GetConnectionString()).Database + ".temp.bak");
        }

        /// <summary>
        /// Performs a backup on the default database.
        /// </summary>
        /// <param name="filename">File name to backup to.</param>
        public static void Backup(string filename)
        {
            var connection = new SqlConnection(ConnectionString.GetConnectionString());

            var backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.Database = connection.Database;
            backup.Devices.Add(new BackupDeviceItem(filename, DeviceType.File));
            backup.Checksum = true;
            backup.ContinueAfterError = true;
            backup.Incremental = false;
            backup.LogTruncation = BackupTruncateLogType.Truncate;

            var server = new Server(new ServerConnection(connection));
            backup.SqlBackup(server);
        }

        /// <summary>
        /// Gets the default Sql Server backup directory.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultBackupDir()
        {
            return
                new Server(new ServerConnection(new SqlConnection(ConnectionString.GetConnectionString()))).Settings.
                    BackupDirectory;
        }

        /// <summary>
        /// Performs restore on the default database, from a file.
        /// </summary>
        /// <param name="filename">File to restore from.</param>
        public static void Restore(string filename)
        {
            Exception exception = null;

            using (SqlConnection saConnection = new SqlConnection(ConnectionString.GetSaConnectionString()))
            {
                const string masterDb = "master";
                string restoreDb = saConnection.Database;
                Server server = new Server(new ServerConnection(saConnection));

                try
                {
                    // set database and restrict access
                    server.Databases[masterDb].ExecuteNonQuery(
                        string.Format("ALTER DATABASE [{0}] SET SINGLE_USER with ROLLBACK IMMEDIATE",
                        restoreDb));

                    // restore database
                    server.ConnectionContext.ExecuteNonQuery(
                        string.Format("RESTORE DATABASE [{0}] FROM DISK = N'{1}' WITH REPLACE;", 
                        restoreDb, filename));
                }
                catch (Exception ex) 
                {
                    exception = ex; // catch and continue, in attempt to restore access
                } 

                try
                {
                    // restore access
                    server.ConnectionContext.ExecuteNonQuery(
                        string.Format("ALTER DATABASE [{0}] SET MULTI_USER", restoreDb));

                    // set database and restore logins
                    server.Databases[restoreDb].ExecuteNonQuery("EXEC __Repair_Login");
                }
                catch
                {
                    if (exception == null)
                        throw;
                }
            }
            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Gets the database schema of the default database.
        /// </summary>
        public static string GetSchema()
        {
            SqlConnection connection = new SqlConnection(ConnectionString.GetConnectionString());
            Server server = new Server(new ServerConnection(connection));

            // Setup the transfer.
            Transfer t = new Transfer(server.Databases[connection.Database]);
            t.DropDestinationObjectsFirst = true;
            t.CopyAllObjects = true;
            t.CopyData = false;
            t.CopySchema = true;
            t.Options.DriAll = true;
            t.Options.WithDependencies = true;
            t.Options.IncludeIfNotExists = true;
            t.Options.ExtendedProperties = true;
            t.Options.NonClusteredIndexes = true;

            // Set to capture mode and capture the transer.
            server.ConnectionContext.SqlExecutionModes = SqlExecutionModes.CaptureSql;
            StringCollection commands = t.ScriptTransfer();

            StringBuilder sb = new StringBuilder();
            foreach (string s in commands)
                sb.AppendLine(s);

            return sb.ToString();
        }

        /// <summary>
        /// Runs a batch script using SMO.
        /// </summary>
        /// <param name="script">The script to run.</param>
        public static void RunScript(string script)
        {
            SqlConnection conn = new SqlConnection(ConnectionString.GetConnectionString());
            Server server = new Server(new ServerConnection(conn));
            server.ConnectionContext.ExecuteNonQuery(script);
        }

        /// <summary>
        /// Runs a batch script using SMO in a transaction context.
        /// </summary>
        /// <param name="script">The script to run.</param>
        public static void RunScriptWithTransaction(string script)
        {
            SqlConnection conn = new SqlConnection(ConnectionString.GetConnectionString());
            Server server = new Server(new ServerConnection(conn));
            try
            {
                server.ConnectionContext.BeginTransaction();
                server.ConnectionContext.ExecuteNonQuery(script);
                server.ConnectionContext.CommitTransaction();
            }
            catch
            {
                server.ConnectionContext.RollBackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Runs a sql command
        /// </summary>
        /// <param name="commandText">The sql to run</param>
        /// <param name="commandParameters">The command parameters</param>
        /// <returns>The result in a data reader</returns>
        public static DataSet ExecuteDataset(
            string commandText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(
                ConnectionString.GetConnectionString(), 
                CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// Runs a sql command
        /// </summary>
        /// <param name="commandText">The sql to run</param>
        /// <returns>The scalar result object</returns>
        public static object ExecuteScalar(string commandText)
        {
            return SqlHelper.ExecuteScalar(
                ConnectionString.GetConnectionString(), CommandType.Text, commandText);
        }
        
        /// <summary>
        /// Gets a list of available instances of Sql Server.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAvailableSqlServers()
        {
            return SmoApplication.EnumAvailableSqlServers();
        }

        /// <summary>
        /// Execute a SqlDataReader sql command.
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <returns>Resultset as a SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sql)
        {
            return SqlHelper.ExecuteReader(ConnectionString.GetConnectionString(), CommandType.Text, sql);
        }

        public static bool TestServerConnection()
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConnectionString.GetConnectionString());
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (conn != null) conn.Dispose();
            }
        }
    }
}
