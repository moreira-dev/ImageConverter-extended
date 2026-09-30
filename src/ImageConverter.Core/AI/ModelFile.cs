using System.IO.Abstractions;

namespace ImageConverter.Core.AI;

/// <summary>
/// Manages the download of the AI model file used for classification
/// </summary>
/// <param name="folderPath">E.g. "~/Library/Application Support/ImageConverter/"</param>
public class ModelFile(IFileSystem fileSystem, string folderPath)
{
    // TODO we should build our own onnx model
    private static string _downloadUrl =
        "https://raw.githubusercontent.com/NickSwardh/YoloDotNet/c5845b1c51690e9c734453ef7428b54b7f22f636/test/assets/Models/yolov11s-cls.onnx";

    public static string FileName { get; } = "yolov11s-cls.onnx";

    public string FilePath { get; } = fileSystem.Path.Combine(folderPath, FileName);

    public bool Exists
    {
        get { return fileSystem.File.Exists(FilePath); }
    }


    /// <summary>
    /// Downloads the selected model
    /// </summary>
    public void Download()
    {
        using HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        byte[] model = client.GetByteArrayAsync(new Uri(_downloadUrl)).GetAwaiter().GetResult();

        string? folder = fileSystem.Path.GetDirectoryName(FilePath);

        if (folder != null)
        {
            fileSystem.Directory.CreateDirectory(folder);
        }

        fileSystem.File.WriteAllBytes(FilePath, model);
    }
}
