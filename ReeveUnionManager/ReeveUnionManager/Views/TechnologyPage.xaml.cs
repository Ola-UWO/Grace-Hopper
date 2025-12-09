using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

public partial class TechnologyPage : ContentPage
{
	private readonly ManagerLogObject _log;
	public TechnologyPage(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
        AvTechnologyTemplate.SubmitClicked += OnSubmitFromTemplate;
    }

	private void OnSubmitFromTemplate(object sender, EventArgs e)
    {
        _log.AvTechnologyNotes = AvTechnologyTemplate.Notes;
        _log.AvTechnologyPictures = AvTechnologyTemplate.Photos;
    }
}