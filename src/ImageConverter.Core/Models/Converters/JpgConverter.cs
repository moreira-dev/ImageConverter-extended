using ImageConverter.Core.Enums;
using ImageConverter.Core.Settings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageConverter.Core.Models.Converters;

public class JpgConverter(ConverterSettings settings) : FormatConverter
{
    public override ImageFormat Format { get; } = ImageFormat.JPG;
    public override string[] SupportedExtensions { get; } = new[] { ".jpg", ".jpeg" };

    protected override ImageEncoder Encoder
    {
        get { return new JpegEncoder { Quality = settings.JpgQuality }; }
    }

    protected override Image PrepareImage(Image image)
    {
        return image.Clone(context => context.BackgroundColor(Color.White));
    }
}
