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
using System.ComponentModel;

namespace MVVMDiversity.Model
{
    public class MapInfo : IDataErrorInfo
    {
        public String Name
        {
            set;
            get;
        }
        public String Description
        {
            set;
            get;
        }
        public float SWLat
        {
            set;
            get;
        }
        public float SWLong
        {
            set;
            get;
        }
        public float SELat
        {
            set;
            get;
        }
        public float SELong
        {
            set;
            get;
        }
        public float NELat
        {
            set;
            get;
        }
        public float NELong
        {
            set;
            get;
        }

        public int ZoomLevel
        {
            set;
            get;
        }

        #region IDataErrorInfo Member

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                string res = null;

                if (columnName == "Name")
                {
                    if (Name != null && Name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) > -1)
                        res = "Name must not contain characters forbidden in file names!";
                    else if (Name == null || Name == "")
                        res = "Name field may not be empty";
                }
                return res;
            }
        }

        #endregion
    }
}
