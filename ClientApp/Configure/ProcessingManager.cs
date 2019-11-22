using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClientApp.Configure.MessageSenders;

namespace ClientApp.Configure
{

    public enum MessageType
    {
        Single = 1,
        Multiple = 2
    }

    public class ProcessingManager
    {
        private const long byteMaxSizeForChunk = 3000000;

        private MessageType _messageType;
        private IMessageSender _messageSender;

        private string _queueName;

        public ProcessingManager(string queueName)
        {
            _queueName = queueName;
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
                    _messageSender = new SingleMessageMessageSender(_queueName);
                    break;
                case MessageType.Multiple:
                    _messageSender = new MultipleMessageMessageSender(_queueName);
                    break;
            }
        }


    }
}
