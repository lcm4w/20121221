using System;

namespace TourWriter.Info
{
    [Serializable]
    public class ErrorMessage
    {
        public string Type;
        public string InstallId;
        public string AppVersion;
        public string DbVersion;
        public string OsVersion;
        public string NetVersion;
        public string UserConnection;
        public string TimeStamp;
        public string Message;
        public string Detail;
    }
}