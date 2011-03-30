using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class TaxonLists : ITaxonListService
    {

        public IList<TaxonList> getAvailableTaxonLists()
        {
            return new List<TaxonList>()
            {
                new TaxonList {DataSource = "Taxa1", DisplayText = "Taxon List 1", TaxonomicGroup = "fungi"},
                new TaxonList {DataSource = "Taxa2", DisplayText = "Taxon List 2", TaxonomicGroup = "plants"},
                new TaxonList {DataSource = "Taxa3", DisplayText = "Taxon List 3", TaxonomicGroup = "fungi"},
            };
        }
    }
}
