using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using Microsoft.Practices.Unity;

namespace MVVMDiversity.DesignServices
{
    public class UserOptions : IUserOptionsService
    {
        IMessenger _msngr;
        [Dependency]
        public IMessenger MessengerInstance
        {
            get
            {
                return _msngr;
            }
            set
            {
                if (_msngr != value)
                {
                    _msngr = value;
                    if (_msngr != null)
                        _msngr.Register<SettingsRequest>(this,
                            (msg) =>
                            {
                                updateSubscribers();
                            });
                }
            }
        }

        public UserOptions()
        {
            _opt = new DiversityUserOptions()
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
                    ScreenHeight = 800,
                    ScreenWidth = 800,
                    TruncateDataItems = false
                };
        }

        private void updateSubscribers()
        {
            MessengerInstance.Send<Settings>(getOptions());
        }
        DiversityUserOptions _opt;
        public DiversityUserOptions getOptions()
        {
            return _opt;
        }

        public void setOptions(DiversityUserOptions o)
        {
            _opt = o;
            updateSubscribers();
        }
    }
}
