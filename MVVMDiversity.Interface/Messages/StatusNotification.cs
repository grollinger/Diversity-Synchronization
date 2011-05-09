using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.Messages
{
    public class StatusNotification : GenericMessage<string>
    {
        public StatusNotification(string msg)
            : base(msg)
        {

        }

        public static implicit operator StatusNotification(string msg)
        {
            return new StatusNotification(msg);
        }
    }
}
