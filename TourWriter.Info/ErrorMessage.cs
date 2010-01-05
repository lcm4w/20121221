using System;

namespace TourWriter.Info
{
    [Serializable]
    public class ErrorMessage
    {
        // tourwriter
        public string ErrorType;
        public string TimeStamp;
        public string UtcTime;
        public string InstallId;
        public string AppVersion;
        public string Revision;
        public string DbVersion;
        public string Connection;

        // system
        public string ApplicationName;
        public string ComputerName;
        public string UserName;
        public string OsVersion;
        public string NetVersion;
        public string Culture;
        public string Resolution;
        public string ApplicationUpTime;

        // message
        public string Message;
        public string Detail;
    }
}