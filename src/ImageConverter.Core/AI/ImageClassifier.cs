using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Models;

namespace ImageConverter.Core.AI;

/// <summary>
/// Guesses what an image shows, using short labels
/// </summary>
public class ImageClassifier : IDisposable
{
    private const int GuessCount = 1;

    private readonly Yolo _yolo;

    /// <param name="modelPath">E.g. "~/Library/Application Support/ImageConverter/model.onnx"</param>
    public ImageClassifier(string modelPath)
    {
        _yolo = new Yolo(new YoloOptions
        {
            ExecutionProvider = new CpuExecutionProvider(modelPath)
        });
    }

    /// <param name="imagePath">E.g. "/foo/dog.jpg"</param>
    /// <returns>The top GuessCounts</returns>
    public IReadOnlyList<Classification> Classify(string imagePath)
    {
        using SKBitmap image = SKBitmap.Decode(imagePath)
                               ?? throw new InvalidDataException($"{imagePath} is not a valid image.");

        return _yolo.RunClassification(image, GuessCount);
    }

    public void Dispose()
    {
        _yolo.Dispose();
        GC.SuppressFinalize(this);
    }
}
