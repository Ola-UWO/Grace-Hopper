using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using QuickLook;
using UIKit;

namespace ReeveUnionManager.Services
{
    /// <summary>
    /// iOS-specific implementation of file preview using QuickLook.
    /// </summary>
    public static partial class FilePreviewService
    {
        public static partial Task PreviewAsync(string filePath, string? title = null)
        {
            // Create a QuickLook controller for a single file
            var previewController = new QLPreviewController
            {
                DataSource = new SingleFilePreviewDataSource(filePath, title)
            };

            // Find the top-most UIViewController to present from
            var window =
                UIApplication.SharedApplication.KeyWindow
                ?? UIApplication.SharedApplication
                    .ConnectedScenes
                    .OfType<UIWindowScene>()
                    .SelectMany(s => s.Windows)
                    .FirstOrDefault(w => w.IsKeyWindow);

            var root = window?.RootViewController;
            if (root == null)
            {
                // Nothing to present from; just bail out.
                return Task.CompletedTask;
            }

            var presenter = root;
            while (presenter.PresentedViewController != null)
            {
                presenter = presenter.PresentedViewController;
            }

            presenter.PresentViewController(previewController, true, null);

            // We don't need to wait for dismissal; just return a completed Task.
            return Task.CompletedTask;
        }

        private sealed class SingleFilePreviewDataSource : QLPreviewControllerDataSource
        {
            private readonly string _filePath;
            private readonly string? _title;

            public SingleFilePreviewDataSource(string filePath, string? title)
            {
                _filePath = filePath;
                _title = title;
            }

            // In this binding, PreviewItemCount is a METHOD, not a property.
            public override nint PreviewItemCount(QLPreviewController controller)
            {
                return 1;
            }

            public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
            {
                return new PreviewItem(_filePath, _title);
            }

            private sealed class PreviewItem : NSObject, IQLPreviewItem
            {
                private readonly string _filePath;
                private readonly string _name;

                public PreviewItem(string filePath, string? name)
                {
                    _filePath = filePath;
                    _name = string.IsNullOrEmpty(name)
                        ? Path.GetFileName(filePath)
                        : name;
                }

                // Expose the title using the selector QuickLook expects
                [Export("previewItemTitle")]
                public string ItemTitle => _name;

                // Expose the URL using the selector QuickLook expects
                [Export("previewItemURL")]
                public NSUrl ItemUrl => NSUrl.FromFilename(_filePath);
            }

        }
    }
}
