using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class ShowProgress : GenericMessage<BackgroundOperation>
    {
        public ShowProgress(BackgroundOperation p)
            :base(p)
        {

        }

        public static implicit operator ShowProgress(BackgroundOperation p)
        {
            return new ShowProgress(p);
        }
    }
}
