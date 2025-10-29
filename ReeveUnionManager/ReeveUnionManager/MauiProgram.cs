using Microsoft.Extensions.Logging;
using ReeveUnionManager.Models;

namespace ReeveUnionManager;

/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public static class MauiProgram
{
	public static BusinessLogic businessLogic = new BusinessLogic(new Database());

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
