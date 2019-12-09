using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    public class SingleMessageFileProcessor : Processor
    {
        public override void Processing(List<Message> messages)
        {
            var message = messages.First();
            using (FileStream output = new FileStream($"{_path}{message.Label}", FileMode.Create)) {
                Read(message.BodyStream, output);
                output.Close();
            }
            message.Dispose();
        }
    }
}
