using System;
using System.IO;
using ClientApp.Configure.Interfaces;


namespace ClientApp.Configure
{
    public enum MessageType
    {
        Single = 1,
        Multiple = 2
    }

    public class ProcessingManager : IProcessingManager
    {
        private readonly IClientOptions _options;
        private readonly IMessageSenderFactory _messageSenderFactory;
        private readonly IMessageCreator _messageCreator;

        public ProcessingManager(IClientOptions options, IMessageSenderFactory messageSenderFactory, IMessageCreator messageCreator)
        {
            _options = options;
            _messageSenderFactory = messageSenderFactory;
            _messageCreator = messageCreator;
        }

        public bool ProcessingFileSendingMessage(string fullFilePath)
        {
            if (string.IsNullOrEmpty(fullFilePath))
                throw new InvalidOperationException("file is null or empty");


            var fileSize = new FileInfo(fullFilePath).Length;
            var type  = fileSize > _options.SizeOfChunks ? MessageType.Multiple : MessageType.Single;
            var messageSender = _messageSenderFactory.GetMessageSender(type, _messageCreator, _options);

            messageSender.SendFile(fullFilePath);

            return true;
        }
    }
}
