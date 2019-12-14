namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IQueueCreateManager
    {
        void CreateQueueIfNotExists(string name);
        bool QueueExists(string name);
    }
}
