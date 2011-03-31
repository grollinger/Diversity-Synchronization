using System.Windows;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Interface;
using MVVMDiversity.Services;
using Microsoft.Practices.Unity;
using MVVMDiversity.ViewModel;

namespace MVVMDiversity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();       
        }

        public static ViewModelLocator Locator
        {
            get
            {
                var ioc = new UnityContainer();

                if (true)
                {
                    ioc.RegisterInstance<IMessenger>(Messenger.Default);
                    ioc.RegisterInstance<IUserOptionsService>(new DesignServices.UserOptions());
                    ioc.RegisterInstance<IConnectionProfilesService>(new DesignServices.ConnectionProfiles());
                    var conn = new DesignServices.Connections();
                    ioc.RegisterInstance<IConnectionManagementService>(conn);
                    ioc.RegisterInstance<IConnectionProvider>(conn);
                    ioc.RegisterInstance<IProjectService>(new DesignServices.Projects());
                    ioc.RegisterInstance<IUserProfileService>(new DesignServices.Profile());

                    ioc.RegisterInstance<IDefinitionsService>(new DesignServices.Definitions());
                    ioc.RegisterInstance<IFieldDataService>(new DesignServices.FieldData());

                    ioc.RegisterInstance<ISessionManager>(new DesignServices.Sessions());

                    ioc.RegisterInstance<ITaxonListService>(new DesignServices.TaxonLists());

                }
                else
                {
                    ioc.RegisterInstance<IMessenger>(Messenger.Default);
                    ioc.RegisterInstance<IUserOptionsService>(ioc.Resolve<UserOptionsService>());
                    ioc.RegisterInstance<IConnectionProfilesService>(ioc.Resolve<ConnectionProfileProvider>());
                    var connections = ioc.Resolve<ConnectionManager>();
                    ioc.RegisterInstance<IConnectionManagementService>(connections);
                    ioc.RegisterInstance<IConnectionProvider>(connections);
                    ioc.RegisterInstance<IProjectService>(ioc.Resolve<ProjectProvider>());
                    ioc.RegisterInstance<IUserProfileService>(ioc.Resolve<UserProfileProvider>());
                    ioc.RegisterInstance<IDefinitionsService>(ioc.Resolve<DefinitionsService>());
                    ioc.RegisterInstance<IFieldDataService>(ioc.Resolve<FieldDataService>());
                    ioc.RegisterInstance<ITaxonListService>(ioc.Resolve<TaxonListService>());
                    ioc.RegisterInstance<ISessionManager>(ioc.Resolve<SessionManager>());

                }
                return new ViewModelLocator(ioc);
            }
        }
    }
}
