using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace ReeveUnionManager.Models
{
    [Table("log_sheet")]
    public class LogSheet : ObservableBaseModel, IEquatable<CallLog>
    {
        // Backing fields
        DateTime _date = DateTime.MinValue;

        public bool Equals(CallLog? other)
        {
            throw new NotImplementedException();
        }
    }
}