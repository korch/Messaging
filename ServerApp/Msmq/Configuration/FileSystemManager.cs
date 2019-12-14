using System;
using System.Collections.Generic;
using System.Text;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class FileSystemManager : IFileSystemManager
    {
        private IDirectory _directory;
        public FileSystemManager(IDirectory directory)
        {
            _directory = directory;
        }

        public void CreateDirectoryIfNotExists(string path)
        {
            if (!_directory.Exists(path))
            {
                _directory.Create(path);
            }
        }
    }
}
