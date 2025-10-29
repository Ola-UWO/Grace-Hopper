using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class NewLogEntryPage : ContentPage
{
	public NewLogEntryPage()
	{
		InitializeComponent();
		BindingContext = new PageViewModel();
	}

}