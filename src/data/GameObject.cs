namespace UnityAnalyzer.data;

public class GameObject(long fid, string name, Transform transform)
{
    public long Fid => fid;
    public string Name => name;
    public Transform Transform => transform;
}