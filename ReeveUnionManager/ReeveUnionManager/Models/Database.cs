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

	public async Task<CallLog?> SelectCallLog(int callId)
	{
		var response = await supabaseClient.From<CallLog>().Where(callLog => callLog.CallId == callId).Get();
		if (response != null)
		{
			return response!.Models.FirstOrDefault();
		}
		return null;
	}

	public async Task<CallLogError> InsertCallLog(CallLog callLog)
    {
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

	public async Task<CheckInLog?> SelectCheckInLog(int checkInId)
	{
		var response = await supabaseClient.From<CheckInLog>().Where(checkInLog => checkInLog.CheckInId == checkInId).Get();
		if (response != null)
		{
			return response!.Models.FirstOrDefault();
		}
		return null;
	}
	
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
}