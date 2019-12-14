using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class MsmqServiceFactory : IServerFactory
    {
        private IServerOptionsProvider _optionsProvider;
        private IFileSystemManager _fileManager;
        private IQueueCreateManager _queueManager;
        private IMessageToFilePartMapper _messageToFilePartMapper;
        private IFileMessageBuffer _buffer;
        private IFileCopyManager _copyManager;

        public MsmqServiceFactory(IServerOptionsProvider optionsProvider, IFileSystemManager fileManager, IQueueCreateManager queueManager, IMessageToFilePartMapper messageToFilePartMapper, 
            IFileMessageBuffer buffer, IFileCopyManager copyManager)
        {
            _optionsProvider = optionsProvider;
            _fileManager = fileManager;
            _queueManager = queueManager;
            _messageToFilePartMapper = messageToFilePartMapper;
            _buffer = buffer;
            _copyManager = copyManager;
        }

        public IServer GetServer()
        {
            return new MsmqServer(_optionsProvider.GetOptions(), _fileManager, _queueManager, _messageToFilePartMapper, _buffer, _copyManager);
        }
    }
}
