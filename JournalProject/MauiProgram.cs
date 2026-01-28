using Microsoft.Extensions.Logging;
using JournalProject.Data;
using JournalProject.Services;

namespace JournalProject;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		
		// Register Services
		builder.Services.AddSingleton<AppDbContext>();
		builder.Services.AddSingleton<UserService>();
		builder.Services.AddSingleton<JournalService>();
		builder.Services.AddSingleton<TagService>();
		builder.Services.AddSingleton<StreakService>();
		builder.Services.AddSingleton<SearchService>();
		builder.Services.AddSingleton<AnalyticsService>();
		builder.Services.AddSingleton<ExportService>();
		builder.Services.AddSingleton<SecurityService>();
		builder.Services.AddSingleton<ThemeService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
