namespace ReeveUnionManager.Views;

using ReeveUnionManager.Models;

/// <summary>
/// Author: Kyle Johnson
/// </summary>
public partial class PhoneCallEntry : ContentPage
{
    int callId = 0;
    public PhoneCallEntry()
    {
        InitializeComponent();
        BindingContext = MauiProgram.businessLogic;
    }

    public async void HandleSubmit(object sender, EventArgs args)
    {
        callId += 1;    // FIXME: IMPLEMENT AUTO INCREMENT LATER IN BUSINESSLOGIC
        // String timeOfCall = DateTime.Now.ToString();    // ADD THIS IN SPRINT 3
        string timeOfCall = TimeOfCallEntry.Text;
		string callerName = CallerEntry.Text;
        string callNotes = PhoneCallNotesEntry.Text;
        
        CallLogError error = await MauiProgram.businessLogic.AddCallLog(callId, callerName, timeOfCall, callNotes);
        if (error != CallLogError.None)
		{
			await DisplayAlert("Addition has failed", error.ToString(), "OK");
		}
    }
}