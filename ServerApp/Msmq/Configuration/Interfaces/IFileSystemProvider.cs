namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IFileSystemManager
    {
        void CreateDirectoryIfNotExists(string path);
    }
}
