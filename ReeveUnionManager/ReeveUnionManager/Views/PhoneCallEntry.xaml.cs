namespace ReeveUnionManager.Views;

using ReeveUnionManager.Models;

/// <summary>
/// Author: Kyle Johnson
/// </summary>
public partial class PhoneCallEntry : ContentPage
{
    public PhoneCallEntry()
    {
        InitializeComponent();
    }

    public async void HandleSubmit(object sender, EventArgs args)
    {
        Guid callId = Guid.NewGuid();
        string timeOfCall = TimeOfCallEntry.Text;
		string callerName = CallerEntry.Text;
        string callNotes = PhoneCallNotesEntry.Text;
        
        CallLogError error = await MauiProgram.businessLogic.AddCallLog(callId, callerName, timeOfCall, callNotes);
        if (error != CallLogError.None)
		{
			await DisplayAlert("Addition has failed", error.ToString(), "OK");
		}
    }

    public void OnViewPhoneLogClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(PhoneCallLogs));
    }
}