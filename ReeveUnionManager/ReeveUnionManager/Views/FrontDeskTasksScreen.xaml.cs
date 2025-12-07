using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Caleb Wisneski
/// </summary>
public partial class FrontDeskTasksScreen : ContentPage
{
	private readonly ManagerLogObject _log;

	public FrontDeskTasksScreen(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
        FrontDeskTemplate.SubmitClicked += OnSubmitFromTemplate;
    }

	private void OnSubmitFromTemplate(object sender, EventArgs e)
    {
        _log.FrontDeskTasksNotes = FrontDeskTemplate.Notes;
        _log.FrontDeskTasksPictures = FrontDeskTemplate.Photos;

        Navigation.PopAsync();
    }
}
