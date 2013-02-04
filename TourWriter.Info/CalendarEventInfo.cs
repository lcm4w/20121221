using System;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;

namespace TourWriter.Info
{
    public class CalendarEventInfo
    {
        public string EventName;
        public string AuthorName;
        public int TimeCount;
        public DateTime StartTime;
        public DateTime EndTime;
        public string Location;
        public string Comments;
        public string Content;

        public CalendarEventInfo(EventEntry entry)
        {
            EventName = entry.Title.Text;
            AuthorName = entry.Authors[0].Name;
            TimeCount = entry.Times.Count;
            StartTime = entry.Times[0].StartTime;
            EndTime = entry.Times[0].EndTime;
            Location = entry.Locations[0].ValueString;
            Comments = entry.Comments.ToString();
            Content = entry.Content.Content;
        }
    }
}