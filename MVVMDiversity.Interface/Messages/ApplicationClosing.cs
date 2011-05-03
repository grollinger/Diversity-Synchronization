using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.Messages
{
    public class ApplicationClosing : MessageBase
    {
        public ApplicationClosing(bool warningOnly)
        {
            WarningOnly = warningOnly;
        }
        public bool WarningOnly { get; private set; }
    }
}
