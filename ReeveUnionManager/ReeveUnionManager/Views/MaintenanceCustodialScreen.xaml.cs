using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Caleb Wisneski
/// </summary>
public partial class MaintenanceCustodialScreen : ContentPage
{
	private readonly ManagerLogObject _log;

	public MaintenanceCustodialScreen(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
        MaintenanceCustodialTemplate.SubmitClicked += OnSubmitFromTemplate;
    }

	private void OnSubmitFromTemplate(object sender, EventArgs e)
    {
        _log.CustiodialNotes = MaintenanceCustodialTemplate.Notes;
        _log.CustiodialPictures = MaintenanceCustodialTemplate.Photos;

        Navigation.PopAsync();
    }
}