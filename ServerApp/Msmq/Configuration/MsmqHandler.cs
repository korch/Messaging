using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class MsmqHandler : IHandler
    {
        public MsmqHandler()
        { }
        public void Handle(Message message)
        {

        }
    }
}
