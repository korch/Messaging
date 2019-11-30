using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.MessageSenders
{
    public class SingleMessageSender : IMessageSender
    {
        private readonly string _queueName;

        public SingleMessageSender(string messageQueuePath)
        {
            _queueName = messageQueuePath;
        }

        /// <summary>
        /// Process single message. When we shouldn't split a byte array for chunks
        /// </summary>
        /// <param name="fullPath"></param>
        public bool SendFile(string path, Stream stream)
        {
            using (var serverQueue = new MessageQueue(_queueName, QueueAccessMode.Send)) {
                try {
                    var message = new Message
                    {
                        BodyStream = stream,
                        Label = Path.GetFileName(path),
                        Priority = MessagePriority.Normal,
                        Formatter = new BinaryMessageFormatter(),
                        AppSpecific = 100
                    };

                    serverQueue.Send(message);
                    stream.Close();
                } catch (Exception e) {
                    throw new InvalidOperationException(e.Message);
                }
            }
            return true;
        }
    }
}
