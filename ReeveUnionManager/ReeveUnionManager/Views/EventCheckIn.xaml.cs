using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Kyle Johnson
/// </summary>
public partial class EventCheckIn : ContentPage
{
	int checkInId = 0;

	public EventCheckIn()
	{
		InitializeComponent();
	}

	public async void HandleSubmit(object sender, EventArgs args)
    {
        checkInId += 1;    // FIXME: IMPLEMENT AUTO INCREMENT LATER IN BUSINESSLOGIC
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
}