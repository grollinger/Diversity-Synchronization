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

namespace MVVMDiversity.ViewModel
{
    public class SelectionViewModel : PageViewModel
    {
        private ILog _Log = LogManager.GetLogger(typeof(SelectionViewModel));

        [Dependency]
        public IFieldDataService FDSvc { get; set; }


        public SelectionViewModel()
            : base("Selection_Next", "Selection_Previous", "Selection_Title","")
        {
            PreviousPage = Messages.Page.FieldData;
            NextPage = Messages.Page.Actions;

            MessengerInstance.Register<GenericMessage<ICollection<ISerializableObject>>>(this, (msg) =>
            {
                Selection = msg.Content;
            });
            MessengerInstance.Register<GenericMessage<ITreeViewModel>>(this, (msg) =>
            {
                SelectionTree = msg.Content;
            });
        }

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

        /// <summary>
        /// Gets the IsBusy property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                var oldValue = _isBusy;
                _isBusy = value;               

                // Verify Property Exists
                VerifyPropertyName(IsBusyPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }
        protected override bool OnNavigateNext()
        {
            if (!IsBusy)
            {
                if (FDSvc != null)
                {
                    if (SelectionTree != null)
                    {
                        var progress = FDSvc.downloadData(SelectionTree.buildSelection(),
                            () =>
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(
                                () =>
                                {
                                    MessengerInstance.Send<SyncStepFinished>(SyncState.FieldDataDownloaded);
                                    MessengerInstance.Send<HideProgress>(new HideProgress());
                                    NavigateNext.Execute(null);
                                });
                            });

                        MessengerInstance.Send<ShowProgress>(progress);

                        //Cancel Navigation
                        return true;
                    }
                }
                else
                    _Log.Error("FieldDataService N/A");


            }
            else
            {
                //Resume Navigation
                IsBusy = false;
            }

            return base.OnNavigateNext();
        }


        /// <summary>
        /// The <see cref="Selection" /> property's name.
        /// </summary>
        public const string SelectionPropertyName = "Selection";

        private ICollection<ISerializableObject> _selection = null;

        /// <summary>
        /// Gets the Selection property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ICollection<ISerializableObject> Selection
        {
            get
            {
                return _selection;
            }

            private set
            {
                if (_selection == value)
                {
                    return;
                }
                
                _selection = value;                     

                // Verify Property Exists
                VerifyPropertyName(SelectionPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectionPropertyName);
            }
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


        protected override bool CanNavigateNext
        {
            get { return true; }
        }

        protected override bool CanNavigateBack
        {
            get { return true; }
        }
    }
}
