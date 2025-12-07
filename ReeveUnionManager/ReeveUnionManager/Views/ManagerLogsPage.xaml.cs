using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using ReeveUnionManager.Services;
using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class ManagerLogsPage : ContentPage
{
    private readonly ObservableCollection<DriveItem> _oneDriveLogs = new();
    private bool _isSignedIn;

    public ManagerLogsPage()
    {
        InitializeComponent();

        // Navigation ViewModel for "+ New Log Entry"
        BindingContext = new PageViewModel();

        // Bind logs list to OneDrive-backed collection
        LogsList.ItemsSource = _oneDriveLogs;

        // Initialize sign-in state from AuthService if it tracks the user
        _isSignedIn = !string.IsNullOrWhiteSpace(AuthService.SignedInUser);
        UpdateAuthUi();
        UpdateEmptyState();
    }

    private async void OnSignInWithMicrosoftClicked(object sender, EventArgs e)
    {
        var result = await AuthService.SignInAsync();

        if (result != null)
        {
            _isSignedIn = true;
            UpdateAuthUi();

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

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        await AuthService.SignOutAsync();

        _isSignedIn = false;

        _oneDriveLogs.Clear();
        UpdateEmptyState();
        UpdateAuthUi();

        await DisplayAlert(
            "Signed Out",
            "You have been signed out of Microsoft.",
            "OK");
    }


    private async void OnLoadLogsClicked(object sender, EventArgs e)
    {
        // Prompt for folder name
        string folderName = await DisplayPromptAsync(
            "Choose OneDrive Folder",
            "Enter the folder name where your log files are stored:",
            accept: "Load",
            cancel: "Cancel",
            initialValue: OneDriveService.SelectedFolderName);

        if (string.IsNullOrWhiteSpace(folderName))
            return;

        // Save selection
        OneDriveService.SelectedFolderName = folderName.Trim();

        // Load files
        await LoadOneDriveLogsAsync();
    }


    private async Task LoadOneDriveLogsAsync()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LogsList.IsEnabled = false;

            var files = await OneDriveService.GetManagerLogFilesAsync();

            var sorted = files
                .OrderByDescending(f => f.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _oneDriveLogs.Clear();

            foreach (var file in sorted)
            {
                _oneDriveLogs.Add(file);
            }

            UpdateEmptyState();

            if (_oneDriveLogs.Count == 0)
            {
                await DisplayAlert(
                    "OneDrive Logs",
                    "No .docx logs found in the Manager Logs (TEST) folder.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "OneDrive Error",
                ex.Message,
                "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            LogsList.IsEnabled = true;
        }
    }

    private async void OnLogSelected(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (CollectionView)sender;
        DriveItem? selected = null;

        try
        {
            selected = e.CurrentSelection?.FirstOrDefault() as DriveItem;
            if (selected == null)
                return;

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // Download the file locally
            var localPath = await OneDriveService.DownloadFileToLocalAsync(selected);

            // Open with platform launcher (Word, Pages, Files App, etc)
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(localPath),
                Title = selected.Name ?? "Manager Log"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Open Log Error",
                $"Unable to open this log.\n\nDetails: {ex.Message}",
                "OK");
        }
        finally
        {
            // ALWAYS clear the highlight no matter what happens
            collectionView.SelectedItem = null;

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }


    private void UpdateAuthUi()
    {
        if (_isSignedIn)
        {
            SignInButton.IsVisible = false;
            SignOutButton.IsVisible = true;
            LoadLogsButton.IsVisible = true;

        }
        else
        {
            SignInButton.IsVisible = true;
            SignOutButton.IsVisible = false;
            LoadLogsButton.IsVisible = false;

        }
    }

    private void UpdateEmptyState()
    {
        EmptyStateLabel.IsVisible = _oneDriveLogs.Count == 0;
    }
}
