using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace TourWriter.Reports
{
	/// <summary>
	/// Localization Class
	/// Designed to read in a embeded resource file for the report and change the static Label
	/// and TextBox text fields to a localized language
	/// </summary>
	public class Localization
	{
		/// <summary>
		/// Localize Method
		/// Takes a ActiveReport object and a culture info object, looks up the embeded
		/// resource file and changes the assigned TextBox and Label objects to the localized
		/// text.
		/// </summary>
		/// <param name="rpt">Report object to localize</param>
		/// <param name="culture">CultureInfo object to localize to</param>
		public static void Localize(DataDynamics.ActiveReports.ActiveReport rpt, string languageFileName, System.Globalization.CultureInfo culture)
		{
			//Local xml document from the Resources
			System.Xml.XmlDocument xmlLocalizationFile = RetrieveLocalizationResources(languageFileName);

			//If a resources file hasn't found, return
			if(xmlLocalizationFile == null)
				return;

			//Get the Language list from the xml data
			System.Xml.XmlNodeList elemList = xmlLocalizationFile.GetElementsByTagName("language");
			System.Xml.XmlElement cultureElem = null;

			//Identify the data for the passed in culture
			foreach(System.Xml.XmlElement elem in elemList)
			{
				if(elem.Attributes["langid"].Value == culture.Name)
				{
					cultureElem = elem;
				}
			}

			//If no data was found, return
			if(cultureElem == null)
				return;

			//Load the localized Label and TextBox data
			System.Xml.XmlNodeList textList = cultureElem.GetElementsByTagName("localize");

			foreach(DataDynamics.ActiveReports.Section sec in rpt.Sections)
			{
				foreach(DataDynamics.ActiveReports.ARControl ctl in sec.Controls)
				{
					string name = rpt.Document != null ? rpt.Document.Name : "report";
					System.Diagnostics.Debug.WriteLine("Report: " + name + ", Control: " + ctl.Name);

					foreach(System.Xml.XmlElement elem in textList)
					{
						//Proper control found
						if(elem.Attributes["name"].Value == ctl.Name)
						{
							//Check type, if type passes then make the change
							if(ctl is DataDynamics.ActiveReports.TextBox && elem.Attributes["type"].Value.ToLower() == "textbox")
								((DataDynamics.ActiveReports.TextBox) ctl).Text = elem.Attributes["text"].Value;
							if(ctl is DataDynamics.ActiveReports.Label && elem.Attributes["type"].Value.ToLower() == "label")
								((DataDynamics.ActiveReports.Label) ctl).Text = elem.Attributes["text"].Value;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets a list of the languages available for this report, in an ArrayList of Dictionary objects.
		/// </summary>
		/// <returns>ArrayList of key/value pair DictionaryEntry objects.</returns>
		public static ArrayList GetLanguagesInDictionaryEntryArray(string languageFileName)
		{
			//Local xml document from the Resources
			System.Xml.XmlDocument xmlLocalizationFile = RetrieveLocalizationResources(languageFileName);

			//If a resources file wasn't found, return
			if(xmlLocalizationFile == null)
				return new ArrayList(0);

			//Get the Language list from the xml data
			System.Xml.XmlNodeList elemList = xmlLocalizationFile.GetElementsByTagName("language");

			ArrayList ar = new ArrayList(elemList.Count);

			foreach(System.Xml.XmlElement elem in elemList)
			{
				 ar.Add(new DictionaryEntry(
					 elem.Attributes["langname"].Value, elem.Attributes["langid"].Value));
			}
			return ar;
		}
		
		
		private static System.Xml.XmlDocument RetrieveLocalizationResources(string languageFileName)
		{
			System.Xml.XmlDocument xmlLocalizationFile = new System.Xml.XmlDocument();

			try
			{
				FileStream file = new FileStream(
					languageFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				
				System.Xml.XmlTextReader reader = null;
				reader = new System.Xml.XmlTextReader(file);
				reader.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
				
				string xmlString = "";

				// Parse the file and display each of the nodes.
				while (reader.Read()) 
				{
					switch (reader.NodeType) 
					{
						case XmlNodeType.Element:
							xmlString = xmlString + String.Format("<{0}", reader.Name.ToLower());
							bool isEmpty = reader.IsEmptyElement;
							if(reader.HasAttributes)
							{
								bool hasAttributes = reader.MoveToFirstAttribute();
								
								while(hasAttributes)
								{
									xmlString = xmlString + String.Format(" {0}=\"{1}\"", reader.Name.ToLower(), reader.Value);
									hasAttributes = reader.MoveToNextAttribute();
								}
							}
							if(isEmpty)
								xmlString = xmlString + "/>";
							else
								xmlString = xmlString + ">";
							break;
						case XmlNodeType.Text:
							xmlString = xmlString + reader.Value;
							break;
						case XmlNodeType.CDATA:
							xmlString = xmlString + String.Format("<![CDATA[{0}]]>", reader.Value);
							break;
						case XmlNodeType.ProcessingInstruction:
							xmlString = xmlString + String.Format("<?{0} {1}?>", reader.Name, reader.Value);
							break;
						case XmlNodeType.Comment:
							xmlString = xmlString + String.Format("<!--{0}-->", reader.Value);
							break;
						case XmlNodeType.XmlDeclaration:
							xmlString = xmlString + "<?xml version='1.0'?>";
							break;
						case XmlNodeType.Document:
							break;
						case XmlNodeType.DocumentType:
							xmlString = xmlString + String.Format("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
							break;
						case XmlNodeType.EntityReference:
							xmlString = xmlString + reader.Name;
							break;
						case XmlNodeType.EndElement:
							xmlString = xmlString + String.Format("</{0}>", reader.Name.ToLower());
							break;
					}       
				}
				xmlLocalizationFile.LoadXml(xmlString);

				return xmlLocalizationFile;
			}
			catch(Exception)
			{
				return null;
			}
		}
	
	}
}
