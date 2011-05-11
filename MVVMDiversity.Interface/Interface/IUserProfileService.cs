using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IUserProfileService
    {
        void tryLoadProfile();
        event AsyncOperationFinishedHandler ProfileLoaded;

        int ProjectID { get; set; }
        string HomeDB { get; }
        string UserNr { get; }
    }
}
