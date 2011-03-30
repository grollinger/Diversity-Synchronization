using GalaSoft.MvvmLight;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using MVVMDiversity.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

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
    public class OptionsViewModel : ViewModelBase
    {
        private const int NO_SELECTION = -1;


        IConnectionProfilesService _cps = null;
        [Dependency]
        public IConnectionProfilesService CPService 
        {
            get
            {
                return _cps;
            }
            set
            {
                _cps = value;
                updateProfiles();
            }
        }        

        IUserOptionsService _uos = null;
        [Dependency]
        public IUserOptionsService UserOptions
        {
            get
            {
                return _uos;
            }
            set
            {
                _uos = value;
                updateProfiles();
            }
        }

        [Dependency]
        public ICloseableView AssociatedView { get; set; }

        /// <summary>
        /// The <see cref="SaveOptions" /> property's name.
        /// </summary>
        public const string SaveOptionsPropertyName = "SaveOptions";

        private ICommand _saveOptions = null;

        /// <summary>
        /// Gets the SaveOptions property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ICommand SaveOptions
        {
            get
            {
                return _saveOptions;
            }

            set
            {
                if (_saveOptions == value)
                {
                    return;
                }

                var oldValue = _saveOptions;
                _saveOptions = value;


                // Verify Property Exists
                VerifyPropertyName(SaveOptionsPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SaveOptionsPropertyName);               
            }
        }

        #region Properties

        /// <summary>
        /// The <see cref="Options" /> property's name.
        /// </summary>
        public const string OptionsPropertyName = "Options";

        private DiversityUserOptions _options = null;

        /// <summary>
        /// Gets the Options property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DiversityUserOptions Options
        {
            get
            {
                return _options;
            }

            set
            {
                if (_options == value)
                {
                    return;
                }

                var oldValue = _options;
                _options = value;
               
                // Verify Property Exists
                VerifyPropertyName(OptionsPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(OptionsPropertyName);               
            }
        }

        /// <summary>
        /// The <see cref="ConnectionProfiles" /> property's name.
        /// </summary>
        public const string ConnectionProfilesPropertyName = "ConnectionProfiles";

        private IList<ConnectionProfile> _CPs = null;

        /// <summary>
        /// Gets the ConnectionProfiles property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IList<ConnectionProfile> ConnectionProfiles
        {
            get
            {
                return _CPs;
            }

            set
            {
                if (_saveOptions == value)
                {
                    return;
                }
                
                _CPs = value;                

                // Verify Property Exists
                VerifyPropertyName(ConnectionProfilesPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(ConnectionProfilesPropertyName);                
            }
        }

        /// <summary>
        /// The <see cref="SelectedProfile" /> property's name.
        /// </summary>
        public const string SelectedProfilePropertyName = "SelectedProfile";

        private int _selectedP = 1;

        /// <summary>
        /// Gets the SelectedProfile property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int SelectedProfile
        {
            get
            {
                return _selectedP;
            }

            set
            {
                if (_selectedP == value)
                {
                    return;
                }

                var oldValue = _selectedP;
                _selectedP = value;
                       
                // Verify Property Exists
                VerifyPropertyName(SelectedProfilePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedProfilePropertyName);              
            }
        }

        private ConnectionProfile SelectedCP
        {
            get
            {
                if (ConnectionProfiles != null)
                {
                    try
                    {
                        return ConnectionProfiles[SelectedProfile];
                    }
                    catch { }
                }
                return null;
            }
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the OptionsViewModel class.
        /// </summary>
        public OptionsViewModel()
        {
            SaveOptions = new RelayCommand(
                () =>
                {
                    if (UserOptions != null && Options != null)
                    {
                        if (SelectedCP != null)
                        {
                            Options.CurrentConnection = SelectedCP;
                            UserOptions.setOptions(Options);
                        }                        
                    }
                },
                () =>
                {
                    return Options.Error != null;
                });
        }

        private void updateProfiles()
        {
            if (CPService != null)
            {
                ConnectionProfiles = CPService.getAvailableProfiles();
                SelectedProfile = 1;
            }
            
            ConnectionProfile currentProfile = null;
            if (UserOptions != null)
            {
                var currentOptions = UserOptions.getOptions();
                if (currentOptions != null)
                {
                    currentProfile = currentOptions.CurrentConnection;
                    Options = currentOptions;
                }
                else
                    Options = new DiversityUserOptions();
            }

            if (ConnectionProfiles != null && currentProfile != null && ConnectionProfiles.Contains(currentProfile))
            {
                SelectedProfile = ConnectionProfiles.IndexOf(currentProfile);
            }
        }
    }
}