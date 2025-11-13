using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Kyle Johnson
/// </summary>
public partial class EventCheckIn : ContentPage
{
	public EventCheckIn()
	{
		InitializeComponent();
	}

	public async void HandleSubmit(object sender, EventArgs args)
    {
		Guid checkInId = Guid.NewGuid();
		string checkInName = CheckInNameEntry.Text;
		string timeOfCheckIn = TimeOfCheckInEntry.Text;
		string checkInLocation = CheckInLocationEntry.Text;
        string checkInNotes = EventNotesEntry.Text;
        
        CheckInLogError error = await MauiProgram.businessLogic.AddCheckInLog(checkInId, checkInName, timeOfCheckIn, checkInLocation, checkInNotes);
        if (error != CheckInLogError.None)
		{
			await DisplayAlert("Addition has failed", error.ToString(), "OK");
		}
    }
	public void OnViewEventLogPage(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync(nameof(EventCheckInLogs));
	}
}