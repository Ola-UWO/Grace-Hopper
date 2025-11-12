using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
#if ANDROID
using Android.Telecom;
using Android.Text.Format;
#endif
using ReeveUnionManager.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net;
using System.Diagnostics;

namespace ReeveUnionManager.Models;

public class BusinessLogic : IBusinessLogic
{
    private readonly IDatabase _database;
    public ObservableCollection<CallLog> CallLogs { get; set; }
    public ObservableCollection<CheckInLog> CheckInLogs { get; set; }
    public ObservableCollection<ScrapeEvent> ScrapeEvents {get; set; }

    public BusinessLogic(IDatabase database)
    {
        _database = database;
        CallLogs = new ObservableCollection<CallLog>();
        CheckInLogs = new ObservableCollection<CheckInLog>();

        LoadAllDataAsync();
    }

    /// <summary>
    /// Loads every class all at once
    /// </summary>
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

    /// <summary>
    /// Helper method to load all classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="loadFunc"></param>
    /// <returns></returns>
    private async Task LoadCollectionAsync<T>(ObservableCollection<T> collection, Func<Task<ObservableCollection<T>>> loadFunc)
    {
        var items = await loadFunc();
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Gets all entries into CallLog
    /// </summary>
    /// <returns>Every callLog</returns>
    public async Task<ObservableCollection<CallLog>> GetCallLogs()
    {
        return await _database.SelectAllCallLogs();
    }

    /// <summary>
    /// Adds a call log to the collection
    /// </summary>
    /// <param name="callId">Unique identifier for a call log</param>
    /// <param name="callerName">Name of who called</param>
    /// <param name="timeOfCall">The time of the call</param>
    /// <param name="callNotes">Notes about the call</param>
    /// <returns></returns>
    public async Task<CallLogError> AddCallLog(Guid callId, string callerName, string timeOfCall, string callNotes)
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

    /// <summary>
    /// Finds a specific call log
    /// </summary>
    /// <param name="callId">Unique identifier for a call log</param>
    /// <returns>A specific call Log</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<CallLog> FindCallLog(Guid callId)
    {
        CallLog? cl = await _database.SelectCallLog(callId);

        return cl;
    }

    /// <summary>
    /// Deletes a call log
    /// </summary>
    /// <param name="callId">Call log to be deleted</param>
    /// <returns>Whether the delete was successful</returns>
    public async Task<CallLogError> DeleteCallLog(Guid callId)
    {
        var callLog = CallLogs.FirstOrDefault(cl => cl.CallId == callId);
        if (callLog == null)
        {
            return CallLogError.CallLogIdNotFound;
        }

        await _database.DeleteCallLog(callId);
        CallLogs.Remove(callLog);

        return CallLogError.None;
    }

    /// <summary>
    /// Delete all call logs
    /// </summary>
    /// <returns>Whether the delete was successful or not</returns>
    public async Task<CallLogError> DeleteAllCallLogs()
    {
        try
        {
            var callsToDelete = CallLogs.ToList();
            foreach (var cl in callsToDelete)
            {
                var result = await DeleteCallLog(cl.CallId);
                if (result != CallLogError.None)
                {
                    return result;
                }
            }
            return CallLogError.None;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting all clubs -- {ex}");
            return CallLogError.DeleteError;
        }
    }

    /// <summary>
    /// Gets all check in logs
    /// </summary>
    /// <returns>All check in logs</returns>
    public async Task<ObservableCollection<CheckInLog>> GetCheckInLogs()
    {
        return await _database.SelectAllCheckInLogs();
    }

    public async Task<CheckInLogError> AddCheckInLog(Guid checkInId, string checkInName, string checkInTime, string checkInLocation, string checkInNotes)
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

        if (checkInLocation == "")
        {
            return CheckInLogError.MissingLocation;
        }

        var newCheckInLog = new CheckInLog
        {
            CheckInId = checkInId,
            CheckInName = checkInName,
            TimeOfCheckIn = checkInTime,
            CheckInLocation = checkInLocation,
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

    /// <summary>
    /// Finds a specific check in log
    /// </summary>
    /// <param name="checkInId">The unique id for a check in log</param>
    /// <returns>The check in log specified</returns>
    public async Task<CheckInLog> FindCheckInLog(Guid checkInId)
    {
        CheckInLog? cil = await _database.SelectCheckInLog(checkInId);

        return cil;
    }

    /// <summary>
    /// Deletes a check in log
    /// </summary>
    /// <param name="checkInId">The unique identifier for a check in id</param>
    /// <returns>Whether the delete was successful</returns>
    public async Task<CheckInLogError> DeleteCheckInLog(Guid checkInId)
    {
        var checkInLog = CheckInLogs.FirstOrDefault(cil => cil.CheckInId == checkInId);
        if (checkInLog == null)
        {
            return CheckInLogError.CheckInLogIdNotFound;
        }

        await _database.DeleteCheckInLog(checkInId);
        CheckInLogs.Remove(checkInLog);

        return CheckInLogError.None;
    }

    /// <summary>
    /// Deletes all check in logs
    /// </summary>
    /// <returns>Whether the delete was successful</returns>
    public async Task<CheckInLogError> DeleteAllCheckInLogs()
    {
        try
        {
            var checkInsToDelete = CheckInLogs.ToList();
            foreach (var cil in checkInsToDelete)
            {
                var result = await DeleteCheckInLog(cil.CheckInId);
                if (result != CheckInLogError.None)
                {
                    return result;
                }
            }
            return CheckInLogError.None;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting all clubs -- {ex}");
            return CheckInLogError.DeleteError;
        }
    }

    public async Task<CallLogError> AddBasicEntry(string title, string notes, ObservableCollection<PhotoInfo> photos)
    {
        
        return CallLogError.None;
        
    }

    public async Task<ScrapeEventError> Scrape25Live()
    {
        try
        {
            await _database.DeleteAllEvents();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting all events -- {ex}");
        }

        var doc = XDocument.Load("https://25livepub.collegenet.com/calendars/2016-today-in-reeve.rss");

        var scrapeEvents = doc.Descendants("item").Select(item =>
        {
            string description = (string)item.Element("description") ?? "";
            description = CleanHtmlEntities(description);

            string[] parts = description.Split(["<br/>", "<br />"], StringSplitOptions.None);

            string location = parts.Length > 0 ? CleanHtmlEntities(parts[0]) : string.Empty;
            string dateAndTime = parts.Length > 1 ? CleanHtmlEntities(parts[1]) : string.Empty;

            if (dateAndTime.Contains('—'))
            {
                dateAndTime = dateAndTime.TrimEnd('—').Trim();
            }

            return new ScrapeEvent
            {
                EventId = Guid.NewGuid(),
                EventTitle = (string)item.Element("title"),
                EventLocation = location,
                EventDateAndTime = dateAndTime
            };
        });

        foreach (var ev in scrapeEvents)
        {
            try
            {
                Debug.WriteLine("Before DB Insert");
                await _database.InsertEvent(ev);
                Debug.WriteLine("After DB Insert");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting event -- {ex}");
                return ScrapeEventError.InsertionError;
            }
        }
        return ScrapeEventError.None;
    }
    
    private static string CleanHtmlEntities(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        string decoded = WebUtility.HtmlDecode(input);

        decoded = decoded
            .Replace('\u00A0', ' ')
            .Replace('\u2013', '-')
            .Replace('\u2014', '-')
            .TrimEnd('-', '–', '—', ' ')
            .Trim();

        return decoded;
    }
}
