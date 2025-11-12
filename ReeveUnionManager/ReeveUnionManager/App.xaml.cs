namespace ReeveUnionManager;
/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        _ = MauiProgram.businessLogic.Scrape25Live();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}