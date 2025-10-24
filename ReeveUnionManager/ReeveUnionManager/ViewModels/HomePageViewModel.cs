using System.Windows.Input;

namespace ReeveUnionManager.ViewModels;

public class HomePageViewModel
{
    public ICommand NavigateCommand { get; }

    public HomePageViewModel()
    {
        NavigateCommand = new Command<string>(OnNavigate);
    }

    private async void OnNavigate(string? routeName)
    {
        if (string.IsNullOrWhiteSpace(routeName)) return;
        await Shell.Current.GoToAsync(routeName);
    }
}
