using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ReeveUnionManager.Views;

namespace ReeveUnionManager.Models;

public interface IBusinessLogic
{
    Task<ObservableCollection<CallLog>> GetCallLogs();
    Task<CallLogError> AddCallLog(int callId, string callerName, string timeOfCall, string callNotes);
    Task<CallLog> FindCallLog(int callId);
    Task<CallLogError> DeleteCallLog(int callId);
    Task<CallLogError> DeleteAllCallLogs();
    Task<ObservableCollection<CheckInLog>> GetCheckInLogs();
    Task<CheckInLogError> AddCheckInLog(int checkInId, string checkInName, string checkInTime, string checkInLocation, string checkInNotes);
    Task<CheckInLog> FindCheckInLog(int checkInId);
    Task<CheckInLogError> DeleteCheckInLog(int checkInId);
    Task<CheckInLogError> DeleteAllCheckInLogs();
}
