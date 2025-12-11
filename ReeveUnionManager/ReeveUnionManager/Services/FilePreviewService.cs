using System.Threading.Tasks;

namespace ReeveUnionManager.Services
{
    /// <summary>
    /// Cross-platform entry point for file preview.
    /// Implemented with QuickLook on iOS.
    /// </summary>
    public static partial class FilePreviewService
    {
        /// <summary>
        /// Shows a preview of the given local file.
        /// On iOS, uses QuickLook; on other platforms, can be extended later.
        /// </summary>
        public static partial Task PreviewAsync(string filePath, string? title = null);
    }
}
