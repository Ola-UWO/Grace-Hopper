using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.IO;

namespace ReeveUnionManager.Services
{
    /// <summary>
    /// Provides access tokens to the Microsoft Graph SDK using the access token
    /// obtained from MSAL via <see cref="AuthService"/>.
    /// </summary>
    public class MsGraphTokenProvider : IAccessTokenProvider
    {
        /// <summary>
        /// Hosts that this token provider is valid for (defaults to graph.microsoft.com).
        /// </summary>
        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();

        /// <summary>
        /// Returns the current MSAL access token for use by the Graph SDK.
        /// </summary>
        public Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object>? additionalAuthenticationContext = default,
            CancellationToken cancellationToken = default)
        {
            // Parameters are part of the interface; we don't currently use them.
            _ = uri;
            _ = additionalAuthenticationContext;
            _ = cancellationToken;

            var token = AuthService.AccessToken;

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Not signed in. Call AuthService.SignInAsync() first.");

            return Task.FromResult(token);
        }
    }

    /// <summary>
    /// Helper functions for accessing the user's OneDrive via Microsoft Graph,
    /// including folder browsing, log file discovery, and file download.
    /// </summary>
    public static class OneDriveService
    {
        private const string SelectedFolderPreferenceKey = "ManagerLogs_OneDriveFolderName";
        private const string SelectedFolderIdPreferenceKey = "ManagerLogs_OneDriveFolderId";
        private const string DefaultFolderName = "Manager Logs (TEST)";

        /// <summary>
        /// The display name of the currently selected OneDrive folder for manager logs.
        /// Persisted using <see cref="Preferences"/>.
        /// </summary>
        public static string SelectedFolderName
        {
            get => Preferences.Get(SelectedFolderPreferenceKey, DefaultFolderName);
            set => Preferences.Set(SelectedFolderPreferenceKey, value ?? string.Empty);
        }

        /// <summary>
        /// The OneDrive item ID of the selected log folder, or null if none is stored.
        /// </summary>
        public static string? SelectedFolderId
        {
            get => Preferences.Get(SelectedFolderIdPreferenceKey, null);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Preferences.Remove(SelectedFolderIdPreferenceKey);
                }
                else
                {
                    Preferences.Set(SelectedFolderIdPreferenceKey, value);
                }
            }
        }

        /// <summary>
        /// Creates a GraphServiceClient that uses the current MSAL access token.
        /// </summary>
        private static GraphServiceClient CreateGraphClient()
        {
            var tokenProvider = new MsGraphTokenProvider();
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

            return new GraphServiceClient(authProvider);
        }

        /// <summary>
        /// Returns child folders for the specified parent folder ID ("root" for drive root).
        /// Only items with a non-null <see cref="DriveItem.Folder"/> are returned.
        /// </summary>
        public static async Task<IList<DriveItem>> GetFoldersAsync(string parentFolderId = "root")
        {
            var graphClient = CreateGraphClient();

            var drive = await graphClient.Me.Drive.GetAsync();
            if (drive == null || string.IsNullOrEmpty(drive.Id))
                return new List<DriveItem>();

            var children = await graphClient
                .Drives[drive.Id]
                .Items[parentFolderId]
                .Children
                .GetAsync();

            if (children?.Value == null)
                return new List<DriveItem>();

            return children.Value
                .Where(i => i.Folder != null)
                .ToList();
        }

        /// <summary>
        /// Lists .docx files from the selected OneDrive folder.
        /// If no valid stored folder ID exists, falls back to locating a folder by name
        /// (using <see cref="SelectedFolderName"/>) in the drive root.
        /// </summary>
        public static async Task<IList<DriveItem>> GetManagerLogFilesAsync()
        {
            var graphClient = CreateGraphClient();

            var drive = await graphClient.Me.Drive.GetAsync();
            if (drive == null || string.IsNullOrEmpty(drive.Id))
                return new List<DriveItem>();

            DriveItem? logsFolder = null;

            // Prefer stored folder ID if available
            if (!string.IsNullOrEmpty(SelectedFolderId))
            {
                try
                {
                    logsFolder = await graphClient
                        .Drives[drive.Id]
                        .Items[SelectedFolderId]
                        .GetAsync();
                }
                catch
                {
                    // ID is invalid (folder moved/deleted); fall back to name lookup below.
                    logsFolder = null;
                }
            }

            // Fallback: locate folder by name in root if ID is missing/invalid
            if (logsFolder == null)
            {
                var rootChildren = await graphClient
                    .Drives[drive.Id]
                    .Items["root"]
                    .Children
                    .GetAsync();

                var rootItems = rootChildren?.Value ?? new List<DriveItem>();

                logsFolder = rootItems
                    .FirstOrDefault(i =>
                        i.Folder != null &&
                        !string.IsNullOrEmpty(i.Name) &&
                        i.Name.Equals(SelectedFolderName, StringComparison.OrdinalIgnoreCase));
            }

            if (logsFolder == null || string.IsNullOrEmpty(logsFolder.Id))
                return new List<DriveItem>();

            var folderChildren = await graphClient
                .Drives[drive.Id]
                .Items[logsFolder.Id]
                .Children
                .GetAsync();

            var itemsInFolder = folderChildren?.Value ?? new List<DriveItem>();

            var docs = itemsInFolder
                .Where(i =>
                    i.Folder == null &&
                    !string.IsNullOrEmpty(i.Name) &&
                    i.Name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return docs;
        }

        /// <summary>
        /// Downloads the given OneDrive file to the app's cache directory and
        /// returns the local file path.
        /// </summary>
        public static async Task<string> DownloadFileToLocalAsync(DriveItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id))
                throw new ArgumentException("Invalid DriveItem. Cannot download.", nameof(item));

            var graphClient = CreateGraphClient();

            var drive = await graphClient.Me.Drive.GetAsync();
            if (drive == null || string.IsNullOrEmpty(drive.Id))
                throw new InvalidOperationException("Unable to resolve the user's OneDrive.");

            using var contentStream = await graphClient
                .Drives[drive.Id]
                .Items[item.Id]
                .Content
                .GetAsync();

            if (contentStream == null)
                throw new InvalidOperationException("Failed to download file content from OneDrive.");

            var fileName = string.IsNullOrEmpty(item.Name) ? "manager-log.docx" : item.Name;
            var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            using (var fileStream = File.Create(localPath))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            return localPath;
        }
    }
}
