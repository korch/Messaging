using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.MessageSenders
{
    public class SingleMessageMessageSender : IMessageSender
    {
        private readonly string _queueName;

        public SingleMessageMessageSender(string messageQueuePath)
        {
            _queueName = messageQueuePath;
        }

        /// <summary>
        /// Process single message. When we shouldn't split a byte array for chunks
        /// </summary>
        /// <param name="fullPath"></param>
        public void SendFile(string path, Stream stream)
        {
            using (var serverQueue = new MessageQueue(_queueName, QueueAccessMode.Send)) {
                var message = new Message {
                    BodyStream = stream,
                    Label = Path.GetFileName(path),
                    Priority = MessagePriority.Normal,
                    Formatter = new BinaryMessageFormatter(),
                    AppSpecific = 100
                };

                serverQueue.Send(message);
                stream.Close();
            }
        }
    }
}
