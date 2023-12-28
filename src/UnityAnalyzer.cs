using UnityAnalyzer.filesystem;
using UnityAnalyzer.service;

namespace UnityAnalyzer;

public class UnityAnalyzer(IFileSystem fileSystem, IParser parser, IAnalyzer analyzer)
{
    public void Start(string projectRootPath, string outputPath)
    {
        if (!fileSystem.DirectoryExists(projectRootPath))
            throw new Exception($"Specified unity project root directory: {projectRootPath} does not exist.");
        if (!fileSystem.DirectoryExists(outputPath))
            throw new Exception($"Specified output directory: {outputPath} does not exist.");
        
        var scripts = parser.ParseScripts(projectRootPath);
        var scenes = parser.ParseScenes(projectRootPath);

        analyzer.PrintScenesHierarchy(scenes, outputPath);
        analyzer.PrintUnusedScripts(scenes, scripts, outputPath);
    }
}