using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ClientApp.Configure.Interfaces;

namespace ClientApp.Configure
{
    public class MsmqClientOptions : IClientOptions
    {
        public string MessageQueueServerName { get; }
        public string MonitoringFolder { get; }
        public string WatcherFileType { get; }
        public int SizeOfChunks { get; }

        public MsmqClientOptions()
        {
            MessageQueueServerName = ConfigurationManager.AppSettings["MessageQueueServerName"];
            MonitoringFolder = ConfigurationManager.AppSettings["MonitoringFolder"];
            WatcherFileType = ConfigurationManager.AppSettings["WatcherFileType"];
            SizeOfChunks = int.Parse(ConfigurationManager.AppSettings["SizeOfChunks"]);
        }
    }
}
