using Microsoft.Extensions.Logging;
using ReeveUnionManager.Models;

namespace ReeveUnionManager;

/// <summary>
/// Author: Ola ft. Edgerunner
/// </summary>
public static class MauiProgram
{
	public static Database database = new Database();
	public static BusinessLogic businessLogic = new BusinessLogic(database);

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

		builder.Services.AddSingleton<ManagerLogObject>();

		return builder.Build();
	}
}
