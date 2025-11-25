using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ReeveUnionManager.Models;

public interface IDatabase
{
	Task<ObservableCollection<CallLog>> SelectAllCallLogs();
	Task<CallLog?> SelectCallLog(Guid callId);
	Task<CallLogError> InsertCallLog(CallLog callLog);
	Task<CallLogError> DeleteCallLog(Guid callId);
	Task<ObservableCollection<CheckInLog>> SelectAllCheckInLogs();
	Task<CheckInLog?> SelectCheckInLog(Guid checkInId);
	Task<CheckInLogError> InsertCheckInLog(CheckInLog checkInLog);
	Task<CheckInLogError> DeleteCheckInLog(Guid checkInId);
	Task<BasicEntryError> InsertBasicEntry(BasicEntry entry);
	Task<string[]> UploadPhotosAsync(ObservableCollection<PhotoInfo> photos);
	Task<ScrapeEventError> InsertEvent(ScrapeEvent scrapeEvent);
	Task<ScrapeEventError> DeleteAllEvents();
	Task<BasicEntryError> InsertFoodIssue(FoodServiceIssue issue);
	Task<BasicEntryError> InsertEventSupportChange(EventSupportChange change);
}