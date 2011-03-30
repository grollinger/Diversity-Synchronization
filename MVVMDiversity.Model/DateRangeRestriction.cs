using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    public class DateRangeRestriction : Restriction
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateRangeRestriction(string titleID, string property)
            :base(titleID,property)
        {
            StartDate = EndDate = DateTime.Now;
        }

        public override string Error
        {
            get { throw new NotImplementedException(); }
        }

        public override string this[string columnName]
        {
            get
            {
                string result = null;

                if ((columnName == "StartDate" || columnName == "EndDate") &&
                    StartDate > EndDate)
                    result = "Validation_DateRangeRestriction_EndBeforeStart";                  

                return result;
            }
        }
    }
}
