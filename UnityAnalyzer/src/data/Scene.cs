namespace UnityAnalyzer.data;

public class Scene(string name, List<GameObject> gameObjects, List<MonoBehaviour> monoBehaviours)
{
    public string Name => name;
    public List<GameObject> GameObjects => gameObjects;
    public List<MonoBehaviour> MonoBehaviours => monoBehaviours;
    
    private bool Equals(Scene other)
    {
        return Name.Equals(other.Name)
               && GameObjects.Count == other.GameObjects.Count
               && !GameObjects.Except(other.GameObjects).Any()
               && MonoBehaviours.Count == other.MonoBehaviours.Count
               && !MonoBehaviours.Except(other.MonoBehaviours).Any();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Scene)obj);
    }

    public override int GetHashCode()
    {
        var hash = Name.GetHashCode();
        hash = GameObjects.Aggregate(hash, (current, gameObject) => current ^ gameObject.GetHashCode());
        return MonoBehaviours.Aggregate(hash, (current, monoBehaviour) => current ^ monoBehaviour.GetHashCode());
    }
}