//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

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
