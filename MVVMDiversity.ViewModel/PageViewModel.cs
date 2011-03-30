using GalaSoft.MvvmLight;
using System;
using System.Resources;
using GalaSoft.MvvmLight.Messaging;
using System.Globalization;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MVVMDiversity.Messages;
using MVVMDiversity.Interface;
using System.ComponentModel;
using MVVMDiversity.Model;
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

        protected abstract bool CanNavigateNext { get; }

        protected abstract bool CanNavigateBack { get; }

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
            }
        }

        #endregion


        private DelegateCommand _navigateNext;
        public ICommand NavigateNext { get { return _navigateNext; } }


        private DelegateCommand _navigateBack;
        public ICommand NavigateBack { get { return _navigateBack; } }

        protected Page NextPage { get; set; }

        protected Page PreviousPage { get; set; }

        protected virtual bool OnNavigateNext() { return true; }

        protected virtual bool OnNavigateBack() { return true; }

        protected void navigateNextChanged()
        {
            if(_navigateNext != null)
                _navigateNext.RaiseCanExecuteChanged();
        }

        protected void navigateBackChanged()
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
                return this.CanNavigateNext; 
            });
            _navigateBack = new DelegateCommand(() =>
            {
                if(OnNavigateBack())
                    MessengerInstance.Send<NavigateToPage>(new NavigateToPage(PreviousPage)); 
            }, () => 
            { 
                return this.CanNavigateBack; 
            });
        }

        


        
    }
}