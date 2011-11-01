using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace TourWriter.Info
{
	[Serializable]
	public class License
    {
		[XmlIgnore]
        public string LocalLicenseID; // License ID as seen by the users database.

		public string   LicenseID;		// License ID as seen by TourWriter.com database.
		public string   LicenseType;	// Type of license ID.
		public int      MaxUsers;		// Maximum users allowed by this license.
		public DateTime StartDate;		// Start date that license is valid from.
		public DateTime EndDate;		// End date that license is valid to.
		public string   PurchaseURL;	// Url for puchasing new licenses.
		public string   InfoURL;		// Url for general TourWriter information.


        public void LoadFromXml(string xml)
		{
			try  
			{
				XmlSerializer xmls = new XmlSerializer(typeof(License));
                License license = (License)xmls.Deserialize(new StringReader(xml));
                LoadFromLicense(license);
			}
			catch(Exception ex)
			{
				throw new Exception("Invalid license file", ex);
			}
		}

        public void LoadFromLicense(License license)
        {
            LicenseID = license.LicenseID;
            LicenseType = license.LicenseType;
            MaxUsers = license.MaxUsers;
            StartDate = license.StartDate;
            EndDate = license.EndDate;
            PurchaseURL = license.PurchaseURL;
            InfoURL = license.InfoURL;
        }

        public void LoadFromFile(string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                XmlSerializer xmls = new XmlSerializer(typeof(License));
                License license = (License) xmls.Deserialize(fs);
                LoadFromLicense(license);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public void LoadFromDatabase()
        {
            var dr = Services.SqlHelper.ExecuteReader(
                Services.ConnectionString.GetConnectionString(), "_License_GetLatest");

            if (dr.HasRows)
            {
                dr.Read();
                LocalLicenseID = dr.GetInt32(0).ToString();
                LoadFromXml(dr.GetString(1));
            }
            else
            {
                LoadTrialLicense();
                SaveToDatabase();
            }
        }

        public void LoadTrialLicense()
        {
            LocalLicenseID = null;
            LicenseID = null;
            LicenseType = "Trial";
            MaxUsers = 10;
            StartDate = DateTime.Now; // trial period start
            EndDate = DateTime.Now.AddDays(16); // trial period end
            PurchaseURL = @"http://www.tourwriter.com";
            InfoURL = @"http://www.tourwriter.com";
        }

        public void SaveToDatabase()
        {
            string xml;

            // serialise license file
            StringWriter sw = null;
            try
            {
                // serialise
                XmlSerializer xmls = new XmlSerializer(typeof (License));
                sw = new StringWriter();
                xmls.Serialize(sw, this);
                xml = sw.ToString();
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }

            // insert data
            SqlParameter param1 = new SqlParameter("@LicenseFile", SqlDbType.Text);
            param1.Value = xml;
            param1.Direction = ParameterDirection.Input;

            SqlParameter param2 = new SqlParameter("@AddedOn", SqlDbType.DateTime);
            param2.Value = DateTime.Now.Date;
            param2.Direction = ParameterDirection.Input;

            SqlParameter param3 = new SqlParameter("@AddedBy", SqlDbType.Int);
            param3.Value = -1;
            param3.Direction = ParameterDirection.Input;

            SqlParameter param4 = new SqlParameter("@LicenseID", SqlDbType.Int);
            param4.Direction = ParameterDirection.Output;

            Services.SqlHelper.ExecuteReader(
                Services.ConnectionString.GetConnectionString(), "License_Ins", param1, param2, param3, param4);

            // set the install name from the license
            //string sql = String.Format("UPDATE AppSettings SET InstallName = '{0}'", LicenseType);
            //Services.SqlHelper.ExecuteNonQuery(Services.ConnectionString.GetConnectionString(), CommandType.Text, sql);
        }

		public void SaveAsXmlFile(string fileName)
		{
			FileStream fs = null;
			try
			{
				fs = new FileStream(fileName, FileMode.Create);
				XmlSerializer xs = new XmlSerializer(typeof(License));
				xs.Serialize(fs, this);			
			}
			finally
			{
                if(fs != null)
                    fs.Close();
			}
		}

        public bool IsCurrent()
        {
            return DateTime.Now.Date <= EndDate.Date;
        }

		public override string ToString()
		{
			string xml;
			StringWriter sw = null;

			try
			{
				sw = new StringWriter();

				XmlSerializer xs = new XmlSerializer(typeof(License));
				xs.Serialize(sw, this);
				xml = sw.ToString();
			}
			finally
            {
                if (sw != null)
                    sw.Close();
			}
			return xml;
		}
	}
}
