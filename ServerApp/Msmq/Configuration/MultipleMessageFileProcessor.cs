using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    public class MultipleMessageFileProcessor : Processor
    {
        public override void Processing(List<Message> messages)
        { 
            if (!messages.Any()) 
                return;

            var list = messages.Where(m => m.AppSpecific != -1);
            var fileName = list.FirstOrDefault().Label;

            using (FileStream output = new FileStream($"{_path}{fileName}", FileMode.Create)) {
                foreach (var item in list) {
                    Read(item.BodyStream, output);
                    item.Dispose();
                }
                output.Close();
            }
        }
    }
}
