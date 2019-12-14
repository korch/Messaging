using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Experimental.System.Messaging;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class SingleMessageFileProcessor : Processor
    {
        public SingleMessageFileProcessor(IServerOptions options) : base(options) { }

        public override void Processing(List<FilePart> messages)
        {
            var message = messages.First();
            using (FileStream output = new FileStream($"{Options.FolderToCopy}{message.Part.Label}", FileMode.Create)) {
                Read(message.Part.BodyStream, output);
                output.Close();
            }
            message.Part.Dispose();
        }
    }
}
