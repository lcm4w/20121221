using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using TourWriter.Info;

namespace TourWriter.Services
{
	public class EmailMessage : MailMessage
	{
        public string _To;
        public string _Cc;
        public string _Bcc;
        public string _From;
		public object _Tag;
		public string _ErrorMsg;
        public bool _SaveWhenSent;
		public StatusType _Status;
        public enum StatusType
        {
            None,
            Sending,
            Sent,
            Stopped,
            Error
        }

		/// <summary>
		/// Creates a new EmailMessage with default parameters.
		/// </summary>
		public EmailMessage()
		{   
			_To = "";
			_Cc = "";
			_Bcc = "";
			_From = "";
			_Tag = null;
			_ErrorMsg = "";
			_Status = StatusType.None;
		}

        /// <summary>
        /// Send the email.
        /// </summary>
        /// <returns>True if the email was successfully sent.</returns></returns>
        public bool Send()
        {
            ToolSet.AppSettingsRow settings = Global.Cache.ToolSet.AppSettings[0];

            if (string.IsNullOrEmpty(settings.SmtpServerName))
                throw new ApplicationException("Email server name not found, email server settings may not have been entered.");

            return Send(
                settings.SmtpServerName,
                !settings.IsSmtpServerPortNull() ? settings.SmtpServerPort : 25,
                !settings.IsSmtpServerEnableSslNull() && settings.SmtpServerEnableSsl,
                settings.SmtpServerUsername,
                settings.SmtpServerPassword);
        }

        /// <summary>
        /// Send the email.
        /// </summary>
        /// <returns>True if the email was successfully sent.</returns>
        public bool Send(string host, int port, bool enableSsl, string username, string password)
        {
            FillStandardFieldsFromCustomFields();

            // FIX: replace body text as an AlternativeView to control transfer encoding, otherwise defaults to
            // 'quoted-printable' which is fragile and causes problems on some servers/clients.
            var body = Body;
            Body = null;
            var linkedResources = GetLinkedResources(ref body);
            var htmlPart = AlternateView.CreateAlternateViewFromString(body, Encoding.GetEncoding("iso-8859-1"), "text/html");
            htmlPart.TransferEncoding = TransferEncoding.SevenBit;
            AlternateViews.Add(htmlPart);
            foreach (var res in linkedResources)
                htmlPart.LinkedResources.Add(res);
            BodyEncoding = Encoding.GetEncoding("iso-8859-1");

            var smtpClient = new SmtpClient
                                 {
                                     Host = host,
                                     Port = port,
                                     EnableSsl = enableSsl,
                                     Credentials = new System.Net.NetworkCredential(username, password),
                                     DeliveryMethod = SmtpDeliveryMethod.Network
                                 };
            try
            {
                smtpClient.Send(this);
                _Status = StatusType.Sent;
                _ErrorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                _ErrorMsg = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                _Status = StatusType.Error;
                return false;
            }
        }

        /// <summary>
        /// Saves the email to the default location.
        /// </summary>
        /// <returns></returns>
        public string SaveToDisk()
        {
            return SaveToDisk(ExternalFilesHelper.GetEmailFolder());
        }

	    /// <summary>
        /// Save the email to the specified location. Creates the location if it does not exist.
        /// </summary>
        /// <returns>Path and name of the saved file.</returns>
        public string SaveToDisk(string saveToPath)
        {
            var saveClient = new SmtpClient
                                 {
                                     DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                                     PickupDirectoryLocation = saveToPath
                                 };
	        saveClient.SendAsync(this, "token?");

            saveToPath = GetLatestFileEntryName(saveToPath);
            
            // Reset
            _SaveWhenSent = false;

            return saveToPath;
        }

	    /// <summary>
	    /// Validats the emails address fields (to, from, cc, bcc).
	    /// </summary>
	    /// <returns>True if the email addresses are valid</returns>
	    public bool Validate()
	    {	
	        _ErrorMsg = "";
	        _Status = StatusType.None;

	        if(!App.ValidateEmailAddresses(_To))
	            _ErrorMsg += "Invalid 'To' address. ";

	        if(_Cc != "" && !App.ValidateEmailAddresses(_Cc))
	            _ErrorMsg += "Invalid 'Cc' address. ";

	        if(_Bcc != "" && !App.ValidateEmailAddresses(_Bcc))
	            _ErrorMsg += "Invalid 'Bcc' address. ";

	        if(_From != "" && !App.ValidateEmailAddresses(_From))
	            _ErrorMsg += "Invalid 'From' address. ";


	        if(_ErrorMsg != "")
	            _Status = StatusType.Error;
	        return (_Status != StatusType.Error);
	    }

	    /// <summary>
	    /// Add an attachment file.
	    /// </summary>
	    /// <param name="filename">The path and name of the file.</param>
	    public void AddAttachment(string filename)
	    {
	        Attachments.Add(new Attachment(filename, MediaTypeNames.Application.Octet));
	    }

	    /// <summary>
	    /// Add an attachment.
	    /// </summary>
        /// <param name="bytes">The attachment byte array.</param>
        /// <param name="fileName">The attachment name.</param>
	    public void AddAttachment(byte[] bytes, string fileName)
	    {
            var ms = new MemoryStream(bytes);
	        ms.Seek(0, SeekOrigin.Begin);
	        Attachments.Add(new Attachment(ms, fileName));
	        // Don't close, you get send error //ms.Close();
	    }
	    
	    /// <summary>
	    /// If custom parameters have been used, this fills the standard parameters with them. 
	    /// </summary>
	    private void FillStandardFieldsFromCustomFields()
	    {
	        if(!String.IsNullOrEmpty(_To))
	            To.Add(_To.Replace(';',','));
	        
	        if (!String.IsNullOrEmpty(_Cc))
	            CC.Add(_Cc.Replace(';',','));
	        
	        if (!String.IsNullOrEmpty(_Bcc))
	            Bcc.Add(_Bcc.Replace(';',','));

	        if (!String.IsNullOrEmpty(_From))
	            From = new MailAddress(_From);
        }

        private static string GetLatestFileEntryName(string path)
        {
            FileInfo newestFile = null;
            foreach (FileInfo f in new DirectoryInfo(path).GetFiles("*.eml"))
            {
                if (newestFile == null || f.CreationTime > newestFile.CreationTime)
                    newestFile = f;
            }
            return (newestFile != null) ? newestFile.FullName : "";
        }

        /// <summary>
        /// Replace img tags in a html string with content id and create related content resources list.
        /// </summary>
        /// <param name="html">Html string, returned updated as 'ref'</param>
        /// <returns>List of linked resources, and updated html string in 'ref' param.</returns>
        private static List<LinkedResource> GetLinkedResources(ref string html)
        {
            var temp = html;
            var linkedResources = new List<LinkedResource>();

            var searchStart = 0;
            while (true)
            {
                var indexOfImgTagOpen = temp.IndexOf("<img", searchStart, StringComparison.CurrentCultureIgnoreCase);
                if (indexOfImgTagOpen == -1)
                    break; // no more img tags

                searchStart = indexOfImgTagOpen + 4;
                if (searchStart >= temp.Length)
                    break; // end of string

                var indexOfImgTagClose = temp.IndexOf(">", indexOfImgTagOpen);
                if (indexOfImgTagClose == -1)
                    continue;

                var imgTag = temp.Substring(indexOfImgTagOpen, (indexOfImgTagClose - indexOfImgTagOpen) + 1);
                var imageLocation = GetImageLocationFromTag(imgTag);

                // only attach image if the location is NOT a web url
                if (imageLocation != null && !Uri.IsWellFormedUriString(imageLocation, UriKind.Absolute))
                {
                    try
                    {
                        if (html.IndexOf(imageLocation) == -1)
                            continue; // the image has already been attached

                        var stream = new System.Net.WebClient().OpenRead(imageLocation);
                        var resource = new LinkedResource(stream) { ContentId = Guid.NewGuid().ToString("N") };
                        linkedResources.Add(resource);
                        html = html.Replace(imageLocation, "cid:" + resource.ContentId);
                    }
                    catch (Exception ex)
                    {
                        // ignore file/directory not found errors
                        if (!(ex is FileNotFoundException || ex is DirectoryNotFoundException)) throw;
                    }
                }
            }
            return linkedResources;
        }

        /// <summary>
        /// Extracts the image location from a html img tag.
        /// </summary>
        private static string GetImageLocationFromTag(string imgTag)
        {
            // find the src element
            int indexOfSrcElement = imgTag.IndexOf("src", StringComparison.CurrentCultureIgnoreCase);
            if (indexOfSrcElement == -1)
                return null;
            imgTag = imgTag.Substring(indexOfSrcElement);
            imgTag = imgTag.Remove(0, 3);
            
            // find the '=' char, ignoring any white space
            imgTag = imgTag.TrimStart();
            if (imgTag[0] != '=')
                return null;
            imgTag = imgTag.Remove(0, 1);
            
            // find the opening quote, ignoring any white space
            imgTag = imgTag.TrimStart();
            if (imgTag[0] != '"' && imgTag[0] != '\'')
                return null;
            char quoteChar = imgTag[0];
            imgTag = imgTag.Remove(0, 1);

            // find the closing quote
            int indexOfClosingQuote = imgTag.IndexOf(quoteChar);
            if (indexOfClosingQuote == -1)
                return null;
            imgTag = imgTag.Remove(indexOfClosingQuote);

            return imgTag;
        }

        public static void SendAysync(string host, int port, bool enableSsl, string username, string password, string to, string subject, string body)
        {
            new System.Threading.Thread(() =>
            {
                var message = new MailMessage { Subject = subject, Body = body };
                message.To.Add(to);

                var client = new SmtpClient {Host = host, Port = port, EnableSsl = enableSsl, 
                    Credentials = new System.Net.NetworkCredential(username, password), DeliveryMethod = SmtpDeliveryMethod.Network};
                client.Send(message);
                message.Dispose();

            }).Start();
        }
	}
}
