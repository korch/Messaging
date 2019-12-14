using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class MessageToFilePartMapper : IMessageToFilePartMapper
    {
        private IServerOptions _options;

        public MessageToFilePartMapper(IServerOptions options)
        {
            _options = options;
        }

        public FilePart MatToFilePart(Message message)
        {
               return new FilePart(message);
        }
    }
}
