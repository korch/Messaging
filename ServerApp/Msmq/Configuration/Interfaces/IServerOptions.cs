using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IServerOptions
    {
        string MessageQueueName { get; }
        string FolderToCopy { get; }
        int SingleMessageIdentificator { get; }
        int MultipleMessageStartIdentificator { get; }
        int MultipleMessageEndIdentificator { get; }
        int MultipleCommonMessageIdentificator { get; }
    }
}
