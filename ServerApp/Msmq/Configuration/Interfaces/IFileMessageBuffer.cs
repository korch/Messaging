using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IFileMessageBuffer
    {
        void AddFilePart(FilePart part);
        List<ReadyToCopyFiles> GetReadyFiles();
        void RemoveFile(string fileIdentifier);
    }
}
