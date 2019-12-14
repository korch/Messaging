using System;
using System.IO;
using ClientApp.Configure.Interfaces;
using Experimental.System.Messaging;

namespace ClientApp.Configure
{
    public class MessageCreator : IMessageCreator
    {
        public Message CreateMessage(string label, int appSpecific, byte[] buf = null)
        {
            return new Message {
                BodyStream = new MemoryStream(buf),
                Label = label,
                Priority = MessagePriority.Normal,
                Formatter = new BinaryMessageFormatter(),
                AppSpecific = appSpecific
            };
        }
    }
}
