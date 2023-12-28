namespace UnityAnalyzer.data;

public class Script(string path, HashSet<string> serializedFields, string guid)
{ 
    public string Path => path;
    public HashSet<string> SerializedFields => serializedFields;
    public string Guid => guid;

    private bool Equals(Script other)
    {
        return Path.Equals(other.Path)
               && SerializedFields.SetEquals(other.SerializedFields)
               && Guid.Equals(other.Guid);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Script)obj);
    }

    public override int GetHashCode()
    {
        var hash = Path.GetHashCode() ^ Guid.GetHashCode();
        return SerializedFields.Aggregate(hash, (current, field) => current ^ field.GetHashCode());
    }
}