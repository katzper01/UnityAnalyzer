using UnityAnalyzer;
using UnityAnalyzer.parse;

if (args.Length < 2) {
    PrintHelpMessage();
    return;
}

var projectPath = args[0];
var outputPath = args[1];

if (!Directory.Exists(projectPath))
    throw new Exception($"Specified unity project root directory: {projectPath} does not exist.");
if (!Directory.Exists(outputPath))
    throw new Exception($"Specified output directory: {outputPath} does not exist.");
        
var unityProjectParser = new UnityProjectParser(projectPath);

var scripts = unityProjectParser.ParseScripts().ToList();
var scenes = unityProjectParser.ParseScenes().ToList();
        
UnityAnalyzerService.PrintScenesHierarchy(scenes, outputPath);
UnityAnalyzerService.PrintUnusedScripts(scenes, scripts, outputPath);
return;

void PrintHelpMessage()
{
    Console.WriteLine("Usage: unity-analyzer unity_project_path output_folder_path");
}