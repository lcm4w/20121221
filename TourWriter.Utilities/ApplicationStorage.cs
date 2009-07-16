using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TourWriter.Utilities.CustomStorage
{
    /// <summary>
    /// Wrapper class for IsolatedStorage, inherits from Hashtable.
	/// Persists objects to file using key/value pair.
    	
    /// Usage: save settings
    /// CustomStorage.ApplicationStorage storage = new CustomStorage.ApplicationStorage("test.dat");
    /// storage["name"] = "john";
    /// storage["age"]  = 23;
    /// storage.Save();

    /// Usage: load settings
    /// CustomStorage.ApplicationStorage storage = new CustomStorage.ApplicationStorage("test.dat");
    /// string name = storage["name"].ToString();
    /// int age = int.Parse(storage["age"].ToString());
    /// string address = storage["address"].ToString();
    /// </summary>
	[Serializable]
	public class ApplicationStorage : Hashtable
	{
		// File name. Let us use the entry assembly name with .dat as the extension.
		private string settingsFileName;
    
		// The default constructor.
		public ApplicationStorage(string settingsFileName)
		{
			this.settingsFileName = settingsFileName;

			LoadData();
		}
        
		// This constructor is required for deserializing our class from persistent storage.
		protected ApplicationStorage (SerializationInfo info, StreamingContext context)	: base(info, context)
		{
		}
        
		
		private void LoadData()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore( IsolatedStorageScope.User 
				| IsolatedStorageScope.Assembly, null, null );
			if ( isoStore.GetFileNames( settingsFileName ).Length == 0 )
			{
				// File does not exists
				return;
			}
            
			// Read the stream from Isolated Storage.
			Stream stream = new IsolatedStorageFileStream( settingsFileName, 
				FileMode.OpenOrCreate, isoStore );
			if ( stream != null )
			{
				try
				{
					// DeSerialize the Hashtable from stream.
					IFormatter formatter = new BinaryFormatter();
					Hashtable appData = ( Hashtable ) formatter.Deserialize(stream);
                    
					// Enumerate through the collection and load our base Hashtable.
					IDictionaryEnumerator enumerator = appData.GetEnumerator();
					while ( enumerator.MoveNext() )
					{
						this[enumerator.Key] = enumerator.Value;
					}
				}
				finally
				{
					stream.Close();
				}
			}
		}
        
		public void ReLoad()
		{
			LoadData();
		}
        
		public void Save()
		{
			// Open the stream from the IsolatedStorage.
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore( IsolatedStorageScope.User 
				| IsolatedStorageScope.Assembly, null, null );
			Stream stream = new IsolatedStorageFileStream( settingsFileName, 
				FileMode.Create, isoStore );
        
			if ( stream != null )
			{
				try
				{
					// Serialize the Hashtable into the IsolatedStorage.
					IFormatter formatter = new BinaryFormatter();
					formatter.Serialize( stream, (Hashtable)this );
				}
				finally
				{
					stream.Close();
				}
			}
		}
	}
}