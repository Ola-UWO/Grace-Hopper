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
    /// Provides access tokens to the Graph SDK using the token we already got from MSAL.
    /// </summary>
    public class MsGraphTokenProvider : IAccessTokenProvider
    {
        // Which hosts this token provider is valid for (default allows graph.microsoft.com)
        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();

        public Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object> additionalAuthenticationContext = default!,
            CancellationToken cancellationToken = default)
        {
            var token = AuthService.AccessToken;

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Not signed in. Call AuthService.SignInAsync() first.");

            // Just hand the raw bearer token to Graph
            return Task.FromResult(token);
        }
    }

    public static class OneDriveService
    {
        private const string FolderName = "Manager Logs (TEST)";

        private static GraphServiceClient CreateGraphClient()
        {
            var tokenProvider = new MsGraphTokenProvider();
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

            return new GraphServiceClient(authProvider);
        }

        /// <summary>
        /// Lists .docx files from the "Manager Logs (TEST)" folder in the user's OneDrive root.
        /// </summary>
        public static async Task<IList<DriveItem>> GetManagerLogFilesAsync()
        {
            var graphClient = CreateGraphClient();

            // 1) Get the current user's default drive
            var drive = await graphClient.Me.Drive.GetAsync();
            if (drive == null || string.IsNullOrEmpty(drive.Id))
                return new List<DriveItem>();

            // 2) Get the root folder’s children using the v5 pattern:
            //    /drives/{drive-id}/items/root/children
            var rootChildren = await graphClient
                .Drives[drive.Id]
                .Items["root"]
                .Children
                .GetAsync();

            var rootItems = rootChildren?.Value ?? new List<DriveItem>();

            // 3) Find the "Manager Logs (TEST)" folder by name
            var logsFolder = rootItems
                .FirstOrDefault(i => i.Folder != null && i.Name == FolderName);

            if (logsFolder == null || string.IsNullOrEmpty(logsFolder.Id))
                return new List<DriveItem>(); // folder not found, just return empty

            // 4) Get the children of that folder
            var folderChildren = await graphClient
                .Drives[drive.Id]
                .Items[logsFolder.Id]
                .Children
                .GetAsync();

            var itemsInFolder = folderChildren?.Value ?? new List<DriveItem>();


            // 5) Filter to `.docx` items that are not folders.
            var docs = itemsInFolder
                .Where(i =>
                    i.Folder == null &&
                    !string.IsNullOrEmpty(i.Name) &&
                    i.Name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return docs;
        }

        public static async Task<string> DownloadFileToLocalAsync(DriveItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id))
                throw new ArgumentException("Invalid DriveItem. Cannot download.", nameof(item));

            var graphClient = CreateGraphClient();

            // Get the current user's drive again (cheap, and keeps things simple)
            var drive = await graphClient.Me.Drive.GetAsync();
            if (drive == null || string.IsNullOrEmpty(drive.Id))
                throw new InvalidOperationException("Unable to resolve the user's OneDrive.");

            // Download the file content
            using var contentStream = await graphClient
                .Drives[drive.Id]
                .Items[item.Id]
                .Content
                .GetAsync();

            if (contentStream == null)
                throw new InvalidOperationException("Failed to download file content from OneDrive.");

            // Save to app's cache directory
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
