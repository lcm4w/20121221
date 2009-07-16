using System;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    public class TemplateSettings
    {
        private string from;
        private string bcc;
        private string subject;
        private string body;
        private bool showPrices;
        private bool saveToFile;
        private bool readReceipt;

        public string From
        {
            get { return from; }
            set { from = value; }
        }

        public string Bcc
        {
            get { return bcc; }
            set { bcc = value; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public bool ShowPrices
        {
            get { return showPrices; }
            set { showPrices = value; }
        }

        public bool SaveToFile
        {
            get { return saveToFile; }
            set { saveToFile = value; }
        }

        public bool ReadReceipt
        {
            get { return readReceipt; }
            set { readReceipt = value; }
        }

        public static string ReplaceBodyHtml(string documentHtml, string newBodyHtml)
        {
            if (!documentHtml.ToLower().Contains("<body") ||
                !documentHtml.ToLower().Contains("</body>"))
            {
                return String.Empty;
            }

            int firstBodyIndex = documentHtml.ToLower().IndexOf("<body");
            string documentOpeningTags = documentHtml.Substring(0, firstBodyIndex);

            int lastBodyIndex = documentHtml.ToLower().LastIndexOf("</body>") + 7;
            string documentClosingTags = documentHtml.Substring(lastBodyIndex, documentHtml.Length - lastBodyIndex);

            return documentOpeningTags + newBodyHtml + documentClosingTags;
        }
    }
}
