using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ReeveUnionManager.Views;

namespace ReeveUnionManager.Models;

public interface IBusinessLogic
{
    Task<ObservableCollection<CallLog>> GetCallLogs();
    Task<CallLogError> AddCallLog(Guid callId, string callerName, string timeOfCall, string callNotes);
    Task<CallLog> FindCallLog(Guid callId);
    Task<CallLogError> DeleteCallLog(Guid callId);
    Task<CallLogError> DeleteAllCallLogs();
    Task<ObservableCollection<CheckInLog>> GetCheckInLogs();
    Task<CheckInLogError> AddCheckInLog(Guid checkInId, string checkInName, string checkInTime, string checkInLocation, string checkInNotes);
    Task<CheckInLog> FindCheckInLog(Guid checkInId);
    Task<CheckInLogError> DeleteCheckInLog(Guid checkInId);
    Task<CheckInLogError> DeleteAllCheckInLogs();
    Task<ScrapeEventError> Scrape25Live();
}
