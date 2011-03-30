using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class ConnectionStateChanged : GalaSoft.MvvmLight.Messaging.GenericMessage<ConnectionState>
    {
        public ConnectionStateChanged(ConnectionState newValue)
            : base(newValue)
        {

        }
    }
}
