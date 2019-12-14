using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IServerFactory
    {
        IServer GetServer();
    }
}
