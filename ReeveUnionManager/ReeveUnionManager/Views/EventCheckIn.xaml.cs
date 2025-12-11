using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Kyle Johnson
/// </summary>
/// 
[QueryProperty(nameof(EventTitle), "EventTitle")]
[QueryProperty(nameof(EventDateAndTime), "EventDateAndTime")]
[QueryProperty(nameof(EventLocation), "EventLocation")]
[QueryProperty(nameof(EventNotes), "EventNotes")]
public partial class EventCheckIn : ContentPage
{
    private string _eventTitle;
    private string _eventDateAndTime;
    private string _eventLocation;
    private string _eventNotes;

	public string EventTitle
    {
        set
        {
            _eventTitle = value;
            if (CheckInNameEntry != null)
                CheckInNameEntry.Text = value;
        }
    }

    public string EventDateAndTime
    {
        set
        {
            _eventDateAndTime = value;
            if (TimeOfCheckInEntry != null)
                TimeOfCheckInEntry.Text = value;
        }
    }

    public string EventLocation
    {
        set
        {
            _eventLocation = value;
            if (CheckInLocationEntry != null)
                CheckInLocationEntry.Text = value;
        }
    }

	public string EventNotes
    {
        set
        {
            _eventNotes = value;
            if (EventNotesEntry != null)
                EventNotesEntry.Text = value;
        }
    }
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
        bool isChecked = CheckInBox.IsChecked;
        string checkInNotes = EventNotesEntry.Text;
		
		var se = new ScrapeEvent
            {
                EventId = checkInId,
                EventTitle = checkInName,
                EventLocation = checkInLocation,
                EventDateAndTime = timeOfCheckIn,
                EventCheckIn = isChecked,
                EventNotes = checkInNotes,
            };
		// FIX: this should be editing not trying to insert (no way to edit currently)
        ScrapeEventError error = await MauiProgram.database.InsertEvent(se);
        if (error != ScrapeEventError.None)
        {
            await DisplayAlert("Addition has failed", error.ToString(), "OK");
        }
        else
        {
            await DisplayAlert("Success", "Event check-in updated successfully", "OK");
            await Shell.Current.GoToAsync("..");
        }
	}
	public void OnViewEventLogPage(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync(nameof(EventCheckInLogs));
	}
}