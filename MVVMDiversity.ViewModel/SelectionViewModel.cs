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

using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using log4net;
using MVVMDiversity.Messages;
using MVVMDiversity.Model;
using GalaSoft.MvvmLight.Threading;
using System;

namespace MVVMDiversity.ViewModel
{
    public class SelectionViewModel : PageViewModel
    {
        private ILog _Log = LogManager.GetLogger(typeof(SelectionViewModel));      

        [Dependency]
        public IFieldDataService FDSvc
        {
            get;
            set;
        } 
	

        [Dependency]
        public IISOViewModelStore ISOStore { get; set; }


        private IList<ISerializableObject> _completeSelection;

        public SelectionViewModel()
            : base("Selection_Next", "Selection_Previous", "Selection_Title","")
        {
            PreviousPage = Messages.Page.FieldData;
            NextPage = Messages.Page.Actions;
            CanNavigateNext = false;
            CanNavigateBack = false;
            
            MessengerInstance.Register<Selection>(this, (msg) =>
            {
                CanNavigateNext = false;
                CanNavigateBack = false;
                BuildingSelection = false;
                BuildStatus = "Selection_WaitingForTree";
                SelectionTree = new AsyncTreeViewModel(ISOStore);
                SelectionTree.TruncateDataItems = msg.TruncateDataItems;
                foreach (var vm in msg.Content)
                {
                    SelectionTree.addGenerator(vm);
                }



                SelectionTree.SelectionBuildProgressChanged += new SelectionBuildProgressChangedHandler(SelectionTree_SelectionBuildProgressChanged);     

                new Action(() =>
                    {
                        _completeSelection = SelectionTree.buildSelection();
                        DispatcherHelper.CheckBeginInvokeOnUI(()=>
                            {
                                SelectionTree.SelectionBuildProgressChanged -= SelectionTree_SelectionBuildProgressChanged;
                                CanNavigateBack = true;
                                CanNavigateNext = true;
                                BuildingSelection = false;
                                BuildStatus = "Selection_Done";
                            });
                    }).BeginInvoke(null, null);
            });
        }

        void SelectionTree_SelectionBuildProgressChanged(int rootCount, int rootsDone, IISOViewModel currentVM)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {                    
                    RootCount = rootCount;
                    CurrentRootNo = rootsDone;
                    BuildProgress = (rootCount > 0) ? 100*rootsDone / RootCount : 100;
                    BuildingSelection = true;
                });

        }        
        
        protected override void OnNavigateNext()
        {
            if (FDSvc != null)
            {
                if (SelectionTree != null)
                {                    
                    IsBusy = true;
                    FDSvc.DownloadFinished += new AsyncOperationFinishedHandler(FDSvc_DownloadFinished);
                    CurrentOperation = FDSvc.downloadData(_completeSelection);                   
                }
            }
            else
                _Log.Error("FieldDataService N/A");            
            
        }

        void FDSvc_DownloadFinished(AsyncOperationInstance operation)
        {
            if (FDSvc != null)
                FDSvc.DownloadFinished -= FDSvc_DownloadFinished;

            CurrentOperation = null;

            if (operation.State == OperationState.Succeeded)
            {
                MessengerInstance.Send<SyncStepFinished>(SyncState.FieldDataDownloaded);
            }
            else
                showError(operation);

            base.OnNavigateNext();
        }        

        /// <summary>
        /// The <see cref="SelectionTree" /> property's name.
        /// </summary>
        public const string SelectionTreePropertyName = "SelectionTree";

        private ITreeViewModel _selTree = null;

        /// <summary>
        /// 
        /// </summary>
        public ITreeViewModel SelectionTree
        {
            get
            {
                return _selTree;
            }

            set
            {
                if (_selTree == value)
                {
                    return;
                }
                
                _selTree = value;              

                // Verify Property Exists
                VerifyPropertyName(SelectionTreePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectionTreePropertyName);                            
            }
        }

        /// <summary>
        /// The <see cref="BuildProgress" /> property's name.
        /// </summary>
        public const string BuildProgressPropertyName = "BuildProgress";

        private int _buildProgress = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BuildProgress
        {
            get
            {
                return _buildProgress;
            }

            set
            {
                if (_buildProgress == value)
                {
                    return;
                }

                var oldValue = _buildProgress;
                _buildProgress = value;                

                // Verify Property Exists
                VerifyPropertyName(BuildProgressPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(BuildProgressPropertyName);                
            }
        }

        /// <summary>
        /// The <see cref="RootCount" /> property's name.
        /// </summary>
        public const string RootCountPropertyName = "RootCount";

        private int _rootCount = 0;

        /// <summary>
        /// How Many root ISOs are part of the Selection
        /// </summary>
        public int RootCount
        {
            get
            {
                return _rootCount;
            }

            set
            {
                if (_rootCount == value)
                {
                    return;
                }

                var oldValue = _rootCount;
                _rootCount = value;

                // Verify Property Exists
                VerifyPropertyName(RootCountPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(RootCountPropertyName);

            }
        }
        /// <summary>
        /// The <see cref="CurrentRootNo" /> property's name.
        /// </summary>
        public const string CurrentRootNoPropertyName = "CurrentRootNo";

        private int _currRootNo = 0;

        /// <summary>
        /// 
        /// </summary>
        public int CurrentRootNo
        {
            get
            {
                return _currRootNo;
            }

            set
            {
                if (_currRootNo == value)
                {
                    return;
                }

                var oldValue = _currRootNo;
                _currRootNo = value;               

                // Verify Property Exists
                VerifyPropertyName(CurrentRootNoPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentRootNoPropertyName);

            }
        }       

        /// <summary>
        /// The <see cref="SelectionBuilt" /> property's name.
        /// </summary>
        public const string BuildingSelectionPropertyName = "BuildingSelection";

        private bool _buildingSelection = false;

        /// <summary>
        /// 
        /// </summary>
        public bool BuildingSelection
        {
            get
            {
                return _buildingSelection;
            }

            set
            {
                if (_buildingSelection == value)
                {
                    return;
                }

                var oldValue = _buildingSelection;
                _buildingSelection = value;                

                // Verify Property Exists
                VerifyPropertyName(BuildingSelectionPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(BuildingSelectionPropertyName);             
            }
        }

        /// <summary>
        /// The <see cref="BuildStatus" /> property's name.
        /// </summary>
        public const string BuildStatusPropertyName = "BuildStatus";

        private string _buildStats = "";

        /// <summary>
        /// 
        /// </summary>
        public string BuildStatus
        {
            get
            {
                return _buildStats;
            }

            set
            {
                if (_buildStats == value)
                {
                    return;
                }

                var oldValue = _buildStats;
                _buildStats = value;               

                // Verify Property Exists
                VerifyPropertyName(BuildStatusPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(BuildStatusPropertyName);
            }
        }
    }
}
