using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure
{
    internal class Watcher
    {
        private const string DefaultPath = "C:\\DefaultFolder\\";

        private FileSystemWatcher _watcher;
        private string _path;

        public Watcher()
        {
            _watcher = new FileSystemWatcher();
          
            SetMonitoringPath();
            SetupWatcher();
        }

        private void SetMonitoringPath()
        {
            Console.WriteLine("Do you wanna set specific folder path for monitoring files ? Y/N");
            var answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                Console.WriteLine("Please, write here a new path:");
                _path = Console.ReadLine();
                _watcher.Path = _path;
                Console.WriteLine("A new path was changed");
            }
            else
            {
                Console.WriteLine($"You using a default path:{DefaultPath}");
                if (!Directory.Exists(DefaultPath))
                    Directory.CreateDirectory(DefaultPath);

                _watcher.Path = DefaultPath;
            }
        }

        private void SetupWatcher()
        {
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // only pdf files
            _watcher.Filter = "*.pdf";

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
        }

        public void SetCreateHandler(FileSystemEventHandler handler)
        {
            _watcher.Created += handler;
        }
    }
}
