using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class ShowProgress : GenericMessage<AsyncOperationInstance>
    {
        public ShowProgress(AsyncOperationInstance p)
            :base(p)
        {

        }

        public static implicit operator ShowProgress(AsyncOperationInstance p)
        {
            return new ShowProgress(p);
        }
    }
}
