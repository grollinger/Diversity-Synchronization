using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Interface;

namespace MVVMDiversity.Messages
{
    public enum Page
    {
        Connections,
        ProjectSelection,
        Actions,
        FieldData,
        FinalSelection,
        Map,
    }

    public class NavigateToPage : GenericMessage<Page>
    {
        public NavigateToPage(Page page)
            : base(page)
        {

        }

        public static implicit operator NavigateToPage(Page p)
        {
            return new NavigateToPage(p);
        }
    }
}
