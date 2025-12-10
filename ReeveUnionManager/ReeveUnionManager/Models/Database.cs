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
using Supabase.Storage;
using Supabase.Storage.Interfaces;
using System.Diagnostics;


namespace ReeveUnionManager.Models;

public class Database : IDatabase
{
	private Supabase.Client? supabaseClient;
	private IStorageFileApi<FileObject>? imagesBucket;
	private ObservableCollection<CallLog> callLogs = new();
	private ObservableCollection<ScrapeEvent> scrapeEvents = new();
	private ObservableCollection<ManagerLog> managerLogs = new();
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
	public async Task<CallLog?> SelectCallLog(Guid callId)
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
	public async Task<CallLogError> DeleteCallLog(Guid callId)
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
	/// Inserts events into the database
	/// </summary>
	/// <param name="scrapeEvent">The event being inserted</param>
	/// <returns>Whether the insert was successful or not</returns>
	public async Task<ScrapeEventError> InsertEvent(ScrapeEvent scrapeEvent)

	{
		await waitingForInitialization;

		try
		{

			await supabaseClient.From<ScrapeEvent>().Insert(scrapeEvent);

		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");

			return ScrapeEventError.InsertionError;
		}

		return ScrapeEventError.None;
	}

	/// <summary>
    /// Selects all events from the database
    /// </summary>
    /// <returns></returns>
	public async Task<ObservableCollection<ScrapeEvent>> SelectAllEvents()
	{
		await waitingForInitialization;
		var table = supabaseClient!.From<ScrapeEvent>();
		var response = await table.Get();
		callLogs.Clear();
		foreach (ScrapeEvent sc in response.Models)
		{
			scrapeEvents.Add(sc);
		}
		return scrapeEvents;
	}

	/// <summary>
	/// Deletes all events stored in the database, uses a method designed to truncate the table
	/// </summary>
	/// <returns>Whether the task failed or succeeded</returns>
	public async Task<ScrapeEventError> DeleteAllEvents()
	{
		try
		{
			await supabaseClient.Rpc("truncate_events", new { });
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while deleting -- {ex.ToString()}");
			return ScrapeEventError.DeleteError;
		}
		return ScrapeEventError.None;
	}

	public async Task<ObservableCollection<ManagerLog>> SelectRecentManagerLogsAsync(int limit = 7)
	{
		await waitingForInitialization;

		var table = supabaseClient!.From<ManagerLog>();
		var response = await table.Get();

		managerLogs.Clear();

		// sort here to avoid needing extra Supabase/Postgrest ordering APIs here
		var ordered = response.Models
			.OrderByDescending(m => m.Date)
			.Take(limit);

		foreach (var log in ordered)
		{
			managerLogs.Add(log);
		}

		return managerLogs;
	}

	public async Task<BasicEntryError> InsertBasicEntry(BasicEntry entry)
	{
		await waitingForInitialization;

		try
		{
			await supabaseClient.From<BasicEntry>().Insert(entry);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
			return BasicEntryError.InsertionError;
		}

		return BasicEntryError.None;
	}
	
	//Takes collection of images and stores them in supabase storing their reference urls in an array that will be returned
	public async Task<string[]> UploadPhotosAsync(ObservableCollection<PhotoInfo> photos)
    {
		string[] uploadedUrls = new string[photos.Count];
		imagesBucket = supabaseClient.Storage.From("images");
		int count = 0;
        foreach (var photo in photos)
		{
            try
            {
				// Convert the ImageSource into a Stream
				Stream? stream = null;
				if (photo.Image is FileImageSource fileImageSource)
				{
					stream =  File.OpenRead(fileImageSource.File);
				}
				else if (photo.Image is StreamImageSource streamImageSource)
				{
					stream = await streamImageSource.Stream(CancellationToken.None);
				}
		
                if (stream == null) continue;

				// create file name
				string fileName = $"{Guid.NewGuid()}_{photo.FileName}";

				// put into storage
				byte[] bytes = null;
				using(var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					bytes = memoryStream.ToArray();
				}


                var response = await imagesBucket.Upload(
                    bytes,
                    fileName,
                    new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        Upsert = false,
                        ContentType = "application/octet-stream"
                    });

                // Get the public URL
                string publicUrl = imagesBucket.GetPublicUrl(fileName);
                uploadedUrls[count] = publicUrl;

				Console.WriteLine($"Uploaded {fileName} -> {publicUrl}");
				count++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading {photo.FileName}: {ex.Message}");
            }
        }

        return uploadedUrls;
    }

	public async Task<BasicEntryError> InsertFoodIssue(FoodServiceIssue issue)
	{
		await waitingForInitialization;

		try
		{
			await supabaseClient.From<FoodServiceIssue>().Insert(issue);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
			return BasicEntryError.InsertionError;
		}

		return BasicEntryError.None;
	}

	public async Task<BasicEntryError> InsertEventSupportChange(EventSupportChange change)
	{
		await waitingForInitialization;

		try
		{
			await supabaseClient.From<EventSupportChange>().Insert(change);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
			return BasicEntryError.InsertionError;
		}

		return BasicEntryError.None;
	}

	public async Task UploadManagerLogFileAsync(string localFilePath)
    {
		await waitingForInitialization;

    try
    {
        var bucket = supabaseClient!.Storage.From("manager-logs");
        Debug.WriteLine("1.12");

        string remotePath = $"logs/{Guid.NewGuid()}.docx";
        Debug.WriteLine("1.13");

        await bucket.Upload(
            localFilePath,
            remotePath,
            new Supabase.Storage.FileOptions
            {
                ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Upsert = false
            }
        );

        Debug.WriteLine("1.14");
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"UPLOAD ERROR: {ex.GetType().Name} - {ex.Message}");
        Debug.WriteLine(ex.StackTrace);
        throw;
    }
    }
}