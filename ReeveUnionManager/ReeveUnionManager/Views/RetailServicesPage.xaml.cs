using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

public partial class RetailServicesPage : ContentPage
{
	private readonly ManagerLogObject _log;

	public RetailServicesPage(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }

	private void HandleSubmit(object sender, EventArgs e)
    {
        _log.RetailServicesNotes = RetailServicesTemplate.Notes;
        _log.RoomSetsPictures = RetailServicesTemplate.Photos;

        Navigation.PopAsync();
    }
}