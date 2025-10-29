namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Caleb Wisneski
/// </summary>
public partial class UploadNotesTemplate : ContentView
{
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
    }
}