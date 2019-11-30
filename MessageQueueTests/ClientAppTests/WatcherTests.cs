using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Autofac;
using ClientApp.Configure;
using ClientApp.Configure.Interfaces;
using NUnit.Framework;

namespace MessageQueueTests.ClientAppTests
{
    [TestFixture]
    public class WatcherTests
    {
        private IWatcher _watcher;
        private string _monitoringFolder = "C://MonitoringFolder";

        [SetUp]
        public void Setup()
        {
            var container = CompositionRoot.Get();

            _watcher = container.Resolve<IWatcher>();
        }

        [Test]
        public void SetFileTypeTest()
        {
            var type = "*.pdf";

            _watcher.SetFileType(type);
            Assert.AreEqual(type, ((Watcher)_watcher).Filter);
        }

        [Test]
        public void SetCreateHandlerTest()
        {
            var result = _watcher.SetCreateHandler(OnChanged);

            Assert.IsTrue(result);
        }

        [Test]
        public void SetCreateHandlerExceptionTest()
        {
            Assert.Throws<NullReferenceException>(
                () => _watcher.SetCreateHandler(null));
        }

        [Test]
        public void SetMonitoringFolderTest()
        {
            _watcher.SetMonitoringFolder(_monitoringFolder);

            Assert.AreEqual(_monitoringFolder, ((Watcher)_watcher).MonitoringFolder);
            Assert.IsTrue(Directory.Exists(_monitoringFolder));
        }

        [Test]
        public void SetMonitoringFolderExceptionTest()
        {
            Assert.Throws<InvalidOperationException>(
                () => _watcher.SetMonitoringFolder(null));
            Assert.Throws<InvalidOperationException>(
                () => _watcher.SetMonitoringFolder(""));
        }

        [Test]
        public void EnableWatcherTest()
        {
            _watcher.SetMonitoringFolder(_monitoringFolder);
            _watcher.EnableWatcher(true);

            Assert.IsTrue(((Watcher)_watcher).IsRunning);
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // ? :o 
        }

        [TearDown]
        public void Down()
        {
            if (Directory.Exists(_monitoringFolder))
                Directory.Delete(_monitoringFolder);
        }
    }
}
