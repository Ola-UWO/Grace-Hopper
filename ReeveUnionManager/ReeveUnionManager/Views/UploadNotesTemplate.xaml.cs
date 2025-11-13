namespace ReeveUnionManager.Views;

using System.Collections.ObjectModel;
using ReeveUnionManager.Models;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
/// <summary>
/// Author: Caleb Wisneski
/// </summary>
public partial class UploadNotesTemplate : ContentView
{
    public ObservableCollection<PhotoInfo> Photos { get; set; } = new();
    FileResult photo;

    public static readonly BindableProperty TitleProperty
        = BindableProperty.Create(nameof(Title), typeof(string), typeof(UploadNotesTemplate), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public UploadNotesTemplate()
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
            Photos.Add(new PhotoInfo{
                Image = ImageSource.FromFile(newFile), FileName = photo.FileName});
        }
    }

    public async void HandleSubmit(object sender, EventArgs args)
    {

        string notes = NotesBox.Text;

        BasicEntryError error = await MauiProgram.businessLogic.AddBasicEntry(Title, notes, Photos);
        if (error != BasicEntryError.None)
        {
            //await DisplayAlert("Addition has failed", error.ToString(), "OK");
        }
    }
    
    
}