using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class QueueManager : IQueueManager
    {
        public bool Exists(string name)
        {
            return MessageQueue.Exists(name);
        }

        public void Create(string name)
        {
            MessageQueue.Create(name);
        }
    }
}
