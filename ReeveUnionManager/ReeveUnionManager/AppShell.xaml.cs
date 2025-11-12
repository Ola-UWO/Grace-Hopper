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
        Routing.RegisterRoute(nameof(Views.ManagerLogsPage), typeof(Views.ManagerLogsPage));
        Routing.RegisterRoute(nameof(Views.NewLogEntryPage), typeof(Views.NewLogEntryPage));
        Routing.RegisterRoute(nameof(Views.EventCheckIn), typeof(Views.EventCheckIn));
        Routing.RegisterRoute(nameof(Views.FoodServiceIssue), typeof(Views.FoodServiceIssue));
        Routing.RegisterRoute(nameof(Views.GuestCountEntry), typeof(Views.GuestCountEntry));
        Routing.RegisterRoute(nameof(Views.FrontDeskTasksScreen), typeof(Views.FrontDeskTasksScreen));
        Routing.RegisterRoute(nameof(Views.ShiftDetails), typeof(Views.ShiftDetails));
        Routing.RegisterRoute(nameof(Views.TechnologyPage), typeof(Views.TechnologyPage));
        Routing.RegisterRoute(nameof(Views.RetailServicesPage), typeof(Views.RetailServicesPage));
        Routing.RegisterRoute(nameof(Views.MiscellaneousPage), typeof(Views.MiscellaneousPage));

        Loaded += async (_, _) => await OnLoaded();
    }

    private async Task OnLoaded()
    {
        try
        {
            await MauiProgram.businessLogic.Scrape25Live();
            Console.WriteLine("Scrape succeeded");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scrape failed: {ex.Message}");
        }
    }
}
