using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    public class SearchSpecification
    {      
        public SearchSpecification(string titleID, Type objectType, IList<Restriction> restrictions)
        {
            this.TitleID = titleID;
            this.ObjectType = objectType;
            this.Restrictions = restrictions;
        }
        public string TitleID
        {
            get;
            private set;
        }
        public Type ObjectType
        {
            get;
            private set;
        }
        public IList<Restriction> Restrictions
        {
            get;
            private set;
        }

        
    }
}
