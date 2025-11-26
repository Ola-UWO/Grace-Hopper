using System.Diagnostics;
using ReeveUnionManager.Models;
using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class NewLogEntryPage : ContentPage
{
	private readonly ManagerLogObject _log;

	public NewLogEntryPage(ManagerLogObject log)
	{
		InitializeComponent();
		BindingContext = new PageViewModel();
		_log = log;
	}

	private async void OnSubmitClicked(object sender, EventArgs e)
    {
		Debug.WriteLine("Before Log Build");
        var result = await MauiProgram.businessLogic.CreateManagerLogFile(_log);
		Debug.WriteLine("After Log Build");

		if (result == ManagerLogError.None)
			await DisplayAlert("Success", "Manager log created!", "OK");
		else
			await DisplayAlert("Error", "Could not create log file.", "OK");

		await Navigation.PopAsync();
    }
}