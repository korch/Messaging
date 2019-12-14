using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Msmq.Configuration.Interfaces
{
    public interface IFileCopyManager
    {
        Task CopyFiles(IFileMessageBuffer list);
    }
}
