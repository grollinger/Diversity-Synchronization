using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.DesignServices
{
    public class Profile : IUserProfileService
    {

        public int ProjectID
        {
            get
            {
                return 1;
            }
            set
            {
                
            }
        }

        public string HomeDB
        {
            get { return "Init"; }
        }


        public string UserNr
        {
            get { return "1234"; }
        }
    }
}
