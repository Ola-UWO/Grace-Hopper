using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Kyle Johnson
/// </summary>
public partial class EventCheckInLogs : ContentPage
{

	public EventCheckInLogs()
	{
		InitializeComponent();
		BindingContext = MauiProgram.businessLogic;
	}

	private async void OnEventTapped(object sender, EventArgs e)
	{
		var tapGesture = (TapGestureRecognizer)((Border)sender).GestureRecognizers[0];
        var scrapeEvent = (ScrapeEvent)tapGesture.CommandParameter;
        
        // Navigate to EventCheckIn page and pass the event data
        var navigationParameter = new Dictionary<string, object>
        {
            { "EventTitle", scrapeEvent.EventTitle },
            { "EventDateAndTime", scrapeEvent.EventDateAndTime },
            { "EventLocation", scrapeEvent.EventLocation },
            { "EventNotes", scrapeEvent.EventNotes ?? "" }
        };
        
        await Shell.Current.GoToAsync(nameof(NewLogEntryPage), navigationParameter);
	}
}