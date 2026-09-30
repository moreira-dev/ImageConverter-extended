using ImageConverter.Core.Settings;
using YoloDotNet.Models;

namespace ImageConverter.Core.AI;

/// <summary>
/// Renames a filename using AI labels
/// </summary>
public class ImageNamer(ModelFile modelFile, ConverterSettings settings) : IDisposable
{
    // Only loaded when AI naming is on
    private ImageClassifier? _classifier;

    /// <param name="sourcePath">E.g. "/foo/dog.jpg"</param>
    /// <returns>E.g. "dog" with AI naming off, or "dog-golden-retriever" with it on</returns>
    public string BuildBaseName(string sourcePath)
    {
        string originalName = Path.GetFileNameWithoutExtension(sourcePath);

        if (!settings.AiNaming)
        {
            return originalName;
        }

        _classifier = GetClassifier();

        Classification bestGuess = _classifier.Classify(sourcePath)[0]; // Takes the first best guess

        return $"{originalName}-{ToSlug(bestGuess.Label)}";
    }

    /// <param name="label">E.g. "Golden_retriever"</param>
    /// <returns>E.g. "golden-retriever"</returns>
    public static string ToSlug(string label)
    {
        return label.ToLowerInvariant().Replace('_', '-').Replace(' ', '-');
    }

    /// <summary>
    /// Downloads and loads the model
    /// </summary>
    public ImageClassifier GetClassifier()
    {
        if (_classifier != null)
        {
            return _classifier;
        }

        if (!modelFile.Exists)
        {
            modelFile.Download();
        }

        return new ImageClassifier(modelFile.FilePath);
    }

    public void Dispose()
    {
        _classifier?.Dispose();
        GC.SuppressFinalize(this);
    }
}
