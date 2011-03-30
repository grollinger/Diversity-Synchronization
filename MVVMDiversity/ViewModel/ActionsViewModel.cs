using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MVVMDiversity.Messages;
using MVVMDiversity.Model;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;

namespace MVVMDiversity.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ActionsViewModel : PageViewModel
    {
        private const ConnectionState DEFINITIONS = ConnectionState.ConnectedToMobileTax & ConnectionState.ConnectedToRepTax;
        private const ConnectionState REPOSITORY = ConnectionState.ConnectedToMobile & ConnectionState.ConnectedToRepository;

        IConnectionManagementService _cm;
        [Dependency]
        public IConnectionManagementService CM 
        {
            get
            {
                return _cm;
            }
            set
            {
                if (value != null)
                {
                    _cm = value;
                    _cState = _cm.State;
                }
            }
        }


        protected override bool CanNavigateBack
        {
            get { return true; }
        }

        protected override bool CanNavigateNext
        {
            get { return true; }
        }

        public ICommand GetTaxonDefinitions { get; private set; }       

        public ICommand GetPropertyNames { get; private set; }

        public ICommand GetPrimaryData { get; private set; }

        public ICommand UploadData { get; private set; }

        public ICommand CleanDatabase { get; set; }


        private ConnectionState _cState = ConnectionState.None;
        /// <summary>
        /// Initializes a new instance of the ActionsViewModel class.
        /// </summary>
        public ActionsViewModel()
            : base("Actions_NextText","Actions_PreviousText","Actions_Title", "Actions_Description")
        {
            MessengerInstance.Register<ConnectionStateChanged>(this,
                (msg) =>
                {
                    _cState = msg.Content;
                });

            MessengerInstance.Register<SyncStateChanged>(this,
                (msg) =>
                {

                });


            GetTaxonDefinitions = new RelayCommand(
                () =>
                {
                    MessengerInstance.Send<CustomDialogMessage>(Dialog.Taxon);
                },
                () =>
                {                    
                    return requiredConnectionLevel(DEFINITIONS);
                });

            GetPropertyNames = new RelayCommand(
                () =>
                {
                },
                () =>
                {
                    return requiredConnectionLevel(DEFINITIONS);
                });

            GetPrimaryData = new RelayCommand(
                () =>
                {
                },
                () =>
                {
                    return requiredConnectionLevel(REPOSITORY);
                });

            UploadData = new RelayCommand(
                () =>
                {
                },
                () =>
                {
                    return requiredConnectionLevel(REPOSITORY);
                });

            CleanDatabase = new RelayCommand(
                () =>
                {
                },
                () =>
                {
                    return requiredConnectionLevel(ConnectionState.ConnectedToSynchronization & ConnectionState.ConnectedToMobileTax & ConnectionState.ConnectedToMobile);
                });
        }

        private bool requiredConnectionLevel(ConnectionState req)
        {
            return (_cState & req) == req;
        }

    }
}