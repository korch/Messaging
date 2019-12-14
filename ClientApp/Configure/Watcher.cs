using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class Watcher : IWatcher
    {
        private readonly FileSystemWatcher _watcher;
        private IClientOptions _options;

        public event FileSystemEventHandler OnCreated;

        public bool IsRunning => _watcher.EnableRaisingEvents;
        
        public Watcher(IClientOptions options)
        {
            _options = options;
            _watcher = new FileSystemWatcher();

            SetupDefaultNotifiedFilters();
        }


        public void Start()
        {
            _watcher.Created += OnCreated;
            _watcher.Filter = _options.WatcherFileType;
            _watcher.Path = _options.MonitoringFolder;
            _watcher.EnableRaisingEvents = true;
        }

        private void SetupDefaultNotifiedFilters()
        {
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    _watcher.Dispose();
                }
                disposed = true;
            } 
        }

        ~Watcher()
        {
            Dispose(false);
        }
    }
}
