namespace UnityAnalyzer.data;

public class MonoBehaviour(GenericAsset script, Dictionary<string, GenericAsset> serializedFieldsAssets)
{
    public GenericAsset Script => script;
    public Dictionary<string, GenericAsset> SerializedFieldsAssets => serializedFieldsAssets;
    
    private bool Equals(MonoBehaviour other)
    {
        return Script.Equals(other.Script)
            && SerializedFieldsAssets.SequenceEqual(other.SerializedFieldsAssets);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MonoBehaviour)obj);
    }

    public override int GetHashCode()
    {
        var hash = Script.GetHashCode();
        foreach (var (key, val) in SerializedFieldsAssets)
            hash ^= (key.GetHashCode() ^ val.GetHashCode());
        return hash;
    }
}