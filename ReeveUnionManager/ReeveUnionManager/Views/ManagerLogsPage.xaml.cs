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
using Microsoft.Identity.Client;

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
        _isSignedIn = AuthService.IsSignedIn;
        _hasLoadedLogsSuccessfully = false;

        UpdateAuthUi();
        UpdateEmptyState();
        UpdateStatusBar();
    }

    /// <summary>
    /// Handles the sign-in flow and updates auth UI, but does not select a folder or load logs.
    /// </summary>
    private async Task<bool> SignInAsync()
    {
        AuthenticationResult? result = null;

        try
        {
            result = await AuthService.SignInAsync();
        }
        catch (MsalException ex)
        {
            await DisplayAlert(
                "Microsoft Sign-In Error",
                $"We couldn't complete sign-in.\n\nDetails:\n{ex.ErrorCode}: {ex.Message}",
                "OK");

            _isSignedIn = false;
            _hasLoadedLogsSuccessfully = false;
            UpdateAuthUi();
            UpdateStatusBar();
            return false;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Microsoft Sign-In Error",
                $"An unexpected error occurred while signing in.\n\n{ex.Message}",
                "OK");

            _isSignedIn = false;
            _hasLoadedLogsSuccessfully = false;
            UpdateAuthUi();
            UpdateStatusBar();
            return false;
        }

        if (result == null || string.IsNullOrWhiteSpace(AuthService.AccessToken))
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

        // Success
        _isSignedIn = true;
        _hasLoadedLogsSuccessfully = false;

        UpdateAuthUi();
        UpdateStatusBar();

        await DisplayAlert(
            "Signed In",
            $"You are signed in as:\n{AuthService.SignedInUser ?? "<unknown>"}",
            "OK");

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
            await SignInAsync();
        }
    }

    private async void OnUploadLogFromDeviceClicked(object sender, EventArgs e)
    {
        await UploadLogFromDeviceAsync();
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
    /// - If not signed in: starts the sign-in flow.
    /// - If signed in: opens folder picker and reloads logs if selection changes.
    /// </summary>
    private async void OnStatusBarTapped(object sender, TappedEventArgs e)
    {
        if (!_isSignedIn)
        {
            var signedIn = await SignInAsync();
            // If sign-in fails, do nothing else.
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
    /// Lets the user pick a file from the device and uploads it
    /// into the currently selected OneDrive manager logs folder.
    /// Ensures sign-in and folder selection before uploading.
    /// </summary>
    private async Task UploadLogFromDeviceAsync()
    {
        // 1) Ensure the user is signed in
        if (!_isSignedIn)
        {
            var signedIn = await SignInAsync();
            if (!signedIn)
            {
                // Sign-in failed or was cancelled
                return;
            }
        }

        // 2) Ensure a OneDrive folder is selected
        if (string.IsNullOrEmpty(OneDriveService.SelectedFolderId))
        {
            var folderChosen = await EnsureFolderSelectedAsync();
            if (!folderChosen)
            {
                // User cancelled folder selection
                return;
            }
        }

        try
        {
            // 3) Let the user pick a file on the device
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a log file to upload"
                // You can add FileTypes later to restrict to .docx if you want
            });

            if (result == null)
            {
                // User cancelled the picker
                return;
            }

            var localPath = result.FullPath;
            if (string.IsNullOrEmpty(localPath))
                throw new Exception("Unable to access the selected file on this device.");

            // 4) Show loading indicator during upload
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var uploadedItem = await OneDriveService.UploadFileToSelectedFolderAsync(localPath);

            var uploadedName = uploadedItem?.Name ?? System.IO.Path.GetFileName(localPath);

            await DisplayAlert(
                "Upload complete",
                $"Uploaded '{uploadedName}' to your OneDrive folder:\n{OneDriveService.SelectedFolderName}",
                "OK");

            // 5) Optionally reload logs so the new file appears in the list
            await LoadOneDriveLogsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Upload failed",
                $"We couldn't upload that file.\n\nDetails:\n{ex.Message}",
                "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
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

    private async void OnLogSelected(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (CollectionView)sender;
        DriveItem? selected = null;

        try
        {
            selected = e.CurrentSelection?.FirstOrDefault() as DriveItem;
            if (selected == null)
                return;

            // Download then preview via iOS QuickLook
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var localPath = await OneDriveService.DownloadFileToLocalAsync(selected);

            // Native iOS QuickLook preview (we'll implement this next)
            await FilePreviewService.PreviewAsync(localPath, selected.Name);
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


    private async Task OpenDriveItemOnDeviceAsync(DriveItem item)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var localPath = await OneDriveService.DownloadFileToLocalAsync(item);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(localPath),
                Title = item.Name ?? "Manager Log"
            });
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async Task DownloadDriveItemToDeviceAsync(DriveItem item)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var localPath = await OneDriveService.DownloadFileToLocalAsync(item);

            await DisplayAlert(
                "Download complete",
                $"The file was downloaded to:\n{localPath}",
                "OK");
        }
        finally
        {
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
        // Upload button is only shown when signed in AND a folder is selected
        var hasFolderSelectedForUpload =
            _isSignedIn && !string.IsNullOrEmpty(OneDriveService.SelectedFolderId);
        UploadButton.IsVisible = hasFolderSelectedForUpload;

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
    /// Central initialization: refreshes sign-in state and loads logs (with user prompt if already signed in).
    /// </summary>
    private async Task InitializeAsync()
    {
        // Refresh sign-in state in case it changed while the app was running
        _isSignedIn = AuthService.IsSignedIn;

        UpdateAuthUi();
        UpdateEmptyState();
        UpdateStatusBar();

        // If already signed in, auto-load logs if none are present yet.
        // If not signed in, do NOTHING here – user uses the header button or banner.
        if (_isSignedIn && _oneDriveLogs.Count == 0)
        {
            await LoadOneDriveLogsAsync();
        }
    }

    private async void OnLogContextPreviewClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.BindingContext is DriveItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.WebUrl))
            {
                await Launcher.OpenAsync(item.WebUrl);
            }
            else
            {
                await DisplayAlert(
                    "Preview unavailable",
                    "An online preview link is not available for this file. Try opening it on the device instead.",
                    "OK");
            }
        }
    }

    private async void OnLogContextOpenClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.BindingContext is DriveItem item)
        {
            await OpenDriveItemOnDeviceAsync(item);
        }
    }

    private async void OnLogContextDownloadClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.BindingContext is DriveItem item)
        {
            await DownloadDriveItemToDeviceAsync(item);
        }
    }

}
