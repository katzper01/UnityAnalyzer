using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnityAnalyzer.data;
using UnityAnalyzer.filesystem;
using UnityAnalyzer.service;

namespace UnityAnalyzer.Tests.service;

[TestClass]
[TestSubject(typeof(Parser))]
public class ParserTest
{
    private readonly Mock<IFileSystem> _mockFileSystem;

    private readonly Parser _parser;
    
    public ParserTest()
    {
        _mockFileSystem = new Mock<IFileSystem>();
        _parser = new Parser(_mockFileSystem.Object);
    }
    
    [TestInitialize()]
    public void Setup()
    {
        _mockFileSystem
            .Setup(fs => fs.GetFilesInDirectory(TestConstants.ProjectPath, "*.cs"))
            .Returns([TestConstants.ScriptPath1]);
        
        _mockFileSystem
            .Setup(fs => fs.ReadAllTextFromFile(TestConstants.ScriptPath1))
            .Returns(TestConstants.ScriptSourceCode1);
        
        _mockFileSystem
            .Setup(fs => fs.FileExists($"{TestConstants.ScriptPath1}.meta"))
            .Returns(true);

        _mockFileSystem
            .Setup(fs => fs.ReadAllTextFromFile($"{TestConstants.ScriptPath1}.meta"))
            .Returns(TestConstants.ScriptMeta1);
        
        _mockFileSystem
            .Setup(fs => fs.GetFilesInDirectory(TestConstants.ProjectPath, "*.unity"))
            .Returns([TestConstants.ScenePath1]);

        _mockFileSystem
            .Setup(fs => fs.ReadAllTextFromFile(TestConstants.ScenePath1))
            .Returns(TestConstants.SerializedScene1);
    }
    
    [TestMethod]
    public void ParseScripts_ShouldProperlyParseScriptsInProject()
    {
        // when
        var scripts = _parser.ParseScripts(TestConstants.ProjectPath);
        
        // then
        CollectionAssert.AreEquivalent(new List<Script> {
            new (TestConstants.RelativeScriptPath1, 
                [TestConstants.SerializedField1, TestConstants.SerializedField2],
                TestConstants.Guid1)
        }, scripts);
    }

    [TestMethod]
    public void ParseScripts_ScriptMetaFileDoesNotExist_ShouldThrowException()
    {
        // given
        _mockFileSystem
            .Setup(fs => fs.FileExists($"{TestConstants.ScriptPath1}.meta"))
            .Returns(false);
        
        // when
        var ex = Assert.ThrowsException<AggregateException>(() => _parser.ParseScripts(TestConstants.ProjectPath));
        
        // then
        Assert.AreEqual($"Script meta file not found for {TestConstants.ScriptPath1}.", ex.InnerExceptions[0].Message);
    }

    [TestMethod]
    public void ParseScripts_ScriptMetaFileDoesNotContainGuid_ShouldThrowException()
    {
        // given
        _mockFileSystem
            .Setup(fs => fs.ReadAllTextFromFile($"{TestConstants.ScriptPath1}.meta"))
            .Returns(TestConstants.InvalidScriptMeta1);
        
        // when
        var ex = Assert.ThrowsException<AggregateException>(() => _parser.ParseScripts(TestConstants.ProjectPath));
        
        // then
        Assert.AreEqual($"Found no guid in script meta file: {TestConstants.ScriptPath1}.meta.", ex.InnerExceptions[0].Message);
    }

    [TestMethod]
    public void ParseScenes_ShouldProperlyParseScenesInProject()
    {
        // when
        var scenes = _parser.ParseScenes(TestConstants.ProjectPath);
        
        // then
        CollectionAssert.AreEquivalent(new List<Scene> {
            new (TestConstants.SceneName1,
                [TestConstants.GameObject1, TestConstants.GameObject2],
                [TestConstants.MonoBehaviour1]
                )
        }, scenes);
    }

    [TestMethod]
    public void ParseScenes_InvalidSerializedScene_ShouldThrowException()
    {
        // given 
        _mockFileSystem
            .Setup(fs => fs.ReadAllTextFromFile(TestConstants.ScenePath1))
            .Returns(TestConstants.InvalidSerializedScene1);
        
        // when
        var ex = Assert.ThrowsException<AggregateException>(() => _parser.ParseScenes(TestConstants.ProjectPath));
        
        // then
        Assert.AreEqual($"Invalid unity scene: {TestConstants.ScenePath1}.", ex.InnerExceptions[0].Message);
    }
}