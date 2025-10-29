using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ReeveUnionManager.Views;

namespace ReeveUnionManager.Models;

public interface IBusinessLogic
{
    Task<ObservableCollection<CallLog>> GetCallLogs();
    Task<CallLogError> AddCallLog(int callId, string callerName, DateTime timeOfCall, string callNotes);
    Task<CallLog> FindCallLog(int callId);
    Task<ObservableCollection<CheckInLog>> GetCheckInLogs();
    Task<CheckInLogError> AddCheckInLog(int checkInId, string checkInName, DateTime checkInTime, string checkInNotes);
    Task<CheckInLog> FindCheckInLog(int checkInId);
}
