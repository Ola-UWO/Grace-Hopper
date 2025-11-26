using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

public partial class TechnologyPage : ContentPage
{
	private readonly ManagerLogObject _log;
	public TechnologyPage(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }

	private void HandleSubmit(object sender, EventArgs e)
    {
        _log.AvTechnologyNotes = AvTechnologyTemplate.Notes;
        _log.AvTechnologyPictures = AvTechnologyTemplate.Photos;

        Navigation.PopAsync();
    }
}