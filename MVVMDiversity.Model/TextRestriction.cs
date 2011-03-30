using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Model
{ 
    public class TextRestriction : Restriction
    {
        public TextRestriction(string titleID,string property, bool exactMatch)
            : base(titleID,property)
        {                
            ExactMatch = exactMatch;
        }
            
        public bool ExactMatch { get; private set; }    
           
        public string Value { get; set; }




        public override string Error
        {
            get { throw new NotImplementedException(); }
        }

        public override string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "Value" && IsEnabled &&
                    string.IsNullOrEmpty(Value))
                    result = "Validation_TextRestriction_SearchStringEmpty";

                return result;
            }
        }
    }
    
}
