using ReeveUnionManager.Models;
namespace ReeveUnionManager.Views;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class FoodServiceIssue : ContentPage
{
    private readonly ManagerLogObject _managerLogObject;

    public FoodServiceIssue(ManagerLogObject managerLogObject)
    {
        InitializeComponent();
        _managerLogObject = managerLogObject;
        BindingContext = _managerLogObject;
    }
}
