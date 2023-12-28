namespace UnityAnalyzer.data;

public class Transform(long fid, long gameObjectFid, long fatherTransformFid)
{
    public long Fid => fid;
    public long GameObjectFid => gameObjectFid;
    public long FatherTransformFid => fatherTransformFid;
    
    private bool Equals(Transform other)
    {
        return Fid.Equals(other.Fid)
               && GameObjectFid.Equals(other.GameObjectFid)
               && FatherTransformFid.Equals(other.FatherTransformFid);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Transform)obj);
    }

    public override int GetHashCode()
    {
        return Fid.GetHashCode() ^ GameObjectFid.GetHashCode() ^ FatherTransformFid.GetHashCode();
    }
}