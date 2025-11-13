using ReeveUnionManager.Models;
using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class ManagerLogsPage : ContentPage
{
    private readonly Database _database = new();

    public ManagerLogsPage()
    {
        InitializeComponent();
        BindingContext = new PageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Get the most recent manager logs from Supabase
        var logs = await _database.SelectRecentManagerLogsAsync();

        // Bind them to the CollectionView
        LogsList.ItemsSource = logs;
    }
}
