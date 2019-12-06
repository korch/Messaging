using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure.Interfaces
{
    public interface IProcessingManager
    {
        bool ProcessingFileSendingMessage(string fullFilePath, long fileSize);
    }
}
