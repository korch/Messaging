using System.IO;
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

        [TearDown]
        public void Down()
        {
            if (Directory.Exists(_monitoringFolder))
                Directory.Delete(_monitoringFolder);
        }
    }
}
