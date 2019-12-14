using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class QueueCreateManager : IQueueCreateManager
    {
        private IQueueManager _queueManager;
        public QueueCreateManager(IQueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        public void CreateQueueIfNotExists(string name)
        {
            if (!_queueManager.Exists(name)) {
                _queueManager.Create(name);
            }
        }

        public bool QueueExists(string name)
        {
            return _queueManager.Exists(name);
        }
    }
}
