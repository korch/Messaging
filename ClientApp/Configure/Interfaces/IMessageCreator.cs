using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;

namespace ClientApp.Configure.Interfaces
{
    public interface IMessageCreator
    {
        Message CreateMessage(string label, int appSpecific, byte[] array);
    }
}
