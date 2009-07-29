using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
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
            ToolSet.AppSettingsRow settings = TourWriter.Global.Cache.ToolSet.AppSettings[0];

            if (string.IsNullOrEmpty(settings.SmtpServerName))
                throw new ApplicationException("Email server name not found, email server settings may not have been entered.");

            return Send(
                settings.SmtpServerName,
                !settings.IsSmtpServerPortNull() ? settings.SmtpServerPort : 25,
                !settings.IsSmtpServerEnableSslNull() ? settings.SmtpServerEnableSsl : false,
                settings.SmtpServerUsername,
                settings.SmtpServerPassword);
        }

        /// <summary>
        /// Send the email.
        /// </summary>
        /// <returns>True if the email was successfully sent.</returns>
        public bool Send(string host, int port, bool enableSsl, string username, string password)
        {
            if (IsBodyHtml)
                EmbedImages();

            FillStandardFieldsFromCustomFields();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

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
            Directory.CreateDirectory(saveToPath);
            SmtpClient saveClient = new SmtpClient();
            saveClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            saveClient.PickupDirectoryLocation = saveToPath;
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
            MemoryStream ms = new MemoryStream(bytes);
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
	            To.Add(_To);
	        
	        if (!String.IsNullOrEmpty(_Cc))
	            CC.Add(_Cc);
	        
	        if (!String.IsNullOrEmpty(_Bcc))
	            Bcc.Add(_Bcc);

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
        /// Searches for html img tags and attaches images where the image location is a local file path.
        /// Then the local image location is replaced with a content id pointing to the newly attached image.
        /// </summary>
        private void EmbedImages()
        {
            string tempBody = Body;
            List<LinkedResource> linkedResources = new List<LinkedResource>();

            int searchStart = 0;
            while (true)
            {
                int indexOfImgTagOpen = tempBody.IndexOf("<img", searchStart, StringComparison.CurrentCultureIgnoreCase);
                if (indexOfImgTagOpen == -1)
                    break; // no more img tags

                searchStart = indexOfImgTagOpen + 4;
                if (searchStart >= tempBody.Length)
                    break; // end of string

                int indexOfImgTagClose = tempBody.IndexOf(">", indexOfImgTagOpen);
                if (indexOfImgTagClose == -1)
                    continue;

                string imgTag = tempBody.Substring(indexOfImgTagOpen, (indexOfImgTagClose - indexOfImgTagOpen) + 1);
                string imageLocation = GetImageLocationFromTag(imgTag);

                // only attach image if the location is NOT a web url
                if (imageLocation != null && !Uri.IsWellFormedUriString(imageLocation, UriKind.Absolute))
                {
                    try
                    {
                        if (Body.IndexOf(imageLocation) == -1)
                            continue; // the image has already been attached

                        LinkedResource resource = new LinkedResource(imageLocation);
                        resource.ContentType.Name = Path.GetFileName(imageLocation);
                        resource.ContentId = Guid.NewGuid().ToString("N");
                        linkedResources.Add(resource);

                        Body = Body.Replace(imageLocation, "cid:" + resource.ContentId);
                    }
                    catch (Exception ex)
                    {
                        if (ex is FileNotFoundException ||
                            ex is DirectoryNotFoundException)
                        {
                            // ignore file/directory not found errors
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            AlternateView altView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
            foreach (LinkedResource res in linkedResources)
            {
                altView.LinkedResources.Add(res);
            }
            AlternateViews.Add(altView);
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
	}
}
