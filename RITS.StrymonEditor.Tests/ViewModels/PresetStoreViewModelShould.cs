﻿using System;
using System.Collections.Generic;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{


    [TestClass]
    public class PresetStoreViewModelShould : TestContext<PresetStoreViewModel>
    {

        [TestMethod]
        public void CorrectlySetDownloadMode()
        {
            // Arrange
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            Container.Register<bool>(false);
            onlineMock.Setup(x => x.GetAvailableTagNames()).Returns(new List<string> { "Song" });
            Sut.OnlineService = onlineMock.Object;
            // Assert
            Assert.IsTrue(Sut.IsDownloadMode);
            Assert.AreEqual(1, Sut.AvailableTags.Count);
            onlineMock.Verify(x => x.GetAvailableTagNames(), Times.Once());
        }

        [TestMethod]
        public void CorrectlySetUploadMode()
        {
            // Arrange
            Container.Register(TestHelper.TestTimelinePreset);// Force the container to use different constructor
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            onlineMock.Setup(x => x.GetAvailableTagNames()).Returns(new List<string> { "Song" });
            Sut.OnlineService = onlineMock.Object;
            // Assert 
            Assert.IsTrue(Sut.IsUploadMode);
            Assert.AreEqual(1, Sut.AvailableTags.Count);
            onlineMock.Verify(x => x.GetAvailableTagNames(), Times.Once());
        }

        [TestMethod]
        public void CorrectlyDownload()
        {
            // Arrange
            var xmlPreset =  TestHelper.TestTimelinePreset.ToXmlPreset();
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            onlineMock.Setup(x => x.DownloadPreset(It.IsAny<int>())).Returns(xmlPreset);
            Container.Register<bool>(false);
            Sut.OnlineService = onlineMock.Object;
            Sut.Close = new Action(() => { });
            Sut.SelectedPreset=new PresetMetadata();
            // Act
            Sut.DownloadCommand.Execute(null);
            // Assert
            onlineMock.Verify(x=>x.DownloadPreset(It.IsAny<int>()),Times.Once());
        }

        [TestMethod]
        public void CorrectlySearch()
        {
            // Arrange
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            Container.Register<bool>(false);
            Sut.OnlineService = onlineMock.Object;
            onlineMock.Setup(x => x.Search(It.IsAny<PresetSearch>())).Returns(new List<PresetMetadata>());
            // Act
            Sut.SearchCommand.Execute(null);
            // Assert
            onlineMock.Verify(x => x.Search(It.IsAny<PresetSearch>()), Times.Once());
        }

        [TestMethod]
        public void CorrectlyUpload()
        {
            // Arrange
            Container.Register(TestHelper.TestTimelinePreset);// Force the container to use different constructor
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            Sut.MessageDialog = Container.GetMock<IMessageDialog>().Object;
            Sut.OnlineService = onlineMock.Object;
            Sut.Close = new Action(() => { });
            // Act
            Sut.UploadCommand.Execute(null);
            // Assert
            onlineMock.Verify(x => x.UploadPreset(It.IsAny<StrymonXmlPreset>(), It.IsAny<List<Tag>>()), Times.Once());
        }

        [TestMethod]
        public void CorrectlyAddCustomTag()
        {
            // Arrange
            var onlineMock = Container.GetMock<IOnlinePresetService>();
            Container.Register<bool>(false);
            onlineMock.Setup(x => x.GetAvailableTagNames()).Returns(new List<string> { "Song" });
            onlineMock.Setup(x => x.GetExistingValuesForTag("Song")).Returns(new List<string> { "Song 1", "Song 2" });
            Sut.OnlineService = onlineMock.Object;
            Sut.TagToAdd = "Song";
            // Act
            Sut.AddTagCommand.Execute(null);
            // Assert
            onlineMock.Verify(x => x.GetExistingValuesForTag("Song"), Times.Once());
            Assert.AreEqual(0, Sut.AvailableTags.Count);
            Assert.AreEqual(1, Sut.CustomTags.Count);
            Assert.AreEqual(2, Sut.CustomTags[0].AvailableValues.Count);
        }

    }
}
