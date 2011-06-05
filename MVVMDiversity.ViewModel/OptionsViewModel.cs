//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

using GalaSoft.MvvmLight;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using MVVMDiversity.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Linq;
using MVVMDiversity.Messages;
using MVVMDiversity.Enums;
using GalaSoft.MvvmLight.Messaging;


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
        
        /// <summary>
        /// 
        /// </summary>
        public ICommand SaveOptions
        {
            get;
            private set;
        }

        #region Properties

        /// <summary>
        /// The <see cref="Options" /> property's name.
        /// </summary>
        public const string OptionsPropertyName = "Options";

        private DiversityUserOptions _options = null;

        private void relayOptionsChanged(object sender, PropertyChangedEventArgs args)
        {
            if (sender == Options)
            {
                VerifyPropertyName(OptionsPropertyName);
                RaisePropertyChanged(OptionsPropertyName);
            }
        }

        /// <summary>
        /// 
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

                
                if (_options != null)
                    _options.PropertyChanged -= relayOptionsChanged;

                _options = value;

                if (_options != null)
                    _options.PropertyChanged += relayOptionsChanged;
               
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
        /// Available connection profiles
        /// </summary>
        public IList<ConnectionProfile> ConnectionProfiles
        {
            get
            {
                return _CPs;
            }

            set
            {
                if (_CPs == value)
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

        private int _selectedP = 0;

        /// <summary>
        /// 
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
        public OptionsViewModel(IMessenger msngr)
        {
            MessengerInstance = msngr;
            SaveOptions = new RelayCommand(
                () =>
                {
                    if (UserOptions != null && Options != null)
                    {
                        if (SelectedCP != null)
                        {
                            if (Options.CurrentConnection != SelectedCP)
                            {
                                Options.CurrentConnection = SelectedCP;
                                MessengerInstance.Send<ExecuteAction>(Action.DisconnectRepositories);
                                MessengerInstance.Send<NavigateToPage>(Page.Connections);
                            }
                            
                        }
                        UserOptions.setOptions(Options);
                    }
                },
                () =>
                {
                    return Options.Error == null;
                }); 
            
        }

        private void updateProfiles()
        {
            if (CPService != null)
            {
                ConnectionProfiles = CPService.getAvailableProfiles();
                SelectedProfile = 0;
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

            if (ConnectionProfiles != null && currentProfile != null )
            {
                var selectedIndices = from profile in ConnectionProfiles
                                    where profile.CompareTo(currentProfile) == 0
                                    select ConnectionProfiles.IndexOf(profile);

                if (selectedIndices.Count() > 0)
                    SelectedProfile = selectedIndices.First();
            }
        }
    }
}