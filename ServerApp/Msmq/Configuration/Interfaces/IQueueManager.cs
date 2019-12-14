namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IQueueManager
    {
        bool Exists(string name);
        void Create(string name);
    }
}
