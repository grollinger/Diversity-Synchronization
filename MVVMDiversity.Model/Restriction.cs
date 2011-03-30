using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MVVMDiversity.Model
{
    public abstract class Restriction : IDataErrorInfo
    {
        public string TitleID { get; private set; }
        public string Property { get; private set; }
        public bool IsEnabled { get; set; }

        public Restriction(string titleID, string property)
        {
            TitleID = titleID;
            Property = property;
        }

        public abstract string Error{ get; }


        public abstract string this[string columnName] { get; }
    }
}
