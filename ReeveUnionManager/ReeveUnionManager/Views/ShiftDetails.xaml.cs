using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

public partial class ShiftDetails : ContentPage
{
    private readonly ManagerLogObject _log;

    public ShiftDetails(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }
    
    private void OnSubmitClicked(object sender, EventArgs e)
    {
        _log.ShiftDetailsName = NameENT.Text;
        _log.ShiftDetailsDate = ShiftDate.Date.ToString("MM-dd-yyy");
        _log.ShiftDetailsDayOfWeek = DayOfWeekENT.Text;
        _log.ShiftStartTime = ShiftStart.Time.ToString(@"hh\:mm");
        _log.ShiftEndTime = ShiftEnd.Time.ToString(@"hh\:mm");

        Navigation.PopAsync();
    }
}