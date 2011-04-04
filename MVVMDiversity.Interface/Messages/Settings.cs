using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class Settings : GenericMessage<DiversityUserOptions>
    {
        public Settings(DiversityUserOptions newOptions)
            : base (newOptions)
        {

        }

        public static implicit operator Settings(DiversityUserOptions o)
        {
            return new Settings(o);
        }
    }
}
