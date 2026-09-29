using ImageConverter.Core.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageConverter.Core.Models.Converters;

public class JpgConverter : FormatConverter
{
    public override ImageFormat Format { get; } = ImageFormat.JPG;
    public override string[] SupportedExtensions { get; } = new[] { ".jpg", ".jpeg" };
    protected override ImageEncoder Encoder { get; } = new JpegEncoder();

    protected override Image PrepareImage(Image image)
    {
        return image.Clone(context => context.BackgroundColor(Color.White));
    }
}
