using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("phone_call_logs")]
    public class CheckInLog : ObservableBaseModel, IEquatable<CheckInLog>
    {
        // Backing fields
        int _checkInId = -1;
        string _checkInName = "";
        DateTime _timeOfCheckIn = DateTime.MinValue;
        string _checkInNotes = "";

        [PrimaryKey("check_in_id", true)]
        public int CheckInId
        {
            get => _checkInId;
            set => SetProperty(ref _checkInId, value);
        }

        [Column("check_in_name")]
        public string CheckInName
        {
            get => _checkInName;
            set => SetProperty(ref _checkInName, value);
        }

        [Column("check_in_time")]
        public DateTime TimeOfCheckIn
        {
            get => _timeOfCheckIn;
            set => SetProperty(ref _timeOfCheckIn, value);
        }

        [Column("check_in_notes")]
        public string CheckInNotes
        {
            get => _checkInNotes;
            set => SetProperty(ref _checkInNotes, value);
        }

        public CheckInLog() { }

        public CheckInLog(int checkInId, string checkInName, DateTime timeOfCheckIn, string checkInNotes)
        {
            CheckInId = checkInId;
            CheckInName = checkInName;
            TimeOfCheckIn = timeOfCheckIn;
            CheckInNotes = checkInNotes;
        }

        public override string ToString()
            => $"Call ID: {CheckInId}, Caller Name: {CheckInName}, Time: {TimeOfCheckIn}, Call Notes: {CheckInNotes}";


        public override bool Equals(object? obj)
            => obj is CheckInLog other && CheckInId == other.CheckInId;

        public bool Equals(CheckInLog? other)
            => other is not null && CheckInId == other.CheckInId;

        public override int GetHashCode()
            => CheckInId.GetHashCode();
    }
}
