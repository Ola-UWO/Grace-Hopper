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
    Task<BasicEntryError> AddBasicEntry(string title, string notes, ObservableCollection<PhotoInfo> photos);
    Task<ObservableCollection<ScrapeEvent>> GetAllEvents();
    Task<ScrapeEventError> Scrape25Live();
    Task<BasicEntryError> AddEventSupportChange(string name, TimeOnly time, string location, string notes, ObservableCollection<PhotoInfo> photos);
    Task<BasicEntryError> AddFoodIssue(string category, string location, string notes, ObservableCollection<PhotoInfo> photos);
}
