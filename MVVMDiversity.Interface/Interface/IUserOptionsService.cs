using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IUserOptionsService
    {
        DiversityUserOptions getOptions();
        void setOptions(DiversityUserOptions o);                       
    }
}
