using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Model;

namespace MVVMDiversity.Messages
{
    public class SettingsChanged : GenericMessage<DiversityUserOptions>
    {
        public SettingsChanged(DiversityUserOptions newOptions)
            : base (newOptions)
        {

        }

        public static implicit operator SettingsChanged(DiversityUserOptions o)
        {
            return new SettingsChanged(o);
        }
    }
}
