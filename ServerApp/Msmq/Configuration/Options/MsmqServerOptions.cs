using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration.Options
{
    public class MsmqServerOptions : IServerOptions
    {
        public string MessageQueueName { get; }
        public string FolderToCopy { get; }
        public int SingleMessageIdentificator { get; }
        public int MultipleMessageStartIdentificator { get; }
        public int MultipleMessageEndIdentificator { get; }
        public int MultipleCommonMessageIdentificator { get; }

        public MsmqServerOptions()
        {
            MessageQueueName = ConfigurationManager.AppSettings["MessageQueueName"];
            FolderToCopy = ConfigurationManager.AppSettings["FolderToCopy"];
            SingleMessageIdentificator = int.Parse(ConfigurationManager.AppSettings["SingleMessageIdentificator"]);
            MultipleMessageStartIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleMessageStartIdentificator"]);
            MultipleMessageEndIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleMessageEndIdentificator"]);
            MultipleCommonMessageIdentificator = int.Parse(ConfigurationManager.AppSettings["MultipleCommonMessageIdentificator"]);
        }
    }
}
