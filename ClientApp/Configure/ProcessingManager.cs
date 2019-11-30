using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ClientApp.Configure.Interfaces;
using ClientApp.Configure.MessageSenders;

[assembly: InternalsVisibleTo("MessageQueueTests")]

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
        private readonly string _messageQueueServer;

        public ProcessingManager()
        {
            _messageQueueServer = GetMessageQueueName();
        }

        public bool ProcessingFileSendingMessage(string filePath, Stream stream)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new InvalidOperationException("file is null or empty");

            if (stream is null)
                throw new NullReferenceException("stream is null");

            var type  = stream.Length > byteMaxSizeForChunk ? MessageType.Multiple : MessageType.Single;
            var messageSender = GetMessageSender(type);

            SendMessage(messageSender, filePath, stream);

            return true;
        }

        public virtual IMessageSender GetMessageSender(MessageType type)
        {
            switch (type) {
                case MessageType.Single:
                    return new SingleMessageSender(_messageQueueServer);
                case MessageType.Multiple:
                    return new MultipleMessageSender(_messageQueueServer, byteMaxSizeForChunk);
                default:
                    return new SingleMessageSender(_messageQueueServer);
            }
        }

        public virtual void SendMessage(IMessageSender messageSender, string filePath, Stream stream)
        {
            messageSender.SendFile(filePath, stream);
        }

        private string GetMessageQueueName()
        {
            return ConfigurationManager.
                OpenExeConfiguration(Assembly.GetExecutingAssembly().Location)
                .AppSettings
                .Settings[AppSettingsMessageQueueServerName].Value;
        }
    }
}
