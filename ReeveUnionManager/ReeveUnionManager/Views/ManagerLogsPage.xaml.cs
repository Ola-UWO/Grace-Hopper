using ReeveUnionManager.Models;
using ReeveUnionManager.ViewModels;
using ReeveUnionManager.Services;

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

    private async void OnSignInWithMicrosoftClicked(object sender, EventArgs e)
    {
        var result = await AuthService.SignInAsync();

        if (result != null)
        {
            await DisplayAlert(
                "Signed In",
                $"You are signed in as:\n{AuthService.SignedInUser}",
                "OK");
        }
        else
        {
            await DisplayAlert(
                "Sign-In Failed",
                "Unable to sign in. Please try again.",
                "OK");
        }
    }
}
