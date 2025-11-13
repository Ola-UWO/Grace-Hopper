namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public partial class ClosingCheckListPage : ContentPage
{
    public ClosingCheckListPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.ChecklistViewModel(ChecklistType.Closing);
    }
}