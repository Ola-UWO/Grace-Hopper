using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("manager_logs")]
    public class ManagerLog : ObservableBaseModel
    {
        // Primary key
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        // The log date (the one you want to show in the list)
        [Column("date")]
        public DateTime Date { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_edited")]
        public DateTime? LastEdited { get; set; }

        [Column("guest_count")]
        public int? GuestCount { get; set; }
    }
}
