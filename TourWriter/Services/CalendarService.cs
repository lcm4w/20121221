using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Info;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;

namespace TourWriter.Services
{
    class CalendarService
    {
        private string _calendarURI;
        private string _username;
        private string _password;
        private Google.GData.Calendar.CalendarService _service;
        private EventQuery _query;
        private ArrayList _entryList;
        private DateTime[] _datesWithEvents;

        public ArrayList EntryList
        {
            get { return this._entryList; }
        }

        public DateTime[] DatesWithEvents
        {
            get { return this._datesWithEvents; }
        }

        /// <summary>
        /// Creates an instance of Google Calendar Service.
        /// </summary>
        /// <param name="strUsername">Google username</param>
        /// <param name="strPassword">Google password</param>
        /// <param name="strCalendarURI"></param>
        public CalendarService(string strUsername, string strPassword, string strCalendarURI = "https://www.google.com/calendar/feeds/default/private/full")
        {
            this._entryList = null;
            this._datesWithEvents = null;
            this._query = null;

            this._calendarURI = strCalendarURI;
            this._username = strUsername;
            this._password = strPassword;

            this._service = new Google.GData.Calendar.CalendarService("TourwriterGoogleCalendar");

            if (this._username != null && this._username.Length > 0)
            {
                try
                {
                    this._service.setUserCredentials(this._username, this._password);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                throw (new Exception("Username or password is blank."));
        }

        /// <summary>
        /// Gets all Google Calendar events.
        /// </summary>
        /// <param name="dtStartDate"></param>
        /// <param name="dtEndDate"></param>
        /// <param name="cal">Month Calendar control</param>
        /// <returns></returns>
        public int RefreshAllEvents(DateTime dtStartDate, DateTime dtEndDate, MonthCalendar cal = null)
        {
            this._entryList = new ArrayList(100);
            ArrayList dates = new ArrayList(100);
            this._query = new EventQuery();
            
            this._query.Uri = new Uri(this._calendarURI);
            this._query.StartTime = dtStartDate;
            this._query.EndTime = dtEndDate;

            EventFeed calFeed = this._service.Query(this._query) as EventFeed;

            while (calFeed != null && calFeed.Entries.Count > 0)
            {
                foreach (EventEntry entry in calFeed.Entries)
                {
                    this._entryList.Add(entry);
                    if (entry.Times.Count > 0)
                    {
                        foreach (When w in entry.Times)
                        {
                            dates.Add(w.StartTime);
                        }
                    }
                }
                if (calFeed.NextChunk != null)
                {
                    this._query.Uri = new Uri(calFeed.NextChunk);
                    calFeed = this._service.Query(this._query) as EventFeed;
                }
                else
                    calFeed = null;
            }

            this._datesWithEvents = new DateTime[dates.Count];

            int i = 0;
            foreach (DateTime d in dates)
            {
                this._datesWithEvents[i++] = d;
            }

            if (cal != null && dates.Count > 0)
                cal.BoldedDates = this._datesWithEvents;

            return dates.Count;
        }

        /// <summary>
        /// Get Google calendar events for the selected date.
        /// </summary>
        /// <param name="dtSelectedDate">Selected date in the calendar</param>
        /// <param name="lview">List View control</param>
        /// <returns></returns>
        public CalendarEventInfo[] GetEventPerDate(DateTime dtSelectedDate, ListView lview = null)
        {
            CalendarEventInfo[] calEvent = null;

            if (this._entryList != null)
            {
                if (lview != null)
                    lview.Items.Clear();

                ArrayList results = new ArrayList(10);
                foreach (EventEntry entry in this._entryList)
                {
                    if (entry.Times.Count > 0)
                    {
                        foreach (When w in entry.Times)
                        {
                            if (dtSelectedDate == w.StartTime.Date || dtSelectedDate == w.EndTime.Date)
                            {
                                results.Add(entry);
                                break;
                            }
                        }
                    }
                }

                int i = 0;
                calEvent = new CalendarEventInfo[results.Count];
                foreach (EventEntry entry in results)
                {
                    calEvent[i] = new CalendarEventInfo(entry);

                    if (lview != null)
                    {
                        ListViewItem item = new ListViewItem(calEvent[i].EventName);
                        item.SubItems.Add(calEvent[i].AuthorName);
                        if (calEvent[i].TimeCount > 0)
                        {
                            item.SubItems.Add(calEvent[i].StartTime.TimeOfDay.ToString());
                            item.SubItems.Add(calEvent[i].EndTime.TimeOfDay.ToString());
                        }

                        lview.Items.Add(item);
                    }
                }
            }
            return calEvent;
        }

        /// <summary>
        /// Retrieves a Google calendar event that matches the full text query.
        /// </summary>
        /// <param name="strText">Full text query</param>
        /// <returns></returns>
        public EventEntry GetEntryByText(string strText)
        {
            EventEntry match = null;
            try
	        {
                EventQuery query = new EventQuery(this._calendarURI);
                query.Query = strText;
                EventFeed resultsFeed = this._service.Query(query);
                if (resultsFeed.Entries.Count > 0)
                    match = (EventEntry)resultsFeed.Entries[0];
	        }
	        catch (Exception ex)
	        {
		        throw ex;
	        }
            return match;
        }

        /// <summary>
        /// Checks if a Google calendar event name matches the full text query.
        /// </summary>
        /// <param name="strText">Full text query</param>
        /// <returns></returns>
        public bool IsExisting(string strText)
        {
            if (GetEntryByText(strText) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a Google calendar event.
        /// </summary>
        /// <param name="strEventName"></param>
        /// <param name="strDescription"></param>
        /// <param name="strLocation"></param>
        /// <param name="dtStartDate"></param>
        /// <param name="dtEndDate"></param>
        /// <returns></returns>
        public int CreateEvent(string strEventName, string strDescription, string strLocation, DateTime dtStartDate, DateTime dtEndDate)
        {
            try
            {
                EventEntry entry = new EventEntry();
                entry.Title.Text = strEventName;
                entry.Content.Content = strDescription;
                if (strLocation.Length > 0)
                {
                    Where eventLocation = new Where();
                    eventLocation.ValueString = strLocation;
                    entry.Locations.Add(eventLocation);
                }
                When eventTime = new When(dtStartDate, dtEndDate);
                entry.Times.Add(eventTime);

                AtomEntry insertedEntry = this._service.Insert(this._calendarURI, entry);
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates a Google calendar event based on the event name.
        /// </summary>
        /// <param name="strEventName">Event name (cannot be edited)</param>
        /// <param name="strDescription"></param>
        /// <param name="strLocation"></param>
        /// <param name="dtStartDate"></param>
        /// <param name="dtEndDate"></param>
        /// <returns></returns>
        public int UpdateEvent(string strEventName, string strDescription, string strLocation, DateTime dtStartDate, DateTime dtEndDate)
        {
            try
            {
                EventEntry entry = GetEntryByText(strEventName);
                if (entry != null)
                {
                    entry.Content.Content = strDescription;
                    if (strLocation.Length > 0)
                    {
                        Where eventLocation = new Where();
                        eventLocation.ValueString = strLocation;
                        entry.Locations.Clear();
                        entry.Locations.Add(eventLocation);
                    }
                    When eventTime = new When(dtStartDate, dtEndDate);
                    entry.Times.Clear();
                    entry.Times.Add(eventTime);
                    entry.Update();
                    return 1;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
    }
}