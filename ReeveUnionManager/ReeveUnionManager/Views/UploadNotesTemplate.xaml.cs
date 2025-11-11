namespace ReeveUnionManager.Views;

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
        BindingContext = MauiProgram.businessLogic;
    }

    public async void OnUploadPhotoClicked(object sender, EventArgs args)
    {
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
    }

    public async void OnCapturePhotoClicked(object sender, EventArgs args)
    {
        FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
    }
    /* 
    public async void HandleSubmit(object sender, EventArgs args)
    {

        string notes = NotesBox.Text;


        CallLogError error = await MauiProgram.businessLogic.AddCallLog(callId, callerName, timeOfCall, callNotes);
        if (error != CallLogError.None)
        {
            await DisplayAlert("Addition has failed", error.ToString(), "OK");
        }
    }

    */
}