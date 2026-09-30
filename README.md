# Image Converter

A command-line tool for converting images between JPG, PNG and WebP.

Image Converter is a simple .NET CLI project to practice good OOP principles.

![Menu](docs/Menu.png)

It is a first attempt at creating a useful CLI tool with .NET. It will serve as a foundation for a more advance project in the future.

The program places special focus on extensibility, allowing developers to add new menu options and image formats without modifying the existing code.

To add a new image format see [Adding a new image format](#adding-a-new-image-format).
To add a new menu option see [Adding a new menu option](#adding-a-new-menu-option).

## Technology

* [.NET 10](https://learn.microsoft.com/dotnet/) with C#
* [ImageSharp](https://docs.sixlabors.com/articles/imagesharp/) to read and convert the images
* [Spectre.Console](https://spectreconsole.net/) for the CLI menus
* [YoloDotNet](https://github.com/NickSwardh/YoloDotNet) to run the YOLO11s classification model from [Ultralytics](https://docs.ultralytics.com/tasks/classify/)

## Features

* **Convert one image.** You can browse your system for an image file and convert it to a chosen format.
* **Convert all images in a folder.** Browse your system for a folder and convert all images in it to a chosen format.
* **AI naming.** Uses an image classification model to tag your converted images with contextual information. For example, `dog.jpg` converted to WebP gets saved as `dog-golden-retriever.webp` (provided there is a golden retriever in the image!).
* **Show image stats.** Choose a folder to see stats of all images within it.

## Settings

The app saves its settings in a file `settings.json` inside a folder in your application data folder. Use **Settings** in the menu to see or change them.

| OS | Path |
|---|---|
| Windows | `%APPDATA%\ImageConverter\settings.json` |
| Linux | `~/.config/ImageConverter/settings.json` |
| Mac | `~/Library/Application Support/ImageConverter/settings.json` |

## Local development

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

To run the app:

```
dotnet run --project src/ImageConverter.Cli
```

To create a build:

```
dotnet build
```

The app is built as `imgconv` in `src/ImageConverter.Cli/bin/`.

### Adding a new image format

Each format is a subclass of [FormatConverter](src/ImageConverter.Core/Models/Converters/FormatConverter.cs).

To add a format:

1. Add the format to [ImageFormat](src/ImageConverter.Core/Enums/ImageFormat.cs).
2. Create a new converter in `src/ImageConverter.Core/Models/Converters/` that inherits from [FormatConverter](src/ImageConverter.Core/Models/Converters/FormatConverter.cs).
3. Register the converter in [ServiceCollectionExtensions.cs](src/ImageConverter.Cli/ServiceCollectionExtensions.cs) with `services.AddSingleton<FormatConverter, YourConverter>()`.

### Adding a new menu option

Each menu option is a class that implements [IMenuOption](src/ImageConverter.Cli/Options/IMenuOption.cs).

To add a menu option:

1. Create a new class in `src/ImageConverter.Cli/Options/` that implements [IMenuOption](src/ImageConverter.Cli/Options/IMenuOption.cs).
2. Set `DisplayText` to the text shown in the menu, and put the option's logic in `Run()`.
3. Register the class in [ServiceCollectionExtensions.cs](src/ImageConverter.Cli/ServiceCollectionExtensions.cs) with `services.AddSingleton<IMenuOption, YourOption>()`.

Options appear in the menu in the order they are registered, so add yours before `MenuExit`.

## Limitations and opportunities for improvement

* Converting a folder does not include its subfolders, and it's not clear to the user if it will
* The app needs to facilitate adding settings to all image encoders
* AI functionality is very basic at the moment. Labels are not specific enough to truly be useful

## License

See [LICENSE](LICENSE).
