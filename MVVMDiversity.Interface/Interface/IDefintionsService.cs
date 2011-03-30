using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MVVMDiversity.Interface
{
    public interface IDefintionsService : INotifyPropertyChanged
    {
        int ProgressPercentage { get; }
        
    }
}
