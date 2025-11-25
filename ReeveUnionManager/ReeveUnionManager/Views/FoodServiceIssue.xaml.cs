using ReeveUnionManager.Models;
namespace ReeveUnionManager.Views;
/// <summary>
/// <!-- Kevin Kraiss -->
/// </summary>
public partial class FoodServiceIssue : ContentPage
{
    private readonly ManagerLogObject _log;

    public FoodServiceIssue(ManagerLogObject log)
    {
        InitializeComponent();
        _log = log;
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        _log.FoodServiceCategory = FoodServiceCategory.SelectedItem?.ToString();
        _log.FoodServiceLocation = FoodServiceLocation.Text;
        _log.FoodServiceDescription = FoodServiceDescription.Text;
        // _log.FoodServicePictures = FoodServicePictures.Image;          // FIXME: Another picture area

        Navigation.PopAsync();
    }
}
