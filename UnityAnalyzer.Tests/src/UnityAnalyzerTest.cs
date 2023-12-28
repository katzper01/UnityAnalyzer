using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnityAnalyzer.data;
using UnityAnalyzer.filesystem;
using UnityAnalyzer.service;

namespace UnityAnalyzer.Tests;

[TestClass]
[TestSubject(typeof(UnityAnalyzer))]
public class UnityAnalyzerTest
{
    private readonly Mock<IFileSystem> _mockFileSystem;
    private readonly Mock<IParser> _mockParser;
    private readonly Mock<IAnalyzer> _mockAnalyzer;
    
    private readonly UnityAnalyzer _unityAnalyzer;
    
    public UnityAnalyzerTest()
    {
        _mockFileSystem = new Mock<IFileSystem>();
        _mockParser = new Mock<IParser>();
        _mockAnalyzer = new Mock<IAnalyzer>();
        _unityAnalyzer = new UnityAnalyzer(_mockFileSystem.Object, _mockParser.Object, _mockAnalyzer.Object);
    }

    [TestInitialize()]
    public void Setup()
    {
        _mockFileSystem
            .Setup(fs => fs.DirectoryExists(TestConstants.ProjectPath))
            .Returns(true);

        _mockFileSystem
            .Setup(fs => fs.DirectoryExists(TestConstants.OutputPath))
            .Returns(true);
    }
    
    [TestMethod]
    public void Start_ProjectRootDirectoryDoesNotExist_ShouldThrowException()
    {
        // given
        _mockFileSystem
            .Setup(fs => fs.DirectoryExists(TestConstants.ProjectPath))
            .Returns(false);
        
        // when
        var ex = Assert.ThrowsException<Exception>(
            () => _unityAnalyzer.Start(TestConstants.ProjectPath, TestConstants.OutputPath));
        
        // then
        Assert.AreEqual($"Specified unity project root directory: {TestConstants.ProjectPath} does not exist.", ex.Message);
    }

    [TestMethod]
    public void Start_OutputDirectoryDoesNotExist_ShouldThrowException()
    {
        // given
        _mockFileSystem
            .Setup(fs => fs.DirectoryExists(TestConstants.OutputPath))
            .Returns(false);
        
        // when
        var ex = Assert.ThrowsException<Exception>(
            () => _unityAnalyzer.Start(TestConstants.ProjectPath, TestConstants.OutputPath));
        
        // then
        Assert.AreEqual($"Specified output directory: {TestConstants.OutputPath} does not exist.", ex.Message);
    }

    [TestMethod]
    public void Start_CallsParserAndAnalyzerWithProperArguments()
    {
        // given 
        var mockScripts = new List<Script> { TestConstants.Script1, TestConstants.Script2 };
        var mockScenes = new List<Scene> { TestConstants.Scene1, TestConstants.Scene2 };

        _mockParser
            .Setup(p => p.ParseScripts(TestConstants.ProjectPath))
            .Returns(mockScripts);

        _mockParser
            .Setup(p => p.ParseScenes(TestConstants.ProjectPath))
            .Returns(mockScenes);
        
        // when
        _unityAnalyzer.Start(TestConstants.ProjectPath, TestConstants.OutputPath);
        
        // then
        _mockParser.Verify(p => p.ParseScripts(TestConstants.ProjectPath), Times.Once);
        _mockParser.Verify(p => p.ParseScripts(TestConstants.ProjectPath), Times.Once);
        _mockAnalyzer.Verify(a => a.PrintScenesHierarchy(mockScenes, TestConstants.OutputPath));
        _mockAnalyzer.Verify(a => a.PrintUnusedScripts(mockScenes, mockScripts, TestConstants.OutputPath));
    }
}