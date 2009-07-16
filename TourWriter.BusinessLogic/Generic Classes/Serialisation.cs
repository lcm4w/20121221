using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace TourWriter.BusinessLogic
{
	/// <summary>
	/// Methods to encrypt and serialise data to disk files.
	/// </summary>
	public class Serialisation
	{
		private static byte[] Key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
		private static byte[] IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
		
		/// <summary>
		/// Serialize and encrypt an object to file using Rijndael encryption.
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <param name="pathAndFile">Path and file name to create/serialize to</param>
		public static void EncryptToFile(object obj, string pathAndFile)
		{
			FileStream fs = null;			
			CryptoStream cs = null;
			try
			{				
				//Create a new instance of the RijndaelManaged class to encrypt the stream.
				RijndaelManaged RMCrypto = new RijndaelManaged();

				// Open stream to write to
				fs = new FileStream(pathAndFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
				
				// encrypt the filestream with the Rijndael class.
				cs = new CryptoStream(fs, RMCrypto.CreateEncryptor(Key, IV), CryptoStreamMode.Write);

				// serialize the data into the encryption stream.
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(cs, obj);
			}
			finally
			{
				//Close all the connections.
				cs.Close();
				fs.Close();
			}
		}

		/// <summary>
		/// Deserialize and decrypt an object from file using Rijndael encryption.
		/// </summary>
		/// <param name="pathAndFile">Object file to read from</param>
		/// <returns>decrypted object</returns>
		public static object DecryptFromFile(string pathAndFile)
		{
			FileStream fs = null;
			CryptoStream cs = null;
			Object obj = null;
			try
			{
				RijndaelManaged RMCrypto = new RijndaelManaged();

				fs = new FileStream(pathAndFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				cs = new CryptoStream(fs, RMCrypto.CreateDecryptor(Key, IV), CryptoStreamMode.Read);

				// deserialize the data from the encryption stream.
				BinaryFormatter formatter = new BinaryFormatter();
				obj = formatter.Deserialize(cs);
			}
			finally
			{
				//Close all the connections.
				if(cs != null) cs.Close();
				if(fs != null) fs.Close();
			}
			return obj;
		}


	}

}
