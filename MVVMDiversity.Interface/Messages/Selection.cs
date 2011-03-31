using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Interface;
using System.Collections.ObjectModel;

namespace MVVMDiversity.Messages
{
    public class Selection : GenericMessage<ICollection<IISOViewModel>>
    {
        public Selection(ICollection<IISOViewModel> selection)
            : base(selection)
        {
        }

        public static implicit operator Selection(Collection<IISOViewModel> sel)
        {
            return new Selection(sel);
        }
    }
}
