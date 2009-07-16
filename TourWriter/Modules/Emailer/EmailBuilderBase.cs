using System.IO;
using System.Collections;
using System.Text;
using TourWriter.Global;
using TourWriter.Services;

namespace TourWriter.Modules.Emailer
{
	public abstract class EmailBuilderBase
	{
        protected const string TAB = "\t";
        protected const string NEW_LINE = "\r\n";
		private IList _sourceList;
		private IList _emailList;
		public string TemplateBody = "";
		public string TemplateSubject = "";		
		public string TemplateFrom = Cache.User.Email;		
		public string TemplateBcc = "";
        public bool SaveSendMessages;
        public bool ShowBookingPrice;
		private int currentIndex = 0;

		public IList SourceList
		{
			get
			{
				if(_sourceList == null)
					_sourceList = new ArrayList();

				return _sourceList; 
			}
		}

		public IList EmailList
		{
			get
			{
				if(_emailList == null)
					_emailList = new ArrayList();

				return _emailList; 
			}
		}
		
		public EmailMessage CurrentEmail
		{
			get{ return EmailList[currentIndex] as EmailMessage; }
			set
			{
				for(int i=0; i<EmailList.Count; i++)
					if(EmailList[i] == value)
					{
						currentIndex = i;
						break;
					}
			}
		}

		public int CurrentIndex
		{
			get
			{
				if(currentIndex > EmailList.Count-1)
					return EmailList.Count-1;
				return currentIndex;
			}
			set{ currentIndex = value; }
		}
		

		public abstract void BuildEmails();
	    
	    public abstract void RecordSavedEmail(EmailMessage email, string savedFilename);

		public void Remove(int index)
		{
			SourceList.RemoveAt(index);
			EmailList.RemoveAt(index);
		}

		public void Remove(EmailMessage email)
		{
			for(int i = EmailList.Count-1; i>-1; i--)
				if(EmailList[i] == email)
					Remove(i);
		}

		public static string ReadTemplateText(string templateFile)
		{
            if (!File.Exists(templateFile))
			{
                App.ShowInfo("The template file was not found, '" + templateFile + "'.");
				return "";
			}

			StreamReader sr = null;
            StringBuilder sb = new StringBuilder();
			try
            {
                string input;
                sr = File.OpenText(templateFile);
                
                while ((input = sr.ReadLine()) != null)
                    sb.AppendLine(input);
			}
			finally
			{
                if(sr != null)
				    sr.Close();
			}
            return sb.ToString();
		}

		public bool QueryContinueToCancelEmails()
		{
			bool continueCancel = true;

			if(EmailList.Count > 0)
			{
				System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(
					"Cancel unsent emails?\r\nClick Yes to cancel the emails or No to return.",
					App.MessageCaption,
					System.Windows.Forms.MessageBoxButtons.YesNo, 
					System.Windows.Forms.MessageBoxIcon.Question, 
					System.Windows.Forms.MessageBoxDefaultButton.Button1);
				
				continueCancel = (result == System.Windows.Forms.DialogResult.Yes);
			}
			return continueCancel;
		}

		public bool QueryContinueToFinishEmails()
		{
			bool continueFinish = true;

			if(EmailList.Count > 0)
			{
				System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(
					"Send unsent emails?\r\nClick Yes to send the emails or No to return.",
					App.MessageCaption,
					System.Windows.Forms.MessageBoxButtons.YesNo, 
					System.Windows.Forms.MessageBoxIcon.Question, 
					System.Windows.Forms.MessageBoxDefaultButton.Button1);
				
				continueFinish = (result == System.Windows.Forms.DialogResult.Yes);
			}
			return continueFinish;
		}
	}
}
