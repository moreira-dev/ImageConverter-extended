using System.IO.Abstractions;
using ImageConverter.Cli.Options;
using ImageConverter.Core.Models.Converters;
using ImageConverter.Core.Services;
using ImageConverter.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ImageConverter.Cli;

internal abstract class Program
{
    private static string SettingsFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ImageConverter");

    private static void Main(string[] args)
    {
        string settingsPath = Path.Combine(SettingsFolder, "settings.json");

        FileSystem fileSystem = new FileSystem();
        SettingsFile settingsFile = new SettingsFile(fileSystem, settingsPath);
        ConverterSettings settings = settingsFile.Load();

        // Source of truth for supported formats. Update here when new formats are supported.
        FormatConverter[] supportedFormats = new FormatConverter[]
        {
            new JpgConverter(settings),
            new PngConverter(),
            new WebpConverter()
        };
        ImageConversion conversionService = new ImageConversion(supportedFormats, settings, fileSystem, SettingsFolder);
        ImageStats statsService = new ImageStats(conversionService.ImageFormats);

        // Dependency injection setup
        // TODO consider moving to Extension Members
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<ImageConversion>(conversionService);
        services.AddSingleton<ImageStats>(statsService);
        services.AddSingleton<SettingsFile>(settingsFile);
        services.AddSingleton<ConverterSettings>(settings);

        // New menu options here
        services.AddSingleton<IMenuOption, MenuOneImage>();
        services.AddSingleton<IMenuOption, MenuOneFolder>();
        services.AddSingleton<IMenuOption, MenuShowStats>();
        services.AddSingleton<IMenuOption, MenuSettings>();
        services.AddSingleton<IMenuOption, MenuExit>();
        services.AddSingleton<CliManager>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        CliManager cli = serviceProvider.GetRequiredService<CliManager>();

        cli.Run();
    }
}
