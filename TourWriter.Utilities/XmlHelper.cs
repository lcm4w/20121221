using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;

namespace TourWriter.Utilities.Xml
{
	/// <summary>
	/// Helper class for basic XML functions: serialization, schema validation, etc.
	/// </summary>
	public class XmlHelper 
	{
		/// <summary>
		/// Returns the XML representation of an object as a string.
		/// </summary>
		/// <param name="objectType">
		/// Type of the object to serialize as a string of XML
		/// </param>
		/// <param name="objectToSerialize">
		/// The data class (or other object) to convert to XML
		/// </param>
		public static string ObjectToXML(Type objectType, object objectToSerialize)
		{
			return ObjectToXML(objectType, objectToSerialize, null);
		}

		public static string ObjectToXML(Type objectType, object objectToSerialize, XmlSerializerNamespaces namespaces)
		{
			XmlSerializer xmlSerializer = null;
			MemoryStream stream = null;
			ASCIIEncoding encoder = null;
			byte[] byteArray;
 
			try
			{
				stream = new MemoryStream();
				xmlSerializer = new XmlSerializer(objectType);

				if (namespaces == null)
				{
					xmlSerializer.Serialize(stream, objectToSerialize);
				}
				else
				{
					xmlSerializer.Serialize(stream, objectToSerialize, namespaces);
				}

				stream.Position = 0;
				byteArray = stream.ToArray();
				encoder = new ASCIIEncoding();
				return encoder.GetString(byteArray);

			}
			finally
			{
				xmlSerializer = null;
				stream = null;
				encoder = null;
			}

		}

		public static object XMLToObject(Type objectType, string xmlData)
		{
			return XMLToObject(objectType, xmlData, null);
		}


		/// <summary>
		/// Returns the object represented by the string parameter containing the XML
		/// representation of the object
		/// </summary>
		/// <param name="objectType">
		/// The type of the object to be created. Use the
		/// GetType() method of an object to get this value.
		/// </param>
		/// <param name="xmlData">
		/// The XML representation of the object or object graph
		/// </param>
		public static object XMLToObject(Type objectType, string xmlData, string namespaceURI)
		{
				
			XmlSerializer xmlSerializer = null;
			MemoryStream stream = null;
			ASCIIEncoding encoder = null;
			byte[] byteArray;

			try
			{
				//check if null works, but for now
				if (namespaceURI == null)
				{
					xmlSerializer = new XmlSerializer(objectType);
				}
				else
				{
					xmlSerializer = new XmlSerializer(objectType, namespaceURI);
				}

				//Convert the xmlData into a byteArray and put it into the 
				//memory stream.
				encoder = new ASCIIEncoding();
				byteArray = encoder.GetBytes(xmlData);
				stream = new MemoryStream();
				stream.Write(byteArray, 0, byteArray.Length);
				stream.Position = 0;

				return xmlSerializer.Deserialize(stream);
			}
			finally
			{
				xmlSerializer = null;
				stream = null;
				encoder = null;

			}

		}

		/// <summary>
		/// Validates a string of XML against a schema with the provided URL
		/// </summary>
        /// <param name="namespaceURL">The namespace URL</param>
        /// <param name="schemaURL">The URL for the schema used to validate the XML</param>
		/// <param name="xmlToValidate">The XML to be validated</param>
		public static bool IsXMLValid(string namespaceURL, string schemaURL, string xmlToValidate)
		{
            try
			{
                // convert string to stream
                var xmlStream = new StringReader(xmlToValidate);

                // setup the validation settings
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(namespaceURL, schemaURL);

				var context = new XmlParserContext(null, null, "", XmlSpace.None);
                var reader = XmlReader.Create(xmlStream, settings, context);

				while(reader.Read())
				{ }

				return true;
			}
			catch
			{
				return false;
			}
		}


		public static void SerialiseToFile(string pathAndFile, object objectToSerialise)
		{
			TextWriter writer = null;
			try
			{
				writer = new StreamWriter(pathAndFile);
				XmlSerializer serializer = new XmlSerializer(objectToSerialise.GetType());

				serializer.Serialize(writer, objectToSerialise);
			}
			finally
			{
				writer.Close();
			}
		}

		
		public static object DeserialiseFromFile(string pathAndFile, Type objectType)
		{
			TextReader reader = null;		
			try
			{
				reader = new StreamReader(pathAndFile);
				XmlSerializer serializer = new XmlSerializer(objectType);	

				return serializer.Deserialize(reader);
			}
			finally
			{
				reader.Close();
			}
		}


		
		// Sign an XML file and save the signature in a new file.
		public static void SignXmlFile(string FileName, string SignedFileName, RSA Key)
		{
			// Create a new XML document.
			XmlDocument doc = new XmlDocument();

			// Format the document to ignore white spaces.
			doc.PreserveWhitespace = false;

			// Load the passed XML file using it's name.
			doc.Load(new XmlTextReader(FileName));

			SignXmlDoc(doc, SignedFileName, Key);
		}
		
		public static void SignXmlDoc(XmlDocument doc, string SignedFileName, RSA Key)
		{
			// Create a SignedXml object.
			SignedXml signedXml = new SignedXml(doc);

			// Add the key to the SignedXml document. 
			signedXml.SigningKey = Key;

			// Create a reference to be signed.
			Reference reference = new Reference();
			reference.Uri = "";

			// Add an enveloped transformation to the reference.
			XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);

			// Add the reference to the SignedXml object.
			signedXml.AddReference(reference);

			// Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
			KeyInfo keyInfo = new KeyInfo();
			keyInfo.AddClause(new RSAKeyValue((RSA)Key));
			signedXml.KeyInfo = keyInfo;

			// Compute the signature.
			signedXml.ComputeSignature();

			// Get the XML representation of the signature and save
			// it to an XmlElement object.
			XmlElement xmlDigitalSignature = signedXml.GetXml();

			// Append the element to the XML document.
			doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));


//			if (doc.FirstChild is XmlDeclaration)  
//			{
//				doc.RemoveChild(doc.FirstChild);
//			}

			// Save the signed XML document to a file specified
			// using the passed string.
			XmlTextWriter xmltw = new XmlTextWriter(SignedFileName, new UTF8Encoding(false));
			doc.WriteTo(xmltw);
			xmltw.Close();
		}

		// Verify the signature of an XML file and return the result.
		public static Boolean VerifySignedXmlFile(String Name)
		{
			// Create a new XML document.
			XmlDocument xmlDocument = new XmlDocument();

			// Format using white spaces.
			xmlDocument.PreserveWhitespace = true;

			// Load the passed XML file into the document. 
			xmlDocument.Load(Name);

			return VerifySignedXmlDoc(xmlDocument);
		}

		public static Boolean VerifySignedXmlDoc(XmlDocument xmlDocument)
		{
			// Create a new SignedXml object and pass it
			// the XML document class.
			SignedXml signedXml = new SignedXml(xmlDocument);

			// Find the "Signature" node and create a new
			// XmlNodeList object.
			XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Signature");

			// Load the signature node.
			signedXml.LoadXml((XmlElement)nodeList[0]);

			// Check the signature and return the result.
			return signedXml.CheckSignature();
		}

	} 
} 