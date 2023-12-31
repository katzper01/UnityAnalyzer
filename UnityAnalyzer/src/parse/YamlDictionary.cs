using YamlDotNet.Serialization;

namespace UnityAnalyzer.parse;

public class YamlDictionary
{
    private readonly Dictionary<object, object> _data;
    
    public YamlDictionary(string yaml)
    {
        var deserializer = new Deserializer();
        var deserializedYaml = deserializer.Deserialize(yaml);
        _data = (Dictionary<object, object>)deserializedYaml! 
               ?? throw new Exception($"Invalid yaml for deserialization: {yaml}.");
    }

    public object? GetValue(string path)
    {
        if (path == "") return _data; 
        
        object value = _data;

        foreach (var s in path.Split("."))
        {
            if (value is not Dictionary<object, object> obj || !obj.ContainsKey(s))
                return null;

            value = obj[s];
        }

        return value;
    }

    public IEnumerable<string> GetKeys(string path)
    {
        var value = GetValue(path);
        return (value is not Dictionary<object, object> obj ? 
            [] : obj.Keys.ToArray()
                .Select(x => x.ToString())
                .Where(x => x != null))!;
    }
}