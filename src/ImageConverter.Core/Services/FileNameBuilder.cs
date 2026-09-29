using System.IO.Abstractions;

namespace ImageConverter.Core.Services;

/// <summary>
/// Generates the base name for the files the app saves
/// </summary>
public class FileNameBuilder(IFileSystem fileSystem)
{
    /// <summary>
    /// Checks if a proposed base name is already taken in any format.
    /// Generates a new base name if so
    /// </summary>
    /// <param name="folder">E.g. "/ConvertedImages"</param>
    /// <param name="baseName">E.g. "photo"</param>
    /// <param name="extensions">E.g. [ ".jpg", ".jpeg", ".png", ".webp" ]</param>
    /// <returns>E.g. "photo" or "photo-2"</returns>
    public string FindFreeBaseName(string folder, string baseName, IReadOnlyList<string> extensions)
    {
        string candidate = baseName;
        int number = 2;

        while (IsTakenInAnyFormat(folder, candidate, extensions))
        {
            candidate = $"{baseName}-{number}";
            number++;
        }

        return candidate;
    }

    private bool IsTakenInAnyFormat(string folder, string baseName, IReadOnlyList<string> extensions)
    {
        // TODO this can be optimised for performance
        foreach (string extension in extensions)
        {
            string path = fileSystem.Path.Combine(folder, $"{baseName}{extension}");

            if (fileSystem.File.Exists(path))
            {
                return true;
            }
        }

        return false;
    }
}
