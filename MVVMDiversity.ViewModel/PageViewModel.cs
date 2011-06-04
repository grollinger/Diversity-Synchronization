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
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using MVVMDiversity.Messages;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using System;
using System.Windows;
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
    public abstract class PageViewModel : ViewModelBase, IPageViewModel
    {
        #region Properties

        public string NextTextID { get; private set; }

        public string PreviousTextID { get; private set; }

        public string TitleTextID { get; private set; }

        public string DescriptionTextID { get; private set; }

        /// <summary>
        /// The <see cref="CanNavigateNext" /> property's name.
        /// </summary>
        public const string CanNavigateNextPropertyName = "CanNavigateNext";

        private bool _canNavNext = false;

        /// <summary>
        /// Determines, whether the user is able to navigate Forward        
        /// </summary>
        protected bool CanNavigateNext
        {
            get
            {
                return _canNavNext;
            }

            set
            {
                if (_canNavNext == value)
                {
                    return;
                }
                _canNavNext = value;
               
                RaiseCanNavigateNextChanged();
            }
        }

        /// <summary>
        /// The <see cref="CanNavigateBack" /> property's name.
        /// </summary>
        public const string CanNavigateBackPropertyName = "CanNavigateBack";

        private bool _canNavBack = false;

        /// <summary>
        /// Determines, whether the user is able to navigate Back        
        /// </summary>
        protected bool CanNavigateBack
        {
            get
            {
                return _canNavBack;
            }

            set
            {
                if (_canNavBack == value)
                {
                    return;
                }                
                _canNavBack = value;

                RaiseCanNavigateBackChanged();
            }
        }

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

        /// <summary>
        /// Should be Set by Extending classes to signal, that it is Busy
        /// Prevents any navigation from the current Page
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            protected set
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

                RaiseCanNavigateBackChanged();
                RaiseCanNavigateNextChanged();
            }
        }

        private AsyncOperationInstance _currOp;

        /// <summary>
        /// Displays the Progress information of the set AsyncOperationInstance to the user.
        /// Set null to hide.
        /// </summary>
        protected AsyncOperationInstance CurrentOperation
        {
            get { return _currOp; }
            set 
            {
                if(_currOp != value)
                    _currOp = value;

                if (_currOp != null)
                {
                    showProgress();
                    _currOp.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OperationStateChanged);
                    OperationStateChanged(_currOp, new System.ComponentModel.PropertyChangedEventArgs(AsyncOperationInstance.StatePropertyName));
                }

                else
                    hideProgress();

                RaiseCanNavigateBackChanged();
                RaiseCanNavigateNextChanged();
            }
        }

        void  OperationStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == AsyncOperationInstance.StatePropertyName)
            {
                var op = (sender as AsyncOperationInstance);
                if (op != null && op.State != OperationState.Running)
                    CurrentOperation = null;
                    
            }
        }
        

       
        #endregion


        private DelegateCommand _navigateNext;
        public ICommand NavigateNext { get { return _navigateNext; } }


        private DelegateCommand _navigateBack;
        public ICommand NavigateBack { get { return _navigateBack; } }

        /// <summary>
        /// Contains the Page Forward Navigation navigates to.
        /// </summary>
        protected Page NextPage { get; set; }

        /// <summary>
        /// Contains the Page Backward Navigation navigates to.
        /// </summary>
        protected Page PreviousPage { get; set; }

        /// <summary>
        /// Is called before Navigating forward.
        /// </summary>
        /// <returns>Wether to continue the Navigation</returns>
        protected virtual void OnNavigateNext() { navigateNext(); }

        /// <summary>
        /// Is called before Navigating back.
        /// </summary>
        /// <returns>Wether to continue the Navigation</returns>
        protected virtual void OnNavigateBack() { navigateBack(); }

        private void RaiseCanNavigateNextChanged()
        {
            if(_navigateNext != null)
                _navigateNext.RaiseCanExecuteChanged();
        }

        private void RaiseCanNavigateBackChanged()
        {
            if(_navigateBack != null)
                _navigateBack.RaiseCanExecuteChanged();
        }

        
        /// <param name="nextTextID">Key of the String displayed on the forward Button</param>
        /// <param name="prevTextID">Key of the String displayed on the back Button</param>
        /// <param name="titleID">Key of the String displayed as the Page Title</param>
        /// <param name="descriptionID">Key of the String displayed as the Page Description</param>
        public PageViewModel(string nextTextID, string prevTextID, string titleID, string descriptionID)
            : base(Messenger.Default)
        {            
            NextTextID = nextTextID;
            PreviousTextID = prevTextID;
            TitleTextID = titleID;
            DescriptionTextID = descriptionID;
            
            _navigateNext = new DelegateCommand(() => 
            {
                OnNavigateNext();                   
            }, 
            () => 
            { 
                return !IsBusy && CurrentOperation == null && this.CanNavigateNext; 
            });
            _navigateBack = new DelegateCommand(() =>
            {
                OnNavigateBack();
            }, () => 
            {
                return !IsBusy && CurrentOperation == null && this.CanNavigateBack; 
            });
        }

        private void showProgress()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   MessengerInstance.Send<ShowProgress>(CurrentOperation);
               });
        }

        private void hideProgress()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   MessengerInstance.Send<HideProgress>(new HideProgress());
               });
        }

        protected void sendNotification(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   MessengerInstance.Send<StatusNotification>(msg);
               });
        }
        
        protected void showMessageBox(string caption, string content, Action<MessageBoxResult> callback)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    MessengerInstance.Send<DialogMessage>(
                    new DialogMessage(content, callback)
                    {
                        Button = MessageBoxButton.OK,
                        Caption = caption,
                        DefaultResult = MessageBoxResult.Cancel
                    });
                });
        }

        protected void showYesNoBox(string caption, string content, MessageBoxResult defaultResult, Action<MessageBoxResult> callback)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   MessengerInstance.Send<DialogMessage>(
                       new DialogMessage(content, callback)
                       {
                           Button = MessageBoxButton.YesNo,
                           Caption = caption,
                           DefaultResult = defaultResult
                       });
               });
        }

        protected void showError(AsyncOperationInstance op)
        {
            showMessageBox("MessageBox_Error_Title", op.StatusDescription, null);
        }

        protected void navigateNext()
        {
            MessengerInstance.Send<NavigateToPage>(new NavigateToPage(NextPage));
        }

        protected void navigateBack()
        {
            MessengerInstance.Send<NavigateToPage>(new NavigateToPage(PreviousPage));
        }
    }
}