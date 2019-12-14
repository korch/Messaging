using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    public class FilePart
    {
        public Message Part { get; }

        public FilePart(Message part)
        {
            Part = part;
        }
    }
}
