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
    }

	private void HandleSubmit(object sender, EventArgs e)
    {
        _log.RoomSetsNotes = RoomSetsTemplate.Notes;
        _log.RoomSetsPictures = RoomSetsTemplate.Photos;

        Navigation.PopAsync();
    }
}