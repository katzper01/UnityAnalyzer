namespace UnityAnalyzer.data;

public class GameObject(long fid, string name, Transform transform)
{
    public long Fid => fid;
    public string Name => name;
    public Transform Transform => transform;
    
    private bool Equals(GameObject other)
    {
        return Fid.Equals(other.Fid)
               && Name.Equals(other.Name)
               && Transform.Equals(other.Transform);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((GameObject)obj);
    }

    public override int GetHashCode()
    {
        return Fid.GetHashCode() ^ Name.GetHashCode() ^ Transform.GetHashCode();
    }
}