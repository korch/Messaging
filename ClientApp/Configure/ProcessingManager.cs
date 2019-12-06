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
        private const string AppSettingsFileSize = "SizeOfChunks";
        private long _byteMaxSizeForChunk;
        private string _messageQueueServer;

        public ProcessingManager()
        {
            ReadAppSettings();
        }

        public bool ProcessingFileSendingMessage(string fullFilePath, long fileSize)
        {
            if (string.IsNullOrEmpty(fullFilePath))
                throw new InvalidOperationException("file is null or empty");

       
            var type  = fileSize > _byteMaxSizeForChunk ? MessageType.Multiple : MessageType.Single;
            var messageSender = GetMessageSender(type);

            SendMessage(messageSender, fullFilePath);

            return true;
        }

        internal IMessageSender GetMessageSender(MessageType type)
        {
            switch (type) {
                case MessageType.Single:
                    return new SingleMessageSender(_messageQueueServer);
                case MessageType.Multiple:
                    return new MultipleMessageSender(_messageQueueServer, _byteMaxSizeForChunk);
                default:
                    return new SingleMessageSender(_messageQueueServer);
            }
        }

        public virtual void SendMessage(IMessageSender messageSender, string filePath)
        {
            messageSender.SendFile(filePath);
        }

        private void ReadAppSettings()
        {
            _messageQueueServer = ConfigurationManager.
                OpenExeConfiguration(Assembly.GetExecutingAssembly().Location)
                .AppSettings
                .Settings[AppSettingsMessageQueueServerName].Value;

            _byteMaxSizeForChunk = long.Parse(ConfigurationManager
                .OpenExeConfiguration(Assembly.GetExecutingAssembly().Location)
                .AppSettings
                .Settings[AppSettingsFileSize].Value);
        }
    }
}
