using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Interface;

namespace MVVMDiversity.Messages
{
    public class CustomDialog : GenericMessage<Dialog>
    {
        public CustomDialog(Dialog d)
            :base(d)
        {

        }

        public event Action OnClose;

        public static implicit operator CustomDialog(Dialog d)
        {
            return new CustomDialog(d);
        }
    }
}
