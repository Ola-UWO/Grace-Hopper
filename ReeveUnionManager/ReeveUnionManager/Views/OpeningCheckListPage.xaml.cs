namespace ReeveUnionManager.Views;
/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public partial class OpeningCheckListPage : ContentPage
{
    public OpeningCheckListPage()
    {
        InitializeComponent();   
        BindingContext = new ViewModels.ChecklistViewModel(ChecklistType.Opening);
    }
}