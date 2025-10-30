using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using ReeveUnionManager.Models;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Gotrue;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ReeveUnionManager.Views;

namespace ReeveUnionManager.Models;

public class Database : IDatabase
{
	private Supabase.Client? supabaseClient;
	private ObservableCollection<CallLog> callLogs = new();
	private ObservableCollection<CheckInLog> checkInLogs = new();
	private Task waitingForInitialization;

	public Database()
	{
		waitingForInitialization = InitializeSupabaseSystems();
	}

	private async Task InitializeSupabaseSystems()
	{
		// Initialize Supabase client
		var supabaseUrl = "https://yoxlpzwqwijxlbnszvlb.supabase.co";
		var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlveGxwendxd2lqeGxibnN6dmxiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA2NTU3NzYsImV4cCI6MjA3NjIzMTc3Nn0.TguOcwCauLJrF81koLj-QYJ2uVpOVg_cxkbr3k1GESY";


		supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
		await supabaseClient.InitializeAsync();
	}

	/// <summary>
    /// Selects all call logs
    /// </summary>
    /// <returns>Every call log</returns>
	public async Task<ObservableCollection<CallLog>> SelectAllCallLogs()
	{
		await waitingForInitialization;
		var table = supabaseClient!.From<CallLog>();
		var response = await table.Get();
		callLogs.Clear();
		foreach (CallLog cl in response.Models)
		{
			callLogs.Add(cl);
		}
		return callLogs;
	}

	/// <summary>
    /// Selects a specific call log
    /// </summary>
    /// <param name="callId">Unique identifier for the call log</param>
    /// <returns>The call log specified</returns>
	public async Task<CallLog?> SelectCallLog(int callId)
	{
		var response = await supabaseClient.From<CallLog>().Where(callLog => callLog.CallId == callId).Get();
		if (response != null)
		{
			return response!.Models.FirstOrDefault();
		}
		return null;
	}

	/// <summary>
	/// Adds a call log to the database
	/// </summary>
	/// <param name="callLog">A complete call log</param>
	/// <returns>Whether the insert was successful</returns>
	public async Task<CallLogError> InsertCallLog(CallLog callLog)
	{
		await waitingForInitialization;

		try
		{
			await supabaseClient.From<CallLog>().Insert(callLog);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
			return CallLogError.InsertionError;
		}

		return CallLogError.None;
	}
	
	/// <summary>
    /// Deletes a call log
    /// </summary>
    /// <param name="callId">Unique identifier for a call log</param>
    /// <returns>Whether the delete was successful</returns>
	public async Task<CallLogError> DeleteCallLog(int callId)
    {
        try
        {
            var unused = await supabaseClient.From<CallLog>().Delete(await SelectCallLog(callId));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: Error while deleting -- {ex.ToString()}");
            return CallLogError.DeleteError;
        }
        return CallLogError.None;
    }

	/// <summary>
    /// Selects all check in logs
    /// </summary>
    /// <returns>Every check in log</returns>
	public async Task<ObservableCollection<CheckInLog>> SelectAllCheckInLogs()
	{
		await waitingForInitialization;
		var table = supabaseClient!.From<CheckInLog>();
		var response = await table.Get();
		checkInLogs.Clear();
		foreach (CheckInLog cil in response.Models)
		{
			checkInLogs.Add(cil);
		}
		return checkInLogs;
	}

	/// <summary>
    /// Selects a specific check in log
    /// </summary>
    /// <param name="checkInId">Unique identifier for a check in log</param>
    /// <returns>The specified check in log</returns>
	public async Task<CheckInLog?> SelectCheckInLog(int checkInId)
	{
		var response = await supabaseClient.From<CheckInLog>().Where(checkInLog => checkInLog.CheckInId == checkInId).Get();
		if (response != null)
		{
			return response!.Models.FirstOrDefault();
		}
		return null;
	}

	/// <summary>
	/// Inserts a check in log
	/// </summary>
	/// <param name="checkInLog">Complete check in log to be inserted</param>
	/// <returns>Whether the insert was successful</returns>
	public async Task<CheckInLogError> InsertCheckInLog(CheckInLog checkInLog)
	{
		try
		{
			await supabaseClient.From<CheckInLog>().Insert(checkInLog);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
			return CheckInLogError.InsertionError;
		}

		return CheckInLogError.None;
	}
	
	/// <summary>
    /// Deletes a specified check in log
    /// </summary>
    /// <param name="checkInId">Unique identifier for check in id to be deleted</param>
    /// <returns>Whether the delete was successful</returns>
	public async Task<CheckInLogError> DeleteCheckInLog(int checkInId)
    {
        try
        {
            var unused = await supabaseClient.From<CheckInLog>().Delete(await SelectCheckInLog(checkInId));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: Error while deleting -- {ex.ToString()}");
            return CheckInLogError.DeleteError;
        }
        return CheckInLogError.None;
    }
}