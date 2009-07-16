using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TourWriter.BusinessLogic
{
	public class BLLBase
	{
		// Hard coded application settings file path
		internal string AppSettingPathAndFile = Path.Combine(Path.Combine(Environment.GetFolderPath(
			Environment.SpecialFolder.CommonApplicationData),	
			System.Windows.Forms.Application.ProductName), 
			"AppSettings.xml");		

		internal string TypeOfConnection;
		internal string ConnectionString;


		internal bool GetConnectionString()
		{
		    ConnectionString = Info.Services.ConnectionString.GetConnectionString_UNSAFE();
		    TypeOfConnection = "LAN";
            return true;
		}
		
		internal object LoadBinaryData(string path)
		{			
			FileStream fs = new FileStream(path, FileMode.Create);
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				Object obj = bf.Deserialize(fs);
				return obj;
			}
			catch
			{
			}
			finally
			{
				if(fs != null) fs.Close();
			}
			return null;
		}


		public bool TestSqlServerConnection(string servername)
		{
			try
			{
				// get the name of the default sql instance installer
				string[] s = servername.Split('\\');
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(s[0]);

				System.Net.IPAddress ip = host.AddressList[0];

				// if the Sql Server is not set to port 1433 (default), then this will not work
				System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
				client.Connect(ip, 1433);
				client.Close();
				return true;
			}
			catch
			{ 
				return false;
			}
		}
		
		public bool TestWebServiceConnection(string wsServer)
		{
			// ** not yet implimented **
//			AgentWS.DataServWS = GetProxy();
//			try
//			{
//				proxy.Ping();
//				return true;
//			}
//			catch
//			{
			return false;
//			}
		}
	}
}
