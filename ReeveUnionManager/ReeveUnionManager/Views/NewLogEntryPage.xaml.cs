using System.Diagnostics;
using ReeveUnionManager.Models;
using ReeveUnionManager.ViewModels;
using ReeveUnionManager.Services;

namespace ReeveUnionManager.Views;

public partial class NewLogEntryPage : ContentPage
{
	private readonly ManagerLogObject _log;

	public NewLogEntryPage(ManagerLogObject log)
	{
		InitializeComponent();
		BindingContext = new PageViewModel();
		_log = log;
	}

	private async void OnSubmitClicked(object sender, EventArgs e)
	{
		Debug.WriteLine("Before Log Build");
		
		var localPath = await MauiProgram.businessLogic.CreateManagerLogFileAsync(_log);
		Debug.WriteLine("After Log Build");
		
		/*
		if (result == ManagerLogError.None)
		{
			await DisplayAlert("Success", "Manager log created!", "OK");
			await MauiProgram.businessLogic.DeleteAllCallLogs(); // clear the phone log after creating the manager log
		}
		else
		*/
		// 1) Create the DOCX via business logic
		if (string.IsNullOrEmpty(localPath))
		{

			await DisplayAlert("Error", "Could not create log file.", "OK");
			return;
		}

		// 2) Ask the user if they want to save to OneDrive
		var confirm = await DisplayAlert(
			"Save to OneDrive?",
			"This will save a copy of this manager log to your selected OneDrive folder.",
			"Save",
			"Cancel");

		if (!confirm)
		{
			// Log file was created locally; user just chose not to upload
			await DisplayAlert("Success", "Manager log created locally.", "OK");
			await Navigation.PopAsync();
			return;
		}

		// 3) Ensure the user is signed in and has selected a folder
		if (!AuthService.IsSignedIn)
		{
			await DisplayAlert(
				"Microsoft sign-in required",
				"Please sign in to OneDrive on the Manager Logs page before saving.",
				"OK");
			return;
		}

		if (string.IsNullOrEmpty(OneDriveService.SelectedFolderId))
		{
			await DisplayAlert(
				"No OneDrive folder selected",
				"Please choose a OneDrive folder on the Manager Logs page before saving.",
				"OK");
			return;
		}

		try
		{
			// 4) Upload the DOCX to the selected OneDrive folder
			await OneDriveService.UploadFileToSelectedFolderAsync(localPath);

			await DisplayAlert("Success", "Manager log created and saved to OneDrive!", "OK");

			ManagerLogsPage.ShouldRefreshOnAppear = true;

			await Navigation.PopAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlert(
				"Upload failed",
				$"We created the log file, but couldn't upload it to OneDrive.\n\nDetails:\n{ex.Message}",
				"OK");
		}
		
		await MauiProgram.businessLogic.DeleteAllCallLogs(); // clear the phone log after creating the manager log
	}
	


}