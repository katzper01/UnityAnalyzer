namespace UnityAnalyzer.data;

public class MonoBehaviour(GenericAsset script, Dictionary<string, GenericAsset> serializedFieldsAssets)
{
    public GenericAsset Script => script;
    public Dictionary<string, GenericAsset> SerializedFieldsAssets => serializedFieldsAssets;
}