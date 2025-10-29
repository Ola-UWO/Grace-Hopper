using ReeveUnionManager.ViewModels;

namespace ReeveUnionManager.Views;

public partial class ManagerLogsPage : ContentPage
{
	public ManagerLogsPage()
	{
		InitializeComponent();
		BindingContext = new PageViewModel();
	}

}