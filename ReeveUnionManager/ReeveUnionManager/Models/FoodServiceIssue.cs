using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("food_service")]
    public class FoodServiceIssue : ObservableBaseModel, IEquatable<FoodServiceIssue>
    {
        // Backing fields
        string _category = "";
        string _location = "";
        string _notes = "";
        string[] _images = {};

        [PrimaryKey("id", false)]
        public Guid EntryId
        {
            get;
            set;
        }

        [Column("category")]
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
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

        public FoodServiceIssue() { }

        public FoodServiceIssue(Guid entryId, string category, string location, string notes, string[] images)
        {
            EntryId = entryId;
            Category = category;
            Location = location;
            Notes = notes;
            Images = images;
        }

        public override string ToString()
            => $"ID: {EntryId}, Category: {Category}, Location: {Location}, Notes: {Notes}";


        public override bool Equals(object? obj)
            => obj is FoodServiceIssue other && EntryId == other.EntryId;

        public bool Equals(FoodServiceIssue? other)
            => other is not null && EntryId == other.EntryId;

        public override int GetHashCode()
            => EntryId.GetHashCode();
    }
}