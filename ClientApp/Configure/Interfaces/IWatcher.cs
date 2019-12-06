using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure.Interfaces
{
    public interface IWatcher : IDisposable
    {
        void Start();
        event FileSystemEventHandler OnCreated;
    }
}
