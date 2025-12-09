using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Caleb Wisneski
/// </summary>
public partial class RoomSetsScreen : ContentPage
{
    private readonly ManagerLogObject _log;

	public RoomSetsScreen(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
        RoomSetsTemplate.SubmitClicked += OnSubmitFromTemplate;
    }

	private void OnSubmitFromTemplate(object sender, EventArgs e)
    {
        _log.RoomSetsNotes = RoomSetsTemplate.Notes;
        _log.RoomSetsPictures = RoomSetsTemplate.Photos;
    }
}