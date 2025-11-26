using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

public partial class MiscellaneousPage : ContentPage
{
	private readonly ManagerLogObject _log;
	
	public MiscellaneousPage(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }

	private void HandleSubmit(object sender, EventArgs e)
    {
        _log.MiscNotes = MiscTemplate.Notes;
        _log.MiscPictures = MiscTemplate.Photos;

        Navigation.PopAsync();
    }
}