using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Experimental.System.Messaging;

namespace ServerApp.Msmq.Configuration
{
    public abstract class Processor
    {
        private const string AppSettingsFolderToCopy = "FolderToCopy";
        protected readonly string _path;

        protected Processor()
        {
            _path = ConfigurationManager.AppSettings[AppSettingsFolderToCopy];
        }

        protected void Read(Stream stream, FileStream output)
        {
            int readBytes;
            byte[] buffer;
            buffer = new byte[stream.Length];
            while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, readBytes);
            }
        }

        public abstract void Processing(List<Message> messages);
    }
}
