using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.Messages
{
    public class SelectedTaxonLists : GenericMessage<IList<TaxonList>>
    {       
        public SelectedTaxonLists (IList<TaxonList> content)
            : base(content)
	    {

	    } 
       
    }
}
