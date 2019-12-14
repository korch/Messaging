using System.IO;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class DirectoryManager : IDirectory
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void Create(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
