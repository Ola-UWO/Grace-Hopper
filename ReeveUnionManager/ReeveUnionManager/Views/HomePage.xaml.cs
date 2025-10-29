namespace ReeveUnionManager.Views;
using ReeveUnionManager.ViewModels;
/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new PageViewModel();
	}
}

