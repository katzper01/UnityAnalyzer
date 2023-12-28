using System.Collections.Generic;
using UnityAnalyzer.data;

namespace UnityAnalyzer.Tests;

public static class TestConstants
{
    public const string ProjectPath = "/home/maciek/projectPath";
    public const string OutputPath = "/home/maciek/outputPath";

    public const string Path1 = "/a/b/c/d/e/f";
    public const string Path2 = "/g/h/i/j/k/l";
    public static string PATH_3 = "/m/n/o/p/q/r";
    public static string PATH_4 = "/s/t/u/w/x/y/z";

    public const string RelativeScriptPath1 = "path/to/ScriptA.cs";
    public const string RelativeScriptPath2 = "path/to/ScriptB.cs";
    public const string RelativeScriptPath3 = "path/to/ScriptC.cs";
    
    public const string ScriptPath1 = $"{ProjectPath}/{RelativeScriptPath1}";

    public const string SceneName1 = "Scene1";
    public const string ScenePath1 = $"{ProjectPath}/path/to/{SceneName1}.unity";

    public const string Guid1 = "guid0000000000001";
    public const string Guid2 = "guid0000000000002";
    public const string Guid3 = "guid0000000000003";
    public static string GUID_4 = "guid0000000000004";

    public const long Fid1 = 100000;
    public const long Fid2 = 200000;
    public const long Fid3 = 300000;
    public const long Fid4 = 400000;
    public const long Fid5 = 500000;
    public const long Fid6 = 600000;
    public const long LooseScriptFid = 11500000;

    public const string SerializedField1 = "Script1";
    public const string SerializedField2 = "Script2";

    public static readonly Script Script1 = new Script(Path1, [SerializedField1], Guid1);
    public static readonly Script Script2 = new Script(Path2, [SerializedField2], Guid2);

    public static readonly Transform Transform1 = new Transform(Fid3, Fid1, 0);
    public static readonly Transform Transform2 = new Transform(Fid4, Fid2, Fid3);

    public const string GameObjectName1 = "Parent";
    public const string GameObjectName2 = "Child";
        
    public static readonly GameObject GameObject1 = new GameObject(Fid1, GameObjectName1, Transform1);
    public static readonly GameObject GameObject2 = new GameObject(Fid2, GameObjectName2, Transform2);

    public static readonly GenericAsset GenericAsset1 = new GenericAsset(LooseScriptFid, Guid1);
    public static readonly GenericAsset GenericAsset2 = new GenericAsset(LooseScriptFid, Guid2);
    public static readonly GenericAsset GenericAsset3 = new GenericAsset(Fid3, Guid3);

    public static readonly Dictionary<string, GenericAsset> SerializedFieldsAssets1 = new()
    {
        { SerializedField1, GenericAsset2 }
    };

    public static readonly MonoBehaviour MonoBehaviour1 = new MonoBehaviour(GenericAsset1, SerializedFieldsAssets1);
    public static readonly MonoBehaviour MonoBehaviour2 = new MonoBehaviour(GenericAsset3, []);

    public static readonly Scene Scene1 = new Scene("scene1", [GameObject1], [MonoBehaviour1]);
    public static readonly Scene Scene2 = new Scene("scene2", [GameObject2], [MonoBehaviour2]);

    public const string ScriptSourceCode1 = 
        """
        using UnityEngine;

        public class ScriptA : MonoBehaviour
        {
            public ScriptB Script1;
            public Object Script2;
            void Start()
            {
                
            }
            void Update()
            {
                
            }
        }
        """;

    public const string ScriptMeta1 =
        $$"""
          fileFormatVersion: 2
          guid: {{Guid1}}
          MonoImporter:
            externalObjects: {}
          """;
    
    public const string InvalidScriptMeta1 = 
        """
        fileFormatVersion: 2
        MonoImporter:
          externalObjects: {}
        """;
    
    public static readonly string SerializedScene1 = 
        $$"""
        %YAML 1.1
        %TAG !u! tag:unity3d.com,2011:
        --- !u!1 &{{Fid1}}
        GameObject:
          m_Component:
          - component: {fileID: {{Fid3}}}
          m_Name: {{GameObjectName1}}
        --- !u!4 &{{Fid3}}
        Transform:
          m_GameObject: {fileID: {{Fid1}}}
          m_Children:
          - {fileID: {{Fid4}}}
          m_Father: {fileID: 0}
        --- !u!1 &{{Fid2}}
        GameObject:
          m_Component:
          - component: {fileID: {{Fid4}}}
          m_Name: {{GameObjectName2}}
        --- !u!4 &{{Fid4}}
        Transform:
          m_GameObject: {fileID: {{Fid2}}}
          m_Children: []
          m_Father: {fileID: {{Fid3}}}
        --- !u!114 &{{Fid5}}
        MonoBehaviour:
          m_Script: {fileID: {{LooseScriptFid}}, guid: {{Guid1}}}
          Script1: {fileID: {{LooseScriptFid}}, guid: {{Guid2}}}
        """;

    public const string InvalidSerializedScene1 =
        """
        %YAML 1.1
        %TAG !u! tag:unity3d.com,2011:
        --- !u!1 &0001
        """;
}