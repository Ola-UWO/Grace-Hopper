using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class EventSupportChanges : ContentPage
{

    private readonly ManagerLogObject _log;

    public EventSupportChanges(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        _log.EventSupportChangesName = EventName.Text;
        _log.EventSupportChangesTime = EventTime.Text;
        _log.EventSupportChangesLocation = EventLocation.Text;
        _log.EventSupportChangesDetails = EventChangeDetails.Text;
        // _log.EventSupportChangesPictures = ShiftEnd.Time.ToString(@"hh\:mm");           FIXME: FIGURE OUT PICTURES LATER
        
        Navigation.PopAsync();
    }
}
