using ReeveUnionManager.Models;

namespace ReeveUnionManager.Views;

using System.Collections.ObjectModel;
using ReeveUnionManager.Models;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class EventSupportChanges : ContentPage
{

    private readonly ManagerLogObject _log;


    public ObservableCollection<PhotoInfo> Photos { get; set; } = new();
    FileResult photo;

    public EventSupportChanges(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
        BindingContext = this;
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        _log.EventSupportChangesName = NameBox.Text;
        _log.EventSupportChangesTime = TimeSelector.Time.ToString(@"hh\:mm");
        _log.EventSupportChangesLocation = LocationBox.Text;
        _log.EventSupportChangesDetails = NotesBox.Text;
        _log.EventSupportChangesPictures = Photos;
        
        Navigation.PopAsync();
    }

    public async void OnUploadPhotoClicked(object sender, EventArgs args)
    {
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(newFile))
        {
            await stream.CopyToAsync(newStream);
        }
            
        if (photo != null)
        {
            Photos.Add(new PhotoInfo{
                Image = ImageSource.FromFile(newFile), FileName = photo.FileName});
        }
    }


    public async void OnCapturePhotoClicked(object sender, EventArgs args)
    {
        FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

        //If no photo is captured return to prevent crash
        if (photo == null)
        {
            return;
        }
        
        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(newFile))
        {
            await stream.CopyToAsync(newStream);
        }

        if (photo != null)
        {
            Photos.Add(new PhotoInfo
            {
                Image = ImageSource.FromFile(newFile),
                FileName = photo.FileName
            });
        }
    }
    
    public async void RemoveImage(object sender, EventArgs args)
    {
        
        if (sender is Button button && button.CommandParameter is PhotoInfo photo)
        {

            Photos.Remove(photo);

        }
    }
}
