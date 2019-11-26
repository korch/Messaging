using ClientApp.Configure;
using System;
using System.IO;
using System.Threading;
using ClientApp.Configure.Interfaces;

namespace ClientApp
{
    public class MsmqClientService : IService, IDisposable
    {
        private IWatcher _watcher;
        private readonly IProcessingManager _manager;

        public MsmqClientService(IWatcher watcher, IProcessingManager manager)
        {
            _watcher = watcher;
            _manager = manager;
        }

        public void Run()
        {
            _watcher.SetFileType("*.pdf");
            _watcher.SetCreateHandler(OnChanged);
            _watcher.EnableWatcher(true);
        }

        public void SetMonitoringFolder(string path)
        {
            _watcher.SetMonitoringFolder(path);
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(3000);
            var fileStream = new FileStream(e.FullPath, FileMode.Open);

            _manager.ProcessingFileSendingMessage(e.FullPath, fileStream);

            fileStream.Dispose();

            Console.WriteLine($"File: {e.FullPath} which was {e.ChangeType} was sent to Server.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                _watcher = null;
            }
        }

        ~MsmqClientService() {
            Dispose(false);
        }
    }
}
