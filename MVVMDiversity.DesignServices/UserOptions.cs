using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class UserOptions : IUserOptionsService
    {               

        public DiversityUserOptions getOptions()
        {
            return new DiversityUserOptions()
                {
                    CurrentConnection = new ConnectionProfile()
                    {
                        InitialCatalog = "Init",
                        IPAddress = "127.0.0.1",
                        Port = "1234",
                        Name = "Test",
                        TaxonNamesInitialCatalog = "TaxaInit"
                    },
                    Paths = new DBPaths(){
                        MobileDB = "Mob.sdf",
                        MobileTaxa = "Tax.sdf",
                    },
                    PasswordVisible = true,
                    UseSqlAuthentification = false,
                    Username = "Tester",
                    UseDeviceDimensions = true,                    
                };
        }

        public void setOptions(DiversityUserOptions o)
        {
            
        }
    }
}
