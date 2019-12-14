namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IDirectory
    {
        bool Exists(string path);
        void Create(string path);
    }
}
