using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class Watcher : FileSystemWatcher, IWatcher
    {

        public Watcher()
        {
            SetupDefaultNotifiedFilters();
        }

        /// <summary>
        /// Set the file type for searching file. for example: '*.pdf' to search only pdf files.
        /// </summary>
        /// <param name="type"></param>
        public void SetFileType(string type)
        {
            this.Filter = type;
        }

        public void SetCreateHandler(FileSystemEventHandler handler)
        {
            this.Created += handler;
        }

        public void SetMonitoringFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            this.Path = path;
        }

        public void EnableWatcher(bool enable)
        {
            this.EnableRaisingEvents = enable;
        }

        private void SetupDefaultNotifiedFilters()
        {
            this.NotifyFilter = NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName;
        }
    }
}
