namespace ReeveUnionManager;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.PhoneCallEntry), typeof(Views.PhoneCallEntry));
        Routing.RegisterRoute(nameof(Views.PhoneCallLogs), typeof(Views.PhoneCallLogs));
        Routing.RegisterRoute(nameof(Views.EventSupportChanges), typeof(Views.EventSupportChanges));
        Routing.RegisterRoute(nameof(Views.EventCheckInLogs), typeof(Views.EventCheckInLogs));
        Routing.RegisterRoute(nameof(Views.RoomSetsScreen), typeof(Views.RoomSetsScreen));
        Routing.RegisterRoute(nameof(Views.MaintenanceCustodialScreen), typeof(Views.MaintenanceCustodialScreen));
        Routing.RegisterRoute(nameof(Views.ClosingCheckListPage), typeof(Views.ClosingCheckListPage));
        Routing.RegisterRoute(nameof(Views.OpeningCheckListPage), typeof(Views.OpeningCheckListPage));
        Routing.RegisterRoute(nameof(Views.GenericNoteEntry), typeof(Views.GenericNoteEntry));
    }
}
