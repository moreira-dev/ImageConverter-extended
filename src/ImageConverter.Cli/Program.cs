using Microsoft.Extensions.DependencyInjection;

namespace ImageConverter.Cli;

internal abstract class Program
{
    private static void Main()
    {
        string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ImageConverter");

        using ServiceProvider serviceProvider = new ServiceCollection()
            .AddImageConverter(settingsFolder)
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<CliManager>().Run();
    }
}
