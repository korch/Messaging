namespace ClientApp.Configure.Interfaces
{
    public interface IClientOptions
    {
        string MessageQueueServerName { get; }
        string MonitoringFolder { get; }
        string WatcherFileType { get; }
        int SizeOfChunks { get; }
    }
}
