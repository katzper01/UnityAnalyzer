using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityAnalyzer.data;
using UnityAnalyzer.filesystem;
using UnityAnalyzer.parse;

namespace UnityAnalyzer.service;

public interface IParser
{
    public List<Script> ParseScripts(string projectRootPath);
    public List<Scene> ParseScenes(string projectRootPath);
}

public class Parser(IFileSystem fileSystem) : IParser
{
    public List<Script> ParseScripts(string projectRootPath)
    {
        var scriptFilePaths = fileSystem.GetFilesInDirectory(projectRootPath, "*.cs");
        
        return scriptFilePaths
            .AsParallel()
            .Select(path =>
            {
                var serializedFields = GetSerializedFields(path);
                var guid = GetGuid(path);
                return new Script(path[(projectRootPath.Length + 1)..], serializedFields, guid);
            })
            .ToList();
    }

    private HashSet<string> GetSerializedFields(string path)
    {
        var scriptFileContent = fileSystem.ReadAllTextFromFile(path);
        
        return
        [
            ..CSharpSyntaxTree.ParseText(scriptFileContent)
                .GetCompilationUnitRoot()
                .Members[0]
                .ChildNodes()
                .Where(node => node.IsKind(SyntaxKind.FieldDeclaration))
                .Select(field => ((FieldDeclarationSyntax)field).Declaration.Variables)
                .SelectMany(x => x)
                .Select(var => var.ToString())
                .ToArray()
        ];
    }
    
    private string GetGuid(string path)
    {
        var metaFilePath = $"{path}.meta";

        if (!fileSystem.FileExists(metaFilePath))
            throw new Exception($"Script meta file not found for {path}.");
        
        var metaFileContent = fileSystem.ReadAllTextFromFile(metaFilePath);
        var parsedMetaFile = new YamlDictionary(metaFileContent);
        
        return parsedMetaFile.GetValue("guid")?.ToString()
               ?? throw new Exception($"Found no guid in script meta file: {metaFilePath}.");
    }

    public List<Scene> ParseScenes(string projectRootPath)
    {
        var sceneFiles = fileSystem.GetFilesInDirectory(projectRootPath, "*.unity");
        
        return sceneFiles
            .AsParallel()
            .Select(CreateSceneFromFile)
            .ToList();
    }

    private Scene CreateSceneFromFile(string path)
    {
        var fidToGameObjectName = new Dictionary<long, string>();
        var gameObjectFidToTransform = new Dictionary<long, Transform>();
        var monoBehaviours = new List<MonoBehaviour>();
        
        var sceneName = path.Split("/").Last()[..^6];
        var sceneFileSections = fileSystem.ReadAllTextFromFile(path).Split("--- ");
        
        foreach (var s in sceneFileSections[1..])
        {
            var (header, content) = s.Split('\n', count: 2) switch
            {
                { Length: 2 } a => (a[0], a[1]),
                _ => throw new Exception($"Invalid unity scene: {path}.")
            };

            var fileId = Convert.ToInt64(header.Split('&')[1]);
            
            var asset = new YamlDictionary(content);

            if (asset.GetValue("GameObject") != null)
            {
                fidToGameObjectName[fileId] = asset.GetValue("GameObject.m_Name")!.ToString()!;
            } 
            else if (asset.GetValue("Transform") != null)
            {
                var gameObjectFid = Convert.ToInt64(asset.GetValue("Transform.m_GameObject.fileID"));
                var fatherTransformFid = Convert.ToInt64(asset.GetValue("Transform.m_Father.fileID"));
                gameObjectFidToTransform[gameObjectFid] = new Transform(fileId, gameObjectFid, fatherTransformFid);
            } 
            else if (asset.GetValue("MonoBehaviour") != null)
            {
                var script = ToGenericAsset(asset, "MonoBehaviour.m_Script");

                var serializedKeys = asset.GetKeys("MonoBehaviour")
                    .Where(x => !Regex.IsMatch(x, "^m_*"));
                
                var serializedFieldsAssets = serializedKeys
                    .ToDictionary(
                        key => key,
                        key => ToGenericAsset(asset, $"MonoBehaviour.{key}"));
                
                monoBehaviours.Add(new MonoBehaviour(script, serializedFieldsAssets));   
            }
        }

        var gameObjects = fidToGameObjectName
            .Select(pair =>
            {
                var fid = pair.Key;
                var name = pair.Value;
                var transform = gameObjectFidToTransform[fid] ??
                                throw new Exception($"Found GameObject with fid:{fid} without transform.");
                return new GameObject(fid, name, transform);
            }).ToList();

        return new Scene(sceneName, gameObjects, monoBehaviours);
    }

    private GenericAsset ToGenericAsset(YamlDictionary parsed, string path)
    {
        var fid = Convert.ToInt64(parsed.GetValue($"{path}.fileID"));
        var guid = parsed.GetValue($"{path}.guid")?.ToString() ?? null;
        return new GenericAsset(fid, guid);
    }
}