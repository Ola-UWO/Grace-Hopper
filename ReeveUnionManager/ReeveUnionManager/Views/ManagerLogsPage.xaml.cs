using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Graphics;
using ReeveUnionManager.Services;
using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class ManagerLogsPage : ContentPage
{
    /// <summary>
    /// In-memory collection of OneDrive log files currently displayed.
    /// </summary>
    private readonly ObservableCollection<DriveItem> _oneDriveLogs = new();

    /// <summary>
    /// True if the user is currently signed in to Microsoft via MSAL.
    /// </summary>
    private bool _isSignedIn;

    /// <summary>
    /// True if logs were successfully loaded from the currently selected OneDrive folder.
    /// </summary>
    private bool _hasLoadedLogsSuccessfully;

    /// <summary>
    /// Initializes the Manager Logs page, binding the list and syncing initial auth state.
    /// </summary>
    public ManagerLogsPage()
    {
        InitializeComponent();

        // Navigation ViewModel for "+ New Log Entry"
        BindingContext = new PageViewModel();

        // Bind logs list to OneDrive-backed collection
        LogsList.ItemsSource = _oneDriveLogs;

        // Initialize sign-in state from AuthService
        _isSignedIn = !string.IsNullOrWhiteSpace(AuthService.SignedInUser);
        _hasLoadedLogsSuccessfully = false;

        UpdateAuthUi();
        UpdateEmptyState();
        UpdateStatusBar();
    }

    /// <summary>
    /// Handles full sign-in flow, folder selection, and initial log load.
    /// Returns true if the sign-in flow completed (even if user declined folder selection).
    /// </summary>
    private async Task<bool> SignInAndLoadLogsAsync()
    {
        var result = await AuthService.SignInAsync();

        if (result == null)
        {
            await DisplayAlert(
                "Sign-In Failed",
                "We couldn't sign you in. Please try again.",
                "OK");

            _isSignedIn = false;
            _hasLoadedLogsSuccessfully = false;

            UpdateAuthUi();
            UpdateStatusBar();
            return false;
        }

        _isSignedIn = true;
        _hasLoadedLogsSuccessfully = false;

        UpdateAuthUi();
        UpdateStatusBar();

        await DisplayAlert(
            "Signed In",
            $"You are signed in as:\n{AuthService.SignedInUser}",
            "OK");

        // Prompt for OneDrive folder
        var folderChosen = await EnsureFolderSelectedAsync();
        if (!folderChosen)
        {
            // Signed in but no folder selected yet → banner will show yellow state
            _hasLoadedLogsSuccessfully = false;
            UpdateStatusBar();
            return true;
        }

        _oneDriveLogs.Clear();
        UpdateEmptyState();

        await LoadOneDriveLogsAsync();
        return true;
    }

    /// <summary>
    /// Header button: toggles between sign-in and sign-out behavior.
    /// </summary>
    private async void OnSignInOutHeaderClicked(object sender, EventArgs e)
    {
        if (_isSignedIn)
        {
            await SignOutAsyncCore();
        }
        else
        {
            await SignInAndLoadLogsAsync();
        }
    }

    /// <summary>
    /// Shared sign-out logic used by any sign-out entry point.
    /// Clears auth state, selected folder, logs, and updates the UI/banner.
    /// </summary>
    private async Task SignOutAsyncCore()
    {
        await AuthService.SignOutAsync();

        _isSignedIn = false;
        _hasLoadedLogsSuccessfully = false;

        OneDriveService.SelectedFolderId = null;
        OneDriveService.SelectedFolderName = string.Empty;

        _oneDriveLogs.Clear();
        UpdateEmptyState();
        UpdateAuthUi();
        UpdateStatusBar();
    }

    /// <summary>
    /// Handles taps on the status bar:
    /// - If not signed in: starts sign-in + folder selection + load.
    /// - If signed in: opens folder picker and reloads logs if selection changes.
    /// </summary>
    private async void OnStatusBarTapped(object sender, TappedEventArgs e)
    {
        if (!_isSignedIn)
        {
            await SignInAndLoadLogsAsync();
            return;
        }

        // When signed in, treat banner tap as "change folder"
        var folderChosen = await EnsureFolderSelectedAsync();
        if (!folderChosen)
        {
            // User cancelled folder selection, keep current folder & logs
            return;
        }

        _oneDriveLogs.Clear();
        UpdateEmptyState();
        await LoadOneDriveLogsAsync();
    }

    /// <summary>
    /// Loads .docx logs from the selected OneDrive folder and updates the list + banner state.
    /// </summary>
    private async Task LoadOneDriveLogsAsync()
    {
        var hadError = false;

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
                    "No .docx logs found in the selected folder.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            hadError = true;

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

            _hasLoadedLogsSuccessfully =
                !hadError &&
                _isSignedIn &&
                !string.IsNullOrEmpty(OneDriveService.SelectedFolderId);

            UpdateStatusBar();
        }
    }

    /// <summary>
    /// Opens the folder picker modal and stores the selected OneDrive folder (ID + name).
    /// Returns false if the user cancels or an invalid folder is returned.
    /// </summary>
    private async Task<bool> EnsureFolderSelectedAsync()
    {
        var pickerPage = new OneDriveFolderPage();

        await Navigation.PushModalAsync(pickerPage);
        var selectedFolder = await pickerPage.CompletionSource.Task;

        if (selectedFolder == null || string.IsNullOrEmpty(selectedFolder.Id))
        {
            return false;
        }

        OneDriveService.SelectedFolderName = selectedFolder.Name ?? string.Empty;
        OneDriveService.SelectedFolderId = selectedFolder.Id;

        _hasLoadedLogsSuccessfully = false;
        UpdateStatusBar();

        return true;
    }

    /// <summary>
    /// Opens a selected OneDrive log file using the platform launcher (Word, Pages, etc.).
    /// </summary>
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

            var localPath = await OneDriveService.DownloadFileToLocalAsync(selected);

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
            collectionView.SelectedItem = null;

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    /// <summary>
    /// Updates auth-related UI elements (currently header sign-in/out button text).
    /// Status bar content/colors are handled by UpdateStatusBar().
    /// </summary>
    private void UpdateAuthUi()
    {
        SignInOutHeaderButton.Text = _isSignedIn ? "Sign Out" : "Sign In";
    }

    /// <summary>
    /// Updates the status bar's colors, icon, and text based on auth state, folder selection, and load result.
    /// </summary>
    private void UpdateStatusBar()
    {
        // 1) Not signed in → always red, regardless of folder/log state
        if (!_isSignedIn)
        {
            StatusBarContainer.BackgroundColor = Color.FromArgb("#FDECEA");  // light red
            StatusBarContainer.Stroke = new SolidColorBrush(Color.FromArgb("#F5C2C7"));

            StatusBarIcon.Text = "⚠️";
            StatusBarTitle.Text = "Not connected to OneDrive";
            StatusBarSubtitle.Text = "Tap here to sign in and view manager logs.";
            return;
        }

        // 2) Signed in – check for folder selection
        var hasFolderSelected = !string.IsNullOrEmpty(OneDriveService.SelectedFolderId);

        if (!hasFolderSelected)
        {
            StatusBarContainer.BackgroundColor = Color.FromArgb("#FFF4E5");   // light yellow
            StatusBarContainer.Stroke = new SolidColorBrush(Color.FromArgb("#FFECB5"));

            StatusBarIcon.Text = "⚠️";
            StatusBarTitle.Text = "No folder selected";
            StatusBarSubtitle.Text = "Tap here to choose a OneDrive folder.";
            return;
        }

        // 3) Signed in + folder selected
        var userEmail = AuthService.SignedInUser ?? "Unknown account";

        if (_hasLoadedLogsSuccessfully)
        {
            // Green: connected and logs loaded successfully
            StatusBarContainer.BackgroundColor = Color.FromArgb("#E8F5E9");   // light green
            StatusBarContainer.Stroke = new SolidColorBrush(Color.FromArgb("#A5D6A7"));

            StatusBarIcon.Text = "✅";
            StatusBarTitle.Text = "Connected to OneDrive";
            StatusBarSubtitle.Text =
                $"Signed in as: {userEmail}\n" +
                $"Folder: {OneDriveService.SelectedFolderName}";
        }
        else
        {
            // Yellow: signed in + folder selected but logs not yet loaded (or last load failed)
            StatusBarContainer.BackgroundColor = Color.FromArgb("#FFF4E5");
            StatusBarContainer.Stroke = new SolidColorBrush(Color.FromArgb("#FFECB5"));

            StatusBarIcon.Text = "ℹ️";
            StatusBarTitle.Text = "Ready to load logs";
            StatusBarSubtitle.Text = "Tap here to load manager logs from OneDrive.";
        }
    }

    /// <summary>
    /// Shows or hides the "no logs" empty state label based on the log collection.
    /// </summary>
    private void UpdateEmptyState()
    {
        EmptyStateLabel.IsVisible = _oneDriveLogs.Count == 0;
    }

    /// <summary>
    /// On page appearance, syncs auth state and ensures logs are loaded appropriately.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await InitializeAsync();
    }

    /// <summary>
    /// Central initialization: refreshes sign-in state and loads logs (with user prompt if needed).
    /// </summary>
    private async Task InitializeAsync()
    {
        // Refresh sign-in state in case it changed while the app was running
        _isSignedIn = !string.IsNullOrWhiteSpace(AuthService.SignedInUser);

        UpdateAuthUi();
        UpdateEmptyState();
        UpdateStatusBar();

        if (!_isSignedIn)
        {
            bool signInNow = await DisplayAlert(
                "Microsoft Sign-In Required",
                "To view manager logs, you need to connect a Microsoft account. Would you like to sign in now?",
                "Sign In",
                "Not Now");

            if (!signInNow)
            {
                return;
            }

            var success = await SignInAndLoadLogsAsync();
            if (!success)
            {
                return;
            }

            return;
        }

        // Already signed in: auto-load logs if none are present yet
        if (_oneDriveLogs.Count == 0)
        {
            await LoadOneDriveLogsAsync();
        }
    }
}
