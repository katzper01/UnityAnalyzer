using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityAnalyzer.data;

namespace UnityAnalyzer.parse;

public class UnityProjectParser(string projectRootPath)
{
    public IEnumerable<Script> ParseScripts()
    {
        var scriptFilePaths = Directory.GetFiles(projectRootPath, "*.cs", SearchOption.AllDirectories);

        // This could be 100% parallelized.
        return scriptFilePaths.Select(path =>
        {
            var serializedFields = GetSerializedFields(path);
            var guid = GetGuid(path);
            return new Script(path[(projectRootPath.Length+1)..], serializedFields, guid);
        });
    }

    private static HashSet<string> GetSerializedFields(string path)
    {
        var scriptFileContent = File.ReadAllText(path);
        
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
    
    private static string GetGuid(string path)
    {
        var metaFilePath = path + ".meta";
        var metaFileContent = File.ReadAllText(metaFilePath);

        var parsedMetaFile = new ParsedYamlDocument(metaFileContent);
        return parsedMetaFile.GetValue("guid")!.ToString()
               ?? throw new Exception($"Found no guid in script meta file: {path}.");
    }

    public IEnumerable<Scene> ParseScenes()
    {
        var sceneFiles = Directory.GetFiles(projectRootPath, "*.unity", SearchOption.AllDirectories);

        // This could be 100% parallelized.
        return sceneFiles.Select(CreateSceneFromFile);
    }

    private static Scene CreateSceneFromFile(string path)
    {
        var fidToGameObjectName = new Dictionary<long, string>();
        var gameObjectFidToTransform = new Dictionary<long, Transform>();
        var monoBehaviours = new List<MonoBehaviour>();
        
        var sceneName = path.Split("/").Last()[..^6];
        var sceneFileSections = File.ReadAllText(path).Split("--- ");
        
        foreach (var s in sceneFileSections[1..])
        {
            var (header, content) = s.Split('\n', count: 2) switch
            {
                { Length: 2 } a => (a[0], a[1]),
                _ => throw new Exception($"Invalid unity scene asset {s}.")
            };

            var fileId = Convert.ToInt64(header.Split('&')[1]);
            
            var asset = new ParsedYamlDocument(content);

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

    private static GenericAsset ToGenericAsset(ParsedYamlDocument parsed, string path)
    {
        var fid = Convert.ToInt64(parsed.GetValue($"{path}.fileID"));
        var guid = parsed.GetValue($"{path}.guid")?.ToString() ?? null;
        return new GenericAsset(fid, guid);
    }
}