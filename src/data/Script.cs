namespace UnityAnalyzer.data;

public class Script(string path, HashSet<string> serializedFields, string guid)
{
    public string Path => path;
    public HashSet<string> SerializedFields => serializedFields;
    public string Guid => guid;
}