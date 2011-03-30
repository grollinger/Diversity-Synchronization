using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    public class TaxonList : IComparable<TaxonList>
    {
        public string DataSource { get; set; }
        public string DisplayText { get; set; }
        public string TaxonomicGroup { get; set; }        

        public int CompareTo(TaxonList other)
        {
            return this.DataSource.CompareTo(other.DataSource);
        }
    }
}
