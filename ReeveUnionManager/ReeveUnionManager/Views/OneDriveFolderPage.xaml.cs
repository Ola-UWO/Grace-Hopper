using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Maui.Controls;
using ReeveUnionManager.Services;

namespace ReeveUnionManager.Views
{
    /// <summary>
    /// Modal page that lets the user browse their OneDrive folder hierarchy
    /// and pick a single folder. Result is returned via <see cref="CompletionSource"/>.
    /// </summary>
    public partial class OneDriveFolderPage : ContentPage
    {
        /// <summary>
        /// Folders currently displayed in the list.
        /// </summary>
        private readonly ObservableCollection<DriveItem> _folders = new();

        /// <summary>
        /// Navigation history for the "Up" button (root → child → grandchild, etc.).
        /// </summary>
        private readonly Stack<DriveItem> _navigationStack = new();

        /// <summary>
        /// Currently viewed folder; null represents the OneDrive root.
        /// </summary>
        private DriveItem? _currentFolder;

        /// <summary>
        /// Completion source used by the caller to await the selected folder.
        /// Returns a <see cref="DriveItem"/> for the folder, or null if canceled.
        /// </summary>
        public TaskCompletionSource<DriveItem?> CompletionSource { get; } = new();

        /// <summary>
        /// Creates the folder picker and begins loading the OneDrive root folder.
        /// </summary>
        public OneDriveFolderPage()
        {
            InitializeComponent();

            FolderList.ItemsSource = _folders;

            // Start at OneDrive root
            _ = LoadFoldersAsync("root", null);
        }

        /// <summary>
        /// Loads child folders for the given folder ID and updates the list, header,
        /// loading overlay, and "no subfolders" message.
        /// </summary>
        /// <param name="folderId">The OneDrive item ID of the folder to load ("root" for drive root).</param>
        /// <param name="folderItem">The DriveItem for the folder, or null when at root.</param>
        private async Task LoadFoldersAsync(string folderId, DriveItem? folderItem)
        {
            try
            {
                NoSubfoldersLabel.IsVisible = false;

                FolderListOverlay.IsVisible = true;
                FolderLoadingIndicator.IsRunning = true;

                FolderList.IsEnabled = false;
                SelectFolderButton.IsEnabled = false;
                UpButton.IsEnabled = false;

                var folders = await OneDriveService.GetFoldersAsync(folderId);

                _folders.Clear();
                foreach (var folder in folders)
                {
                    _folders.Add(folder);
                }

                NoSubfoldersLabel.IsVisible = _folders.Count == 0;

                _currentFolder = folderItem;
                UpdateHeader();
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "OneDrive Error",
                    $"Unable to load folders: {ex.Message}",
                    "OK");

                if (!CompletionSource.Task.IsCompleted)
                {
                    CompletionSource.TrySetResult(null);
                }

                await Navigation.PopModalAsync();
            }
            finally
            {
                FolderLoadingIndicator.IsRunning = false;
                FolderListOverlay.IsVisible = false;

                FolderList.IsEnabled = true;
                SelectFolderButton.IsEnabled = true;
                UpButton.IsEnabled = _currentFolder != null || _navigationStack.Count > 0;
            }
        }

        /// <summary>
        /// Updates the "Current Selection" label and Up button visibility based on the current folder.
        /// </summary>
        private void UpdateHeader()
        {
            if (_currentFolder == null)
            {
                CurrentFolderLabel.Text = "Current Selection: OneDrive root";
                UpButton.IsVisible = _navigationStack.Count > 0;
            }
            else
            {
                CurrentFolderLabel.Text = $"Current Selection: {_currentFolder.Name}";
                UpButton.IsVisible = true;
            }
        }

        /// <summary>
        /// Handles taps on a folder row and navigates into that folder.
        /// </summary>
        private async void OnFolderTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not DriveItem folder || folder.Folder == null)
                return;

            if (_currentFolder != null)
            {
                _navigationStack.Push(_currentFolder);
            }
            else
            {
                // Represent root in the stack with a synthetic item
                _navigationStack.Push(new DriveItem
                {
                    Id = "root",
                    Name = "OneDrive root"
                });
            }

            await LoadFoldersAsync(folder.Id!, folder);
        }

        /// <summary>
        /// Handles the "▲ Up" button tap and navigates to the parent folder or root.
        /// </summary>
        private async void OnUpClicked(object sender, EventArgs e)
        {
            if (_navigationStack.Count == 0)
            {
                await LoadFoldersAsync("root", null);
                return;
            }

            var parent = _navigationStack.Pop();

            if (parent.Id == "root")
            {
                await LoadFoldersAsync("root", null);
            }
            else
            {
                await LoadFoldersAsync(parent.Id!, parent);
            }
        }

        /// <summary>
        /// Confirms the current folder selection and returns it to the caller.
        /// If no folder has been navigated into, root is returned.
        /// </summary>
        private async void OnSelectFolderClicked(object sender, EventArgs e)
        {
            DriveItem? folderToUse = _currentFolder;

            if (folderToUse == null)
            {
                folderToUse = new DriveItem
                {
                    Id = "root",
                    Name = "OneDrive root"
                };
            }

            if (!CompletionSource.Task.IsCompleted)
            {
                CompletionSource.TrySetResult(folderToUse);
            }

            await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Cancels selection, returns null to the caller, and closes the modal.
        /// </summary>
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            if (!CompletionSource.Task.IsCompleted)
            {
                CompletionSource.TrySetResult(null);
            }

            await Navigation.PopModalAsync();
        }

        /// <summary>
        /// If the user dismisses via back gesture or system back, treat it as cancel.
        /// Ensures CompletionSource always completes.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (!CompletionSource.Task.IsCompleted)
            {
                CompletionSource.TrySetResult(null);
            }
        }
    }
}
