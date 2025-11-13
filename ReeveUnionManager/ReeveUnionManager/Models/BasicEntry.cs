using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("basic_entries")]
    public class BasicEntry : ObservableBaseModel, IEquatable<BasicEntry>
    {
        // Backing fields
        int _entryId = -1;
        string _section = "";
        string _notes = "";
        string[] _images = {};

        [PrimaryKey("id", true)]
        public int EntryId
        {
            get => _entryId;
            set => SetProperty(ref _entryId, value);
        }

        [Column("section")]
        public string Section
        {
            get => _section;
            set => SetProperty(ref _section, value);
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

        public BasicEntry() { }

        public BasicEntry(int entryId, string section, string notes, string[] images)
        {
            EntryId = entryId;
            Section = section;
            Notes = notes;
            Images = images;
        }

        public override string ToString()
            => $"ID: {EntryId}, Section: {Section}, Notes: {Notes}";


        public override bool Equals(object? obj)
            => obj is BasicEntry other && EntryId == other.EntryId;

        public bool Equals(BasicEntry? other)
            => other is not null && EntryId == other.EntryId;

        public override int GetHashCode()
            => EntryId.GetHashCode();
    }
}