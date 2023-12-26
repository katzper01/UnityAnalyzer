using UnityAnalyzer.data;

namespace UnityAnalyzer;

public static class UnityAnalyzerService
{
    public static void PrintScenesHierarchy(List<Scene> scenes, string outputPath)
    {
        // This could be parallelized for sure.
        foreach (var scene in scenes)
        {
            File.WriteAllText($"{outputPath}/{scene.Name}.unity.dump", GetSceneHierarchy(scene));
        }
    }

    private static string GetSceneHierarchy(Scene scene)
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
        {
            HierarchyDfs(fid, 0, fidToGameObject, hierarchyGraph, dumpString);
        }
 
        return dumpString.ToString();
    }
    
    private static void HierarchyDfs(
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

    public static void PrintUnusedScripts(IEnumerable<Scene> scenes, IReadOnlyCollection<Script> scripts, string outputPath)
    {
        var dumpString = new StringWriter();
        dumpString.Write("Relative Path,GUID\n");
        
        var guidToScript = scripts.ToDictionary(
            x => x.Guid,
            x => x
        );
        
        var usedScriptsGuids = new HashSet<string>();
        
        foreach (var monoBehavior in scenes.SelectMany(scene => scene.MonoBehaviors))
        {
            if (monoBehavior.Script.Guid == null) continue;
            var script = guidToScript[monoBehavior.Script.Guid];
            usedScriptsGuids.Add(script.Guid);

            foreach (var (name, field) in monoBehavior.SerializedFieldsAssets)
            {
                if (field.Guid != null && script.SerializedFields.Contains(name))
                    usedScriptsGuids.Add(field.Guid);
            }
        }
        
        foreach (var script in scripts.Where(s => !usedScriptsGuids.Contains(s.Guid)))
        {
            dumpString.Write($"{script.Path},{script.Guid}\n");
        }
        
        File.WriteAllText($"{outputPath}/UnusedScripts.csv", dumpString.ToString());
    }
}