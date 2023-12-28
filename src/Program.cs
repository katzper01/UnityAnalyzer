using UnityAnalyzer.filesystem;
using UnityAnalyzer.service;

if (args.Length < 2) {
    PrintHelpMessage();
    return;
}

var projectRootPath = args[0];
var outputPath = args[1];

var fileSystem = new FileSystem();

var unityAnalyzer = new UnityAnalyzer.UnityAnalyzer(
    fileSystem,
    new Parser(fileSystem),
    new Analyzer(fileSystem)
);

unityAnalyzer.Start(projectRootPath, outputPath);

return;

void PrintHelpMessage()
{
    Console.WriteLine("Usage: unity-analyzer unity_project_path output_folder_path");
}