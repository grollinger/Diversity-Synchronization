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
                SelectionTree = new AsyncTreeViewModel(ISOStore);
                foreach (var vm in msg.Content)
                {
                    SelectionTree.addGenerator(vm);
                }

                new Action(() =>
                    {
                        _completeSelection = SelectionTree.buildSelection();
                        DispatcherHelper.CheckBeginInvokeOnUI(()=>
                            {
                                CanNavigateBack = true;
                                CanNavigateNext = true;
                            });
                    }).BeginInvoke(null, null);
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
        /// Gets the SelectionTree property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
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
    }
}
