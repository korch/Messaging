using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IMessageToFilePartMapper
    {
        FilePart MatToFilePart(Message message);
    }
}
