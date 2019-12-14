using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServerApp.Msmq.Configuration.Interfaces;

namespace ServerApp.Msmq.Configuration
{
    public class MultipleMessageFileProcessor : Processor
    {
        public MultipleMessageFileProcessor(IServerOptions options) : base(options) { }

        public override void Processing(List<FilePart> messages)
        { 
            if (!messages.Any()) 
                return;

            var list = messages.Where(m => m.Part.AppSpecific != -1);
            var fileName = list.FirstOrDefault().Part.Label;

            using (FileStream output = new FileStream($"{Options.FolderToCopy}{fileName}", FileMode.Create)) {
                foreach (var item in list) {
                    Read(item.Part.BodyStream, output);
                    item.Part.Dispose();
                }
                output.Close();
            }
        }
    }
}
