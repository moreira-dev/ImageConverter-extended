using System.Globalization;
using System.Reflection;
using ImageConverter.Core.Services;
using ImageConverter.Core.Settings;
using Spectre.Console;

namespace ImageConverter.Cli.Options;

public class MenuSettings(
    ImageConversion conversionService,
    ConverterSettings settings,
    SettingsFile settingsFile) : IMenuOption
{
    private const string ShowChoice = "Show settings";
    private const string ModifyChoice = "Modify settings";
    private const string BackChoice = "Back";
    private const string SameFolderChoice = "Same folder as the original image";
    private const string ChooseFolderChoice = "Choose a folder";

    public string DisplayText { get; } = "Settings";

    private readonly FileBrowser _folderBrowser =
        new FileBrowser("Select the output folder", conversionService.ImageFormats);

    public void Run()
    {
        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Settings:[/]")
                .AddChoices(ShowChoice, ModifyChoice, BackChoice)
        );

        if (choice == ShowChoice)
        {
            ShowSettings();
        }
        else if (choice == ModifyChoice)
        {
            ModifySettings();
        }
    }

    /// <summary>
    /// Prints the settings file path and a table with the current settings
    /// </summary>
    private void ShowSettings()
    {
        AnsiConsole.MarkupLine($"Settings file: [green]{Markup.Escape(settingsFile.FilePath)}[/]");

        Table table = new Table();

        table.AddColumn("Setting");
        table.AddColumn("Value");

        foreach (PropertyInfo property in typeof(ConverterSettings).GetProperties())
        {
            string value = Convert.ToString(property.GetValue(settings), CultureInfo.CurrentCulture) ?? "";

            table.AddRow(Markup.Escape(property.Name), Markup.Escape(value));
        }

        AnsiConsole.Write(table);
    }

    private void ModifySettings()
    {
        settings.OutputFolder = AskForOutputFolder();
        settings.JpgQuality = AskForJpgQuality();

        settingsFile.Save(settings);

        AnsiConsole.MarkupLine($"Settings saved to [green]{Markup.Escape(settingsFile.FilePath)}[/]");
    }

    /// <returns>E.g. "." or "/foo/bar". The current folder when the user cancels</returns>
    private string AskForOutputFolder()
    {
        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Where should we save converted images?[/]")
                .AddChoices(SameFolderChoice, ChooseFolderChoice)
        );

        if (choice == SameFolderChoice)
        {
            return ConverterSettings.SameFolderAsOriginal;
        }

        return _folderBrowser.SelectFolder() ?? settings.OutputFolder;
    }

    private int AskForJpgQuality()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>("[bold]JPG quality, from 1 to 100:[/]")
                .DefaultValue(settings.JpgQuality)
                .Validate(quality => quality is >= 1 and <= 100, "[red]Enter a number from 1 to 100[/]"));
    }
}
