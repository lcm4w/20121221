using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TourWriter.Utilities.Binary

{
	public class BinaryHelper 
	{
		public static void SerialiseToFile(string pathAndFile, object objectToSerialise)
		{
			FileStream fs = null;
			try
			{
				fs = new FileStream(pathAndFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(fs, objectToSerialise);
			}
			finally
			{
				if(fs != null) fs.Close();
			}
		}
		

		public static object DeserialiseFromFile(string pathAndFile)
		{
			FileStream fs = null;
			try
			{
				fs = new FileStream(pathAndFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				BinaryFormatter bf = new BinaryFormatter();
				return bf.Deserialize(fs);
			}
			finally
			{
				if(fs != null) fs.Close();
			}
		}

	} 
} 