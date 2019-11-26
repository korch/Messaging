using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure.Interfaces
{
    public interface IWatcher
    {
        void SetMonitoringFolder(string path);
        void SetFileType(string type);
        void SetCreateHandler(FileSystemEventHandler handler);
        void EnableWatcher(bool enable);
    }
}
