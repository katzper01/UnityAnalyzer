namespace UnityAnalyzer.data;

public class Transform(long fid, long gameObjectFid, long fatherTransformFid)
{
    public long Fid => fid;
    public long GameObjectFid => gameObjectFid;
    public long FatherTransformFid => fatherTransformFid;
}