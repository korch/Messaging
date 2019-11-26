using System.Configuration;
using System.IO;
using ClientApp.Configure.Interfaces;
using ClientApp.Configure.MessageSenders;

namespace ClientApp.Configure
{

    public enum MessageType
    {
        Single = 1,
        Multiple = 2
    }

    public class ProcessingManager : IProcessingManager
    {
        private const string AppSettingsMessageQueueServerName = "MessageQueueServerName";
        private const long byteMaxSizeForChunk = 3000000;

        private MessageType _messageType;
        private IMessageSender _messageSender;

        private string _messageQueueServer;

        public ProcessingManager()
        {
            _messageQueueServer = ConfigurationManager.AppSettings[AppSettingsMessageQueueServerName];
        }

        public void ProcessingFileSendingMessage(string filePath, Stream stream)
        {
            _messageType = stream.Length > byteMaxSizeForChunk ? MessageType.Multiple : MessageType.Single;

            GetMessageSender(_messageType);
            _messageSender.SendFile(filePath, stream);
        }

        private void GetMessageSender(MessageType type)
        {
            switch (type)
            {
                case MessageType.Single:
                    _messageSender = new SingleMessageMessageSender(_messageQueueServer);
                    break;
                case MessageType.Multiple:
                    _messageSender = new MultipleMessageMessageSender(_messageQueueServer, byteMaxSizeForChunk);
                    break;
            }
        }
    }
}
