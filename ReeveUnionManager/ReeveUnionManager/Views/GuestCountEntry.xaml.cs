using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class GuestCountEntry : ContentPage
{
    private readonly ManagerLogObject _log;

    public GuestCountEntry(ManagerLogObject log)
    {
        _log = log;
        InitializeComponent();
    }
    
    private void OnSubmitClicked(object sender, EventArgs e)
    {
        _log.NumberOfGuestsDate = GuestCountDate.Date.ToString("MM-dd-yyy");
        _log.NumberOfGuestsHourBeforeClosing = GuestCountHourBeforeClosing.Text;
        _log.NumberofGuestsAtClosing = GuestCountAtClosing.Text;
        _log.NumberOfGuestsNotes = GuestCountClosingNotes.Text;

        Navigation.PopAsync();
    }
}
