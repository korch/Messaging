using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class Watcher : IWatcher
    {
        private const string AppSettingsFileFilter = "WatcherFileType";
        private const string AppSettingsMonitoringFolder = "MonitoringFolder";

        private readonly FileSystemWatcher _watcher;

        public event FileSystemEventHandler OnCreated;

        public bool IsRunning => _watcher.EnableRaisingEvents;
        
        public Watcher()
        {
            _watcher = new FileSystemWatcher();
        

            ReadAppSettings();
            SetupDefaultNotifiedFilters();
        }


        public void Start()
        {
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;
        }

        private void SetupDefaultNotifiedFilters()
        {
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName;
        }

        private void ReadAppSettings()
        {
            _watcher.Filter = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings[AppSettingsFileFilter].Value;

            var folder = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings[AppSettingsMonitoringFolder].Value;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _watcher.Path = folder;
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
