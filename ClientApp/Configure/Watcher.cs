using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class Watcher : IWatcher
    {
        private FileSystemWatcher _watcher;

        public string MonitoringFolder => _watcher.Path;
        public string Filter => _watcher.Filter;
        public bool IsRunning => _watcher.EnableRaisingEvents;
        
        public Watcher()
        {
            _watcher = new FileSystemWatcher();
            SetupDefaultNotifiedFilters();
        }

        /// <summary>
        /// Set the file type for searching file. for example: '*.pdf' to search only pdf files.
        /// </summary>
        /// <param name="type"></param>
        public void SetFileType(string type)
        {
            _watcher.Filter = type;
        }

        public bool SetCreateHandler(FileSystemEventHandler handler)
        {
            if (handler is null)
               throw new NullReferenceException("parameter is null");
                
            
            _watcher.Created += handler;

            return true;
        }

        public void SetMonitoringFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("parameter is null or empty");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            _watcher.Path = path;
        }

        public void EnableWatcher(bool enable)
        {
            _watcher.EnableRaisingEvents = enable;
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher.Dispose();
            }
        }

        ~Watcher()
        {
            Dispose(false);
        }
    }
}
