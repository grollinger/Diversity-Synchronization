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

        #endregion

        public ICommand NavigateNext { get; private set; }

        public ICommand NavigateBack { get; private set; }

        protected PageViewModel NextPage { get; set; }

        protected PageViewModel PreviousPage { get; set; }

        protected virtual bool OnNavigateNext() { return true; }

        protected virtual bool OnNavigateBack() { return true; }

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
            
            NavigateNext = new RelayCommand(() => 
            {
                if(OnNavigateNext())
                    MessengerInstance.Send<NavigateToPage>(new NavigateToPage(NextPage));
            }, 
            () => 
            { 
                return this.CanNavigateNext; 
            });
            NavigateBack = new RelayCommand(() =>
            {
                if(OnNavigateBack())
                    MessengerInstance.Send<NavigateToPage>(new NavigateToPage(PreviousPage)); 
            }, () => 
            { 
                return this.CanNavigateBack; 
            });
        }         


        public static implicit operator NavigateToPage(PageViewModel p)
        {
            return new NavigateToPage(p);
        }
    }
}