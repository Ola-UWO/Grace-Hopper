using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("event_support")]
    public class EventSupportChange : ObservableBaseModel, IEquatable<EventSupportChange>
    {
        // Backing fields
        string _name = "";
        TimeOnly _time = new TimeOnly(0, 0);
        string _location = "";
        string _notes = "";
        string[] _images = {};

        [PrimaryKey("id", false)]
        public Guid EntryId
        {
            get;
            set;
        }

        [Column("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [Column("time")]
        public TimeOnly Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        [Column("location")]
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        [Column("notes")]
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        [Column("images")]
        public string[] Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        public EventSupportChange() { }

        public EventSupportChange(Guid entryId, string name, TimeOnly time, string location, string notes, string[] images)
        {
            EntryId = entryId;
            Name = name;
            Time = time;
            Location = location;
            Notes = notes;
            Images = images;
        }

        public override string ToString()
            => $"ID: {EntryId}, Name: {Name}, Time: {Time}, Location: {Location}, Notes: {Notes}";


        public override bool Equals(object? obj)
            => obj is EventSupportChange other && EntryId == other.EntryId;

        public bool Equals(EventSupportChange? other)
            => other is not null && EntryId == other.EntryId;

        public override int GetHashCode()
            => EntryId.GetHashCode();
    }
}