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

	public void OnViewEventLogPage(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync(nameof(EventCheckInLogs));
	}
}