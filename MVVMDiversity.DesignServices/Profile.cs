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
        int homedbIdx = 0;
        string[] homeDBs = new string[2] { "Init", "NotInit" };
        public string HomeDB
        {
            get { return homeDBs[homedbIdx = ++homedbIdx % 2]; }
        }


        public string UserNr
        {
            get { return "1234"; }
        }
    }
}
