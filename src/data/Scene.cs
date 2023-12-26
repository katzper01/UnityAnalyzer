namespace UnityAnalyzer.data;

public class Scene(string name, List<GameObject> gameObjects, List<MonoBehaviour> monoBehaviors)
{
    public string Name => name;
    public List<GameObject> GameObjects => gameObjects;
    public List<MonoBehaviour> MonoBehaviors => monoBehaviors;
}