using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using ClientApp.Configure.Interfaces;

[assembly: InternalsVisibleTo("MessageQueueTests")]

namespace ClientApp.Configure
{
    internal class MsmqClientService : IService, IDisposable
    {
        private readonly IWatcher _watcher;
        private readonly IProcessingManager _manager;

        public MsmqClientService(IWatcher watcher, IProcessingManager manager)
        {
            _watcher = watcher;
            _manager = manager;
        }

        public void Run()
        {
            _watcher.OnCreated += OnChanged;
           _watcher.Start();
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            while(!IsFileReady(e.FullPath)) { }

            _manager.ProcessingFileSendingMessage(e.FullPath);

            Console.WriteLine($"File: {e.FullPath} which was {e.ChangeType} was sent to Server.");
        }

        private bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            } catch (Exception) {
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                _watcher.Dispose();
            }
        }

        ~MsmqClientService() {
            Dispose(false);
        }
    }
}
