using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.Messages
{
    public class ExecuteAction : GenericMessage<Enums.Action>
    {
        public ExecuteAction(Enums.Action a)
            :base(a)

        {

        }

        public static implicit operator ExecuteAction(Enums.Action a)
        {
            return new ExecuteAction(a);
        }
    }
}
