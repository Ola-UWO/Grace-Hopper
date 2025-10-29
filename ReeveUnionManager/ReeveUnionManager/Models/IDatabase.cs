using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ReeveUnionManager.Models;

public interface IDatabase
{
	Task<ObservableCollection<CallLog>> SelectAllCallLogs();
	Task<CallLog?> SelectCallLog(int callId);
	Task<CallLogError> InsertCallLog(CallLog callLog);
	Task<ObservableCollection<CheckInLog>> SelectAllCheckInLogs();
	Task<CheckInLog?> SelectCheckInLog(int checkInId);
	Task<CheckInLogError> InsertCheckInLog(CheckInLog checkInLog);

}