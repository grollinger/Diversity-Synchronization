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
        /// Gets the CanNavigateNext property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
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
        /// Gets the CanNavigateBack property.
        /// TODO Update documentation:
        /// 
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

        protected BackgroundOperation CurrentOperation { get; set; }
        #endregion


        private DelegateCommand _navigateNext;
        public ICommand NavigateNext { get { return _navigateNext; } }


        private DelegateCommand _navigateBack;
        public ICommand NavigateBack { get { return _navigateBack; } }

        protected Page NextPage { get; set; }

        protected Page PreviousPage { get; set; }

        protected virtual bool OnNavigateNext() { return true; }

        protected virtual bool OnNavigateBack() { return true; }

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

        /// <summary>
        /// Initializes a new instance of the PageViewModel class.
        /// </summary>
        public PageViewModel(string nextTextID, string prevTextID, string titleID, string descriptionID)
            : base(Messenger.Default)
        {            
            NextTextID = nextTextID;
            PreviousTextID = prevTextID;
            TitleTextID = titleID;
            DescriptionTextID = descriptionID;
            
            _navigateNext = new DelegateCommand(() => 
            {
                if(OnNavigateNext())
                    MessengerInstance.Send<NavigateToPage>(new NavigateToPage(NextPage));
            }, 
            () => 
            { 
                return !IsBusy && this.CanNavigateNext; 
            });
            _navigateBack = new DelegateCommand(() =>
            {
                if(OnNavigateBack())
                    MessengerInstance.Send<NavigateToPage>(new NavigateToPage(PreviousPage)); 
            }, () => 
            { 
                return !IsBusy && this.CanNavigateBack; 
            });
        }

        protected void showProgress()
        {
            MessengerInstance.Send<ShowProgress>(CurrentOperation);
        }

        protected void hideProgress()
        {
            MessengerInstance.Send<HideProgress>(new HideProgress());
        }

        protected bool operationFailed(BackgroundOperation o)
        {
            return (o.OperationState == BackgroundOperation.State.Failed);
        }

        protected void showMessageBox(string caption, string content, Action<MessageBoxResult> callback)
        {
            MessengerInstance.Send<DialogMessage>(
                new DialogMessage(content, callback)
                {
                    Button = MessageBoxButton.OK,
                    Caption = caption,
                    DefaultResult = MessageBoxResult.Cancel
                });
        }

        protected void showYesNoBox(string caption, string content, MessageBoxResult defaultResult, Action<MessageBoxResult> callback)
        {
            MessengerInstance.Send<DialogMessage>(
                new DialogMessage(content, callback)
                {
                    Button = MessageBoxButton.YesNo,
                    Caption = caption,
                    DefaultResult = defaultResult
                });
        }
        
    }
}