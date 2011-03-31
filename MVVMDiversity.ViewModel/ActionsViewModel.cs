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

using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MVVMDiversity.Messages;
using MVVMDiversity.Model;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using log4net;
using System;
using GalaSoft.MvvmLight.Threading;

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
        private ILog _Log = LogManager.GetLogger(typeof(ActionsViewModel));

        private const ConnectionState DEFINITIONS = ConnectionState.ConnectedToMobileTax | ConnectionState.ConnectedToRepTax;
        private const ConnectionState REPOSITORY = ConnectionState.ConnectedToMobile | ConnectionState.ConnectedToRepository;

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

        [Dependency]
        public IDefinitionsService DefinitionsSvc
        {
            get;
            set;
        }

        [Dependency]
        public IFieldDataService FieldDataSvc { get; set; }

        [Dependency]
        public IUserProfileService ProfileSvc { get; set; }

        [Dependency]
        public ISessionManager SessionMgr { get; set; }

        [Dependency]
        public IUserOptionsService Settings { get; set; }

        protected override bool CanNavigateBack
        {
            get { return true; }
        }

        protected override bool CanNavigateNext
        {
            get { return true; }
        }

        private static readonly string GetTaxonDefinitionsPropertyName = "GetTaxonDefinitions";
        public ICommand GetTaxonDefinitions { get; private set; }       

        public ICommand GetPropertyNames { get; private set; }

        public ICommand GetPrimaryData { get; private set; }

        public ICommand UploadData { get; private set; }

        public ICommand CleanDatabase { get; private set; }

        public ICommand OpenMaps { get; private set; }

        #region SyncState Properties
        /// <summary>
        /// The <see cref="AreTaxaDownloaded" /> property's name.
        /// </summary>
        public const string AreTaxaDownloadedPropertyName = "AreTaxaDownloaded";

        private bool _taxDLed = false;

        /// <summary>
        /// Gets the AreTaxaDownloaded property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool AreTaxaDownloaded
        {
            get
            {
                return _taxDLed;
            }

            set
            {
                if (_taxDLed == value)
                {
                    return;
                }

                var oldValue = _taxDLed;
                _taxDLed = value;                

                // Verify Property Exists
                VerifyPropertyName(AreTaxaDownloadedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(AreTaxaDownloadedPropertyName);               
            }
        }

        /// <summary>
        /// The <see cref="IsFieldDataLoaded" /> property's name.
        /// </summary>
        public const string IsFieldDataLoadedPropertyName = "IsFieldDataLoaded";

        private bool _fdLoaded = false;

        /// <summary>
        /// Gets the IsFieldDataLoaded property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsFieldDataLoaded
        {
            get
            {
                return _fdLoaded;
            }

            set
            {
                if (_fdLoaded == value)
                {
                    return;
                }

                var oldValue = _fdLoaded;
                _fdLoaded = value;


                // Verify Property Exists
                VerifyPropertyName(IsFieldDataLoadedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsFieldDataLoadedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ArePropertiesLoaded" /> property's name.
        /// </summary>
        public const string ArePropertiesLoadedPropertyName = "ArePropertiesLoaded";

        private bool _propsLoaded = false;

        /// <summary>
        /// Gets the ArePropertiesLoaded property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool ArePropertiesLoaded
        {
            get
            {
                return _propsLoaded;
            }

            set
            {
                if (_propsLoaded == value)
                {
                    return;
                }

                var oldValue = _propsLoaded;
                _propsLoaded = value;

                

                // Verify Property Exists
                VerifyPropertyName(ArePropertiesLoadedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(ArePropertiesLoadedPropertyName);

              
            }
        }

        /// <summary>
        /// The <see cref="IsDataUploaded" /> property's name.
        /// </summary>
        public const string IsDataUploadedPropertyName = "IsDataUploaded";

        private bool _dataUploaded = false;

        /// <summary>
        /// Gets the IsDataUploaded property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsDataUploaded
        {
            get
            {
                return _dataUploaded;
            }

            set
            {
                if (_dataUploaded == value)
                {
                    return;
                }

                var oldValue = _dataUploaded;
                _dataUploaded = value;               

                // Verify Property Exists
                VerifyPropertyName(IsDataUploadedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsDataUploadedPropertyName);
             
            }
        }

        /// <summary>
        /// The <see cref="IsDBCleaned" /> property's name.
        /// </summary>
        public const string IsDBCleanedPropertyName = "IsDBCleaned";

        private bool _dbCleaned = false;

        /// <summary>
        /// Gets the IsDBCleaned property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsDBCleaned
        {
            get
            {
                return _dbCleaned;
            }

            set
            {
                if (_dbCleaned == value)
                {
                    return;
                }

                var oldValue = _dbCleaned;
                _dbCleaned = value;

                // Verify Property Exists
                VerifyPropertyName(IsDBCleanedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsDBCleanedPropertyName);
            }
        }
        #endregion



        private ConnectionState _cState = ConnectionState.None;
        private SyncState _sState = SyncState.None;
        /// <summary>
        /// Initializes a new instance of the ActionsViewModel class.
        /// </summary>
        public ActionsViewModel()
            : base("Actions_NextText","Actions_PreviousText","Actions_Title", "Actions_Description")
        {
            NextPage = Page.Actions;
            PreviousPage = Page.ProjectSelection;
            updateFromSyncState();

            MessengerInstance.Register<ConnectionStateChanged>(this,
                (msg) =>
                {
                    _cState = msg.Content;
                });

            MessengerInstance.Register<SyncStateChanged>(this,
                (msg) =>
                {
                    _sState = msg.Content;

                    updateFromSyncState();
                });


            GetTaxonDefinitions = new RelayCommand(
                () =>
                {
                    MessengerInstance.Send<CustomDialog>(Dialog.Taxon);
                },
                () =>
                {                    
                    return requiredConnectionLevel(DEFINITIONS);
                });

            GetPropertyNames = new RelayCommand(
                () =>
                {
                    if (DefinitionsSvc != null)
                    {
                        var progress = DefinitionsSvc.loadProperties(
                            () =>
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(
                                    () =>
                                    {
                                        MessengerInstance.Send<HideProgress>(new HideProgress());
                                        MessengerInstance.Send<SyncStepFinished>(SyncState.PropertyNamesDownloaded);
                                    }
                                    );
                                
                            });
                        MessengerInstance.Send<ShowProgress>(progress);
                    }
                    else
                        _Log.Error("DefinitionsService N/A");
                },
                () =>
                {
                    return requiredConnectionLevel(DEFINITIONS);
                });

            GetPrimaryData = new RelayCommand(
                () =>
                {
                    MessengerInstance.Send<NavigateToPage>(Page.FieldData);
                },
                () =>
                {
                    return requiredConnectionLevel(REPOSITORY);
                });

            UploadData = new RelayCommand(
                () =>
                {
                    if (FieldDataSvc != null)
                    {
                        if (ProfileSvc != null)
                        {
                            var projectID = ProfileSvc.ProjectID;
                            var userNo = ProfileSvc.UserNr;

                            var progress = FieldDataSvc.uploadData(userNo, projectID, 
                                ()=>
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            MessengerInstance.Send<SyncStepFinished>(SyncState.FieldDataUploaded);
                                            MessengerInstance.Send<HideProgress>(new HideProgress());
                                        });
                                });

                            MessengerInstance.Send<ShowProgress>(progress);
                            
                        }
                        else
                            _Log.Error("UserProfileService N/A");
                    }
                    else
                        _Log.Error("FieldDataService N/A");
                },
                () =>
                {
                    return requiredConnectionLevel(REPOSITORY);
                });

            CleanDatabase = new RelayCommand(
                () =>
                {
                    if (SessionMgr != null)
                    {
                        if (CM != null)
                        {
                            if (ProfileSvc != null)
                            {
                                if (Settings != null)
                                {
                                    var process = BackgroundOperation.newUninterruptable();
                                    process.IsProgressIndeterminate = true;
                                    process.ProgressDescriptionID = "Actions_Cleaning_DB";
                                    MessengerInstance.Send<ShowProgress>(process);

                                    new Action(() =>
                                    {
                                        var project = ProfileSvc.ProjectID;

                                        var paths = Settings.getOptions().Paths;


                                        if (CM.truncateSyncTable())
                                        {
                                            CM.disconnectFromMobileDB();
                                            var workingPaths = SessionMgr.createCleanWorkingCopies(paths);
                                            CM.connectToMobileDB(workingPaths);
                                        }
                                        else
                                            _Log.Info("Could not truncate Sync Table, aborting clean.");


                                        if (ProfileSvc.ProjectID != project)
                                            ProfileSvc.ProjectID = project;

                                        
                                    }).BeginInvoke((res) =>
                                        {
                                            DispatcherHelper.CheckBeginInvokeOnUI(() => 
                                                {
                                                    MessengerInstance.Send<HideProgress>(new HideProgress());
                                                    MessengerInstance.Send<NavigateToPage>(Page.Connections);                                                        
                                                });
                                        }, null);



                                }
                                else
                                    _Log.Error("Settings N/A");
                            }
                            else
                                _Log.Error("ProfileService N/A");
                        }
                        else
                            _Log.Error("ConnectionManager N/A");
                    }
                    else
                        _Log.Error("Session Manager N/A");
                },
                () =>
                {
                    return requiredConnectionLevel(ConnectionState.ConnectedToSynchronization | ConnectionState.ConnectedToMobileTax | ConnectionState.ConnectedToMobile);
                });
            OpenMaps = new RelayCommand(
                () =>
                {
                    MessengerInstance.Send<NavigateToPage>(Page.Map);
                });
        }

        private void updateFromSyncState()
        {
            IsDataUploaded = syncStateSatisfies(SyncState.FieldDataUploaded);
            //IsDBCleaned
            ArePropertiesLoaded = syncStateSatisfies(SyncState.PropertyNamesDownloaded);
            AreTaxaDownloaded = syncStateSatisfies(SyncState.TaxaDownloaded);
            IsFieldDataLoaded = syncStateSatisfies(SyncState.FieldDataDownloaded);
        }

        private bool requiredConnectionLevel(ConnectionState req)
        {
            return (_cState & req) == req;
        }

        private bool syncStateSatisfies(SyncState req)
        {
            return (_sState & req) == req;
        }
    }
}