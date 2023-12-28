namespace UnityAnalyzer.data;

public class GenericAsset(long? fid, string? guid)
{
    private long? Fid => fid;
    public string? Guid => guid;
    
    private bool Equals(GenericAsset other)
    {
        return Nullable.Equals(Fid, other.Fid) && Nullable.Equals(Guid, other.Guid);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((GenericAsset)obj);
    }

    public override int GetHashCode()
    {
        return (Fid?.GetHashCode() ?? 0) ^ (Guid?.GetHashCode() ?? 0);
    }
}