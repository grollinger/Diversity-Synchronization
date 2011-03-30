using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Interface
{
    public interface IDialogViewModel
    {
        ICloseableView AssociatedView { set; }
    }
}
