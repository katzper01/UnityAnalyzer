namespace UnityAnalyzer.filesystem;

public interface IFileSystem
{
    public bool DirectoryExists(string path);
    public bool FileExists(string path);
    public string[] GetFilesInDirectory(string path, string searchPattern);
    public string ReadAllTextFromFile(string path);
    public void WriteAllTextToFile(string path, string content);
}

public class FileSystem : IFileSystem
{
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string[] GetFilesInDirectory(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
    }

    public string ReadAllTextFromFile(string path)
    {
        return File.ReadAllText(path);
    }

    public void WriteAllTextToFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }
}