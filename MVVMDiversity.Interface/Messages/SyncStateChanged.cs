using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class SyncStateChanged : GenericMessage<SyncState>
    {
        public SyncStateChanged(SyncState s)
            : base(s)
        {

        }

        public static implicit operator SyncStateChanged(SyncState s)
        {
            return new SyncStateChanged(s);
        }
    }
}
