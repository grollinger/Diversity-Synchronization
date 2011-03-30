using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.Messages
{
    public class SyncStepFinished : GenericMessage<SyncState>
    {
        public SyncStepFinished(SyncState step)
            :base(step)
        {


        }

        public static implicit operator SyncStepFinished(SyncState s)
        {
            return new SyncStepFinished(s);
        }
    }
}
