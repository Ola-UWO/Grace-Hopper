using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("phone_call_logs")]
    public class CallLog : ObservableBaseModel, IEquatable<CallLog>
    {
        // Backing fields
        int _callId = -1;
        string _callerName = "";
        DateTime _timeOfCall = DateTime.MinValue;
        string _callNotes = "";

        [PrimaryKey("call_id", true)]
        public int CallId
        {
            get => _callId;
            set => SetProperty(ref _callId, value);
        }

        [Column("caller_name")]
        public string CallerName
        {
            get => _callerName;
            set => SetProperty(ref _callerName, value);
        }

        [Column("time_of_call")]
        public DateTime TimeOfCall
        {
            get => _timeOfCall;
            set => SetProperty(ref _timeOfCall, value);
        }

        [Column("call_notes")]
        public string CallNotes
        {
            get => _callNotes;
            set => SetProperty(ref _callNotes, value);
        }

        public CallLog() { }

        public CallLog(int callId, string callerName, DateTime timeOfCall, string callNotes)
        {
            CallId = callId;
            CallerName = callerName;
            TimeOfCall = timeOfCall;
            CallNotes = callNotes;
        }

        public override string ToString()
            => $"Call ID: {CallId}, Caller Name: {CallerName}, Time: {TimeOfCall}, Call Notes: {CallNotes}";


        public override bool Equals(object? obj)
            => obj is CallLog other && CallId == other.CallId;

        public bool Equals(CallLog? other)
            => other is not null && CallId == other.CallId;

        public override int GetHashCode()
            => CallId.GetHashCode();
    }
}
