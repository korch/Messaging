using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure.Interfaces
{
    public interface IWatcher : IDisposable
    {
        void SetMonitoringFolder(string path);
        void SetFileType(string type);
        void EnableWatcher(bool enable);
        bool SetCreateHandler(FileSystemEventHandler handler);
    }
}
