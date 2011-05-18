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
using MVVMDiversity.Messages;
using MVVMDiversity.Interface;

using MVVMDiversity.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;
using log4net;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.ComponentModel;

namespace MVVMDiversity.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
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
    public class MainViewModel : ViewModelBase
    {
        

        private ILog _Log = LogManager.GetLogger(typeof(MainViewModel));

        private AsyncQueueWorker<string> _notificationsQueue;
        #region Properties

        public ICommand CancelOperation { get; private set; }

        PageViewModel _currentVM;
        public const string CurrentVMPropertyName = "CurrentVM";
        public PageViewModel CurrentVM
        {
            get
            {
                return _currentVM;
            }
            private set
            {
                if (_currentVM != value)
                {
                    _currentVM = value;
                    VerifyPropertyName(CurrentVMPropertyName);
                    RaisePropertyChanged(CurrentVMPropertyName);
                }
            }

        }

        /// <summary>
        /// The <see cref="CurrentPage" /> property's name.
        /// </summary>
        public const string CurrentPagePropertyName = "CurrentPage";

        private Page _currentPage = Page.Connections;

        /// <summary>
        /// Gets the CurrentPage property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Page CurrentPage
        {
            get
            {
                return _currentPage;
            }

            private set
            {
                if (_currentPage == value)
                {
                    return;
                }

                var oldValue = _currentPage;
                _currentPage = value;


                // Verify Property Exists
                VerifyPropertyName(CurrentPagePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentPagePropertyName);              
            }
        }

        /// <summary>
        /// The <see cref="Progress" /> property's name.
        /// </summary>
        public const string ProgressPropertyName = "Progress";

        private AsyncOperationInstance _p = null;

        /// <summary>
        /// Gets the Progress property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public AsyncOperationInstance Progress
        {
            get
            {
                return _p;
            }

            set
            {
                if (_p == value)
                {
                    return;
                }

                var oldP = _p;

                if (oldP != null)
                    oldP.PropertyChanged -= progressChanged;
                _p = value;
                if (_p != null)
                    _p.PropertyChanged += progressChanged;
                
               

                // Verify Property Exists
                VerifyPropertyName(ProgressPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(ProgressPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ShowProgress" /> property's name.
        /// </summary>
        public const string ShowProgressPropertyName = "ShowProgress";

        private bool _showProg = false;

        /// <summary>
        /// Gets the ShowProgress property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool ShowProgress
        {
            get
            {
                return _showProg;
            }

            set
            {
                if (_showProg == value)
                {
                    return;
                }

                var oldValue = _showProg;
                _showProg = value;
                
                // Verify Property Exists
                VerifyPropertyName(ShowProgressPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(ShowProgressPropertyName);
          
            }
        }

        /// <summary>
        /// The <see cref="Status" /> property's name.
        /// </summary>
        public const string StatusPropertyName = "Status";

        private string _status = "";

        /// <summary>
        /// Gets the Status property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                var oldValue = _status;
                _status = value;
              
                // Verify Property Exists
                VerifyPropertyName(StatusPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(StatusPropertyName);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
            : base(Messenger.Default)
        {
            _notificationsQueue = new AsyncQueueWorker<string>((msg) =>
                {
                    setStatus(msg);
                    Thread.Sleep(500); //Delay next Notification                   
                });

            CancelOperation = new RelayCommand(
                () =>
                {
                    if (Progress != null)
                        Progress.IsCancelRequested = true;
                },
                () =>
                {
                    if (Progress != null)
                        return Progress.CanBeCanceled;
                    return false;
                });
            MessengerInstance.Register<ShowProgress>(this,
                (msg) =>
                {
                    Progress = msg.Content;
                    ShowProgress = true;
                });

            MessengerInstance.Register<StatusNotification>(this,(msg) =>
                {
                    _notificationsQueue.Enqueue(msg.Content);
                });
            MessengerInstance.Register<HideProgress>(this,
                (msg) =>
                {
                    ShowProgress = false;
                });
            MessengerInstance.Register<NavigateToPage>(this, (msg) =>
                {                    
                    CurrentVM = vmFromPage(msg.Content);
                    CurrentPage = msg.Content;
                });
            MessengerInstance.Send<NavigateToPage>(Page.Connections);
        }

        private void setStatus(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Status = msg;
            });
        }

        private PageViewModel vmFromPage(Page p)
        {
            switch (p)
            {
                case Page.Connections:
                    return ViewModelLocator.ConnectionsStatic;
                    
                case Page.ProjectSelection:
                    return ViewModelLocator.ProjectSelectionStatic;
                    
                case Page.Actions:
                    return ViewModelLocator.ActionsStatic;
                    
                case Page.FieldData:
                    return ViewModelLocator.SelectFieldDataStatic;
                    
                case Page.FinalSelection:
                    return ViewModelLocator.SelectionStatic;
                    
                case Page.Map:
                    return ViewModelLocator.MapStatic;
                
                default:
                    return null;
                    
            }
        }

        private void progressChanged(object sender, PropertyChangedEventArgs args)
        {
            VerifyPropertyName(ProgressPropertyName);
            RaisePropertyChanged(ProgressPropertyName);
        }     

        
    }
}