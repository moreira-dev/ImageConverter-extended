using System.IO.Abstractions;
using ImageConverter.Cli.Options;
using ImageConverter.Core.Models.Converters;
using ImageConverter.Core.Services;
using ImageConverter.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ImageConverter.Cli;

/// <summary>
/// Sets up the dependency injection container
/// </summary>
internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <param name="settingsFolder">The place to store the app's settings, e.g. "~/Library/Application Support/ImageConverter"</param>
        public IServiceCollection AddImageConverter(string settingsFolder)
        {
            string settingsPath = Path.Combine(settingsFolder, "settings.json");

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<SettingsFile>(provider =>
                new SettingsFile(provider.GetRequiredService<IFileSystem>(), settingsPath)
            );
            services.AddSingleton<ConverterSettings>(provider => provider.GetRequiredService<SettingsFile>().Load());
            services.AddSingleton<ImageConversion>(provider => new ImageConversion(
                provider.GetServices<FormatConverter>().ToArray(),
                provider.GetRequiredService<ConverterSettings>(),
                provider.GetRequiredService<IFileSystem>(),
                settingsFolder));

            services.AddSingleton<ImageStats>();

            // Source of truth for supported formats. Register new formats here.
            services.AddSingleton<FormatConverter, JpgConverter>();
            services.AddSingleton<FormatConverter, PngConverter>();
            services.AddSingleton<FormatConverter, WebpConverter>();

            // Options appear in the menu in the order they are registered
            services.AddSingleton<IMenuOption, MenuOneImage>();
            services.AddSingleton<IMenuOption, MenuOneFolder>();
            services.AddSingleton<IMenuOption, MenuShowStats>();
            services.AddSingleton<IMenuOption, MenuSettings>();
            services.AddSingleton<IMenuOption, MenuExit>();
            services.AddSingleton<CliManager>();

            return services;
        }
    }
}
