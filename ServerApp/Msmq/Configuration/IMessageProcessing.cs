using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    public interface IMessageProcessing
    {
        void Processing(List<Message> messages);
    }
}
