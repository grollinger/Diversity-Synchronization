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
        public bool TruncateDataItems { get; private set; }
        public Selection(ICollection<IISOViewModel> selection, bool truncate)
            : base(selection)
        {
            TruncateDataItems = truncate;
        }        
    }
}
