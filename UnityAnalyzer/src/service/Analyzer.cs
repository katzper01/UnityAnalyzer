using UnityAnalyzer.data;
using UnityAnalyzer.filesystem;

namespace UnityAnalyzer.service;

public interface IAnalyzer
{
    public void PrintScenesHierarchy(List<Scene> scenes, string outputPath);
    public void PrintUnusedScripts(IEnumerable<Scene> scenes, IReadOnlyCollection<Script> scripts, string outputPath);
}

public class Analyzer(IFileSystem fileSystem) : IAnalyzer
{ 
    public void PrintScenesHierarchy(List<Scene> scenes, string outputPath)
    {
        Parallel.ForEach(scenes, scene =>
            fileSystem.WriteAllTextToFile($"{outputPath}/{scene.Name}.unity.dump", GetSceneHierarchy(scene))
        );
    }

    private string GetSceneHierarchy(Scene scene)
    {
        var fidToGameObject = new Dictionary<long, GameObject>();
        var fidToTransform = new Dictionary<long, Transform>();
        var hierarchyGraph = new Dictionary<long, List<long>>();
        var nonRoots = new HashSet<long>();

        foreach (var gameObject in scene.GameObjects)
        {
            fidToGameObject[gameObject.Fid] = gameObject;
            fidToTransform[gameObject.Transform.Fid] = gameObject.Transform;
            hierarchyGraph[gameObject.Fid] = [];
        }

        foreach (var (_, childTransform) in fidToTransform)
        {
            var fatherTransformFid = childTransform.FatherTransformFid;
            if (fatherTransformFid == 0) continue;
            var fatherTransform = fidToTransform[fatherTransformFid];
            hierarchyGraph[fatherTransform.GameObjectFid].Add(childTransform.GameObjectFid);
            nonRoots.Add(childTransform.GameObjectFid);
        }

        var dumpString = new StringWriter();
        
        foreach (var fid in fidToGameObject.Keys.Where(fid => !nonRoots.Contains(fid)))
            HierarchyDfs(fid, 0, fidToGameObject, hierarchyGraph, dumpString);
 
        return dumpString.ToString();
    }
    
    private void HierarchyDfs(
        long vertexFid, 
        int depth, 
        IReadOnlyDictionary<long, GameObject> fidToGameObject, 
        IReadOnlyDictionary<long, List<long>> hierarchyGraph, 
        TextWriter dumpString
    ) {
        for (var i = 0; i < depth; i++) dumpString.Write("--");
        dumpString.Write(fidToGameObject[vertexFid].Name + "\n");
        foreach (var childFid in hierarchyGraph[vertexFid])
        {
            HierarchyDfs(childFid, depth+1, fidToGameObject, hierarchyGraph, dumpString);   
        }
    }

    public void PrintUnusedScripts(IEnumerable<Scene> scenes, IReadOnlyCollection<Script> scripts, string outputPath)
    {
        var dumpString = new StringWriter();
        dumpString.Write("Relative Path,GUID\n");
        
        var guidToScript = scripts.ToDictionary(
            x => x.Guid,
            x => x
        );
        
        var usedScriptsGuids = new HashSet<string>();
        
        foreach (var monoBehaviour in scenes.SelectMany(scene => scene.MonoBehaviours))
        {
            if (monoBehaviour.Script.Guid == null) continue;
            var script = guidToScript[monoBehaviour.Script.Guid];
            usedScriptsGuids.Add(script.Guid);

            foreach (var (name, field) in monoBehaviour.SerializedFieldsAssets)
            {
                if (field.Guid != null && script.SerializedFields.Contains(name))
                    usedScriptsGuids.Add(field.Guid);
            }
        }
        
        foreach (var script in scripts.Where(s => !usedScriptsGuids.Contains(s.Guid)))
        {
            dumpString.Write($"{script.Path},{script.Guid}\n");
        }
        
        fileSystem.WriteAllTextToFile($"{outputPath}/UnusedScripts.csv", dumpString.ToString());
    }
}