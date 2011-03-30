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
