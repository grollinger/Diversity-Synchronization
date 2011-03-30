using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    public enum SessionState
    {
        Uninitialized,
        New,
        Cleaned,
        Dirty
    }
}
