using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
#if ANDROID
using Android.Telecom;
using Android.Text.Format;
#endif
using ReeveUnionManager.Models;

namespace ReeveUnionManager.Models;

public class BusinessLogic : IBusinessLogic
{
    private readonly IDatabase _database;
    public ObservableCollection<CallLog> CallLogs { get; set; }
    public ObservableCollection<CheckInLog> CheckInLogs { get; set; }

    public BusinessLogic(IDatabase database)
    {
        _database = database;
        CallLogs = new ObservableCollection<CallLog>();
        CheckInLogs = new ObservableCollection<CheckInLog>();

        LoadAllDataAsync();
    }

    private async void LoadAllDataAsync()
    {
        try
        {
            await Task.WhenAll(
                LoadCollectionAsync(CallLogs, _database.SelectAllCallLogs),
                LoadCollectionAsync(CheckInLogs, _database.SelectAllCheckInLogs)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex}");
        }
    }

    private async Task LoadCollectionAsync<T>(ObservableCollection<T> collection, Func<Task<ObservableCollection<T>>> loadFunc)
    {
        var items = await loadFunc();
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public async Task<ObservableCollection<CallLog>> GetCallLogs()
    {
        return await _database.SelectAllCallLogs();
    }

    public async Task<CallLogError> AddCallLog(int callId, string callerName, string timeOfCall, string callNotes)
    {
        CallLog? existingCallLog = await _database.SelectCallLog(callId);
        if (existingCallLog != null)
        {
            return CallLogError.DuplicateCallId;
        }

        if (string.IsNullOrWhiteSpace(callerName))
        {
            return CallLogError.NameTooShort;
        }

        if (timeOfCall == null)
        {
            return CallLogError.MissingDate;
        }

        Console.WriteLine(3);

        var newCallLog = new CallLog
        {
            CallId = callId,
            CallerName = callerName,
            TimeOfCall = timeOfCall,
            CallNotes = callNotes
        };

        Console.WriteLine(4);

        try
        {
            await _database.InsertCallLog(newCallLog);
        }
        catch (Exception ex)
        {
            Console.Write($"Attention: {ex.ToString()}");
        }
        CallLogs.Add(newCallLog);

        return CallLogError.None;
    }

    public Task<CallLog> FindCallLog(int callId)
    {
        throw new NotImplementedException();
    }

    public async Task<ObservableCollection<CheckInLog>> GetCheckInLogs()
    {
        return await _database.SelectAllCheckInLogs();
    }

    public async Task<CheckInLogError> AddCheckInLog(int checkInId, string checkInName, string checkInTime, string checkInNotes)
    {
        CheckInLog? existingCheckInLog = await _database.SelectCheckInLog(checkInId);
        if (existingCheckInLog != null)
        {
            return CheckInLogError.DuplicateCheckInId;
        }

        if (string.IsNullOrWhiteSpace(checkInName))
        {
            return CheckInLogError.NameTooShort;
        }

        if (checkInTime == "")
        {
            return CheckInLogError.MissingDate;
        }

        var newCheckInLog = new CheckInLog
        {
            CheckInId = checkInId,
            CheckInName = checkInName,
            TimeOfCheckIn = checkInTime,
            CheckInNotes = checkInNotes
        };
        try
        {
            await _database.InsertCheckInLog(newCheckInLog);
        }
        catch (Exception ex)
        {
            Console.Write($"Attention: {ex.ToString()}");
        }
        CheckInLogs.Add(newCheckInLog);

        return CheckInLogError.None;
    }

    public Task<CheckInLog> FindCheckInLog(int checkInId)
    {
        throw new NotImplementedException();
    }
}
