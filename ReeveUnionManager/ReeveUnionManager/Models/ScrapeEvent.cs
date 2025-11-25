using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("scrape_events")]
    public class ScrapeEvent : ObservableBaseModel, IEquatable<ScrapeEvent>
    {
        // Backing fields
        Guid _eventId = Guid.Empty;
        string _eventTitle = "";
        string _eventLocation = "";
        string _eventDateAndTime = "";
        string _eventNotes = "";

        // [Column("id")]
        // public Guid Id { get; set; }
        [PrimaryKey("event_id", true)]
        public Guid EventId
        {
            get => _eventId;
            set => SetProperty(ref _eventId, value);
        }

        [Column("event_title")]
        public string EventTitle
        {
            get => _eventTitle;
            set => SetProperty(ref _eventTitle, value);
        }

        [Column("event_location")]
        public string EventLocation
        {
            get => _eventLocation;
            set => SetProperty(ref _eventLocation, value);
        }

        [Column("event_date_and_time")]
        public string EventDateAndTime
        {
            get => _eventDateAndTime;
            set => SetProperty(ref _eventDateAndTime, value);
        }

        [Column("event_notes")]
        public string EventNotes
        {
            get => _eventNotes;
            set => SetProperty(ref _eventNotes, value);
        }

        public ScrapeEvent() { }

        public ScrapeEvent(string eventTitle, string eventLocation, string eventDateAndTime, string eventNotes)
        {
            EventTitle = eventTitle;
            EventLocation = eventLocation;
            EventDateAndTime = eventDateAndTime;
            EventNotes = eventNotes;
        }

        public override string ToString()
            => $"Event title: {EventTitle}, Location: {EventLocation}, Time: {EventDateAndTime}";

        public bool Equals(ScrapeEvent? other)
        {
            return EventId == other.EventId;
        }

        public bool HasNotes => !string.IsNullOrWhiteSpace(EventNotes);

    }
}
