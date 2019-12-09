using System;
using System.Collections.Generic;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    internal enum FileTransferPullState
    {
        InProgress = 1,
        ReadyToTransfer
    }

    internal class FileTransferPull
    {
        public string ClientQueueId { get; set; }
        public string FileName { get; set; }
        public FileTransferPullState State { get; set; }
        public MessageType Type { get; set; }

        public bool IsCopied { get; set; }
        public List<Message> Messages = new List<Message>();
      
    }
}
