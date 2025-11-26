using System.Diagnostics;
using ReeveUnionManager.Models;

namespace ReeveUnionManager;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class AppShell : Shell
{
    private bool _hasScraped = false;
    
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.PhoneCallEntry), typeof(Views.PhoneCallEntry));
        Routing.RegisterRoute(nameof(Views.PhoneCallLogs), typeof(Views.PhoneCallLogs));
        Routing.RegisterRoute(nameof(Views.EventSupportChanges), typeof(Views.EventSupportChanges));
        Routing.RegisterRoute(nameof(Views.EventCheckIn), typeof(Views.EventCheckIn));
        Routing.RegisterRoute(nameof(Views.EventCheckInLogs), typeof(Views.EventCheckInLogs));
        Routing.RegisterRoute(nameof(Views.RoomSetsScreen), typeof(Views.RoomSetsScreen));
        Routing.RegisterRoute(nameof(Views.MaintenanceCustodialScreen), typeof(Views.MaintenanceCustodialScreen));
        Routing.RegisterRoute(nameof(Views.ClosingCheckListPage), typeof(Views.ClosingCheckListPage));
        Routing.RegisterRoute(nameof(Views.OpeningCheckListPage), typeof(Views.OpeningCheckListPage));
        Routing.RegisterRoute(nameof(Views.GenericNoteEntry), typeof(Views.GenericNoteEntry));
        Routing.RegisterRoute(nameof(Views.ManagerLogsPage), typeof(Views.ManagerLogsPage));
        Routing.RegisterRoute(nameof(Views.NewLogEntryPage), typeof(Views.NewLogEntryPage));
        Routing.RegisterRoute(nameof(Views.FoodServiceIssue), typeof(Views.FoodServiceIssue));
        Routing.RegisterRoute(nameof(Views.GuestCountEntry), typeof(Views.GuestCountEntry));
        Routing.RegisterRoute(nameof(Views.FrontDeskTasksScreen), typeof(Views.FrontDeskTasksScreen));
        Routing.RegisterRoute(nameof(Views.ShiftDetails), typeof(Views.ShiftDetails));
        Routing.RegisterRoute(nameof(Views.TechnologyPage), typeof(Views.TechnologyPage));
        Routing.RegisterRoute(nameof(Views.RetailServicesPage), typeof(Views.RetailServicesPage));
        Routing.RegisterRoute(nameof(Views.MiscellaneousPage), typeof(Views.MiscellaneousPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasScraped) return;
        _hasScraped = true;

        Debug.WriteLine("AppShell appearing — starting scrape…");
        try
        {
            await MauiProgram.businessLogic.Scrape25Live();
            Debug.WriteLine("Scrape succeeded");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Scrape failed: {ex.Message}");
        }
    }
}
