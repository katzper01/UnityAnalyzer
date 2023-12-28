using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnityAnalyzer.data;
using UnityAnalyzer.filesystem;
using UnityAnalyzer.service;

namespace UnityAnalyzer.Tests.service;

[TestClass]
[TestSubject(typeof(Analyzer))]
public class AnalyzerTest
{
    private readonly Mock<IFileSystem> _mockFileSystem;

    private readonly Analyzer _analyzer;
    
    public AnalyzerTest()
    {
        _mockFileSystem = new Mock<IFileSystem>();
        _analyzer = new Analyzer(_mockFileSystem.Object);
    }
    
    [TestMethod]
    public void PrintScenesHierarchy_ShouldProperlyPrintScenesHierarchy()
    {
        // given 
        var mockScenes = new List<Scene>
        {
            new(TestConstants.SceneName1,
                [TestConstants.GameObject1, TestConstants.GameObject2],
                [TestConstants.MonoBehaviour1]
            )
        };

        // when 
        _analyzer.PrintScenesHierarchy(mockScenes, TestConstants.OutputPath);
        
        // then 
        _mockFileSystem.Verify(
            fs => fs.WriteAllTextToFile(
                $"{TestConstants.OutputPath}/{TestConstants.SceneName1}.unity.dump",
                "Parent\n--Child\n"));
    }

    [TestMethod]
    public void PrintUnusedScripts_ShouldProperlyPrintUnusedScripts()
    {
        // given 
        var mockScenes = new List<Scene>
        {
            new(TestConstants.SceneName1,
                [TestConstants.GameObject1, TestConstants.GameObject2],
                [TestConstants.MonoBehaviour1]
            )
        };
        
        var mockScripts = new List<Script>
        {
            new(TestConstants.RelativeScriptPath1, // used, instantiated as MonoBehaviour
                [TestConstants.SerializedField1, TestConstants.SerializedField2],
                TestConstants.Guid1),
            new(TestConstants.RelativeScriptPath2, // used, but not instantiated as MonoBehaviour
                [],
                TestConstants.Guid2),
            new(TestConstants.RelativeScriptPath3, // unused
                [],
                TestConstants.Guid3)               
        };
        
        // when
        _analyzer.PrintUnusedScripts(mockScenes, mockScripts, TestConstants.OutputPath);
        
        // then
        _mockFileSystem.Verify(
            fs => fs.WriteAllTextToFile(
                $"{TestConstants.OutputPath}/UnusedScripts.csv",
                $"Relative Path,GUID\n{TestConstants.RelativeScriptPath3},{TestConstants.Guid3}\n"));
    }
}