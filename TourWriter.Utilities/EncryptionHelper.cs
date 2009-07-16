using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;


namespace TourWriter.Utilities.Encryption
{
	public class EncryptionHelper
	{        		
		private static byte[] _Key = {0x22, 0xAB, 0x10, 0x64, 0x15, 0xCC, 0x03, 0xEF};
		private static byte[] _IV  = {0xDD, 0x12, 0x56, 0xCF, 0x90, 0x34, 0xCD, 0x19}; 

		public static string EncryptString(string stringToEncrypt)
		{
			return EncryptString(stringToEncrypt, _Key, _IV);
		}

		public static string EncryptString(string stringToEncrypt, string strEncr_Key)
		{
			return EncryptString(stringToEncrypt, KeyFromString(strEncr_Key), _IV);
		}

		public static string DecryptString(string stringToDecrypt)
		{
			return DecryptString(stringToDecrypt, _Key, _IV);
		}

		public static string DecryptString(string stringToDecrypt, string strEncr_Key)
		{			
			return DecryptString(stringToDecrypt, KeyFromString(strEncr_Key), _IV);
		}

		
		public static void   EncryptToFile(object obj, string pathAndFile)
		{
			EncryptToFile(obj, pathAndFile, _Key, _IV);
		}

		public static void   EncryptToFile(object obj, string pathAndFile, string strEncr_Key)
		{
			EncryptToFile(obj, pathAndFile, KeyFromString(strEncr_Key), _IV);
		}

		public static object DecryptFromFile(string pathAndFile)
		{
			return DecryptFromFile(pathAndFile, _Key, _IV);
		}
		
		public static object DecryptFromFile(string pathAndFile, string strEncr_Key)
		{
			return DecryptFromFile(pathAndFile, KeyFromString(strEncr_Key), _IV);
		}


		private static byte[] KeyFromString(string strEncr_Key)
		{
			byte[] key = new byte [8] ;
			key = System.Text.Encoding.UTF8.GetBytes( strEncr_Key.Substring(0,8));

			return key;
		}
		
		private static string EncryptString(string strText, byte[] key, byte[] iv)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			Byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
			MemoryStream ms = new MemoryStream();

			CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();

			return Convert.ToBase64String(ms.ToArray());
		}

		private static string DecryptString(string strText, byte[] key, byte[] iv)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			Byte[] inputByteArray = inputByteArray = Convert.FromBase64String(strText);
			MemoryStream ms = new MemoryStream();

			CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();

			System.Text.Encoding encoding = System.Text.Encoding.UTF8;
			return encoding.GetString(ms.ToArray());
		}	

		private static void   EncryptToFile(object obj, string pathAndFile, byte[] key, byte[] iv)
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
				cs = new CryptoStream(fs, RMCrypto.CreateEncryptor(key, iv), CryptoStreamMode.Write);

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

		private static object DecryptFromFile(string pathAndFile, byte[] key, byte[] iv)
		{
			FileStream fs = null;
			CryptoStream cs = null;
			Object obj = null;
			try
			{
				RijndaelManaged RMCrypto = new RijndaelManaged();

				fs = new FileStream(pathAndFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				cs = new CryptoStream(fs, RMCrypto.CreateDecryptor(key, iv), CryptoStreamMode.Read);

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
