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
        public void EnableWatcherTest()
        {
           _watcher.Start();

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
