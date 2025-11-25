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

    public ObservableCollection<PhotoInfo> Photos { get; set; } = new();
    FileResult photo;

    public EventSupportChanges()
    {
        InitializeComponent();
        BindingContext = this;
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

    public async void HandleSubmit(object sender, EventArgs args)
    {
        string name = NameBox.Text;
        TimeOnly time = TimeOnly.FromTimeSpan(TimePicker.Time);
        string location = LocationBox.Text;
        string notes = NotesBox.Text;

        BasicEntryError error = await MauiProgram.businessLogic.AddEventSupportChange(name, time, location, notes, Photos);
        if (error != BasicEntryError.None)
        {
            //await DisplayAlert("Addition has failed", error.ToString(), "OK");
        }
        

        await Navigation.PopAsync();
    }


}
