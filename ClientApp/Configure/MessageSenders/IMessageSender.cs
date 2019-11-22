﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientApp.Configure.MessageSenders
{
    public interface IMessageSender
    {
        void SendFile(string path, Stream stream);
    }
}