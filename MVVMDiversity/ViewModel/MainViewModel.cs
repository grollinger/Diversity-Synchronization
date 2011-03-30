using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using MVVMDiversity.Interface;

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
        IPageViewModel _currentPage;
        public const string CurrentPagePropertyName = "CurrentPage";
        public IPageViewModel CurrentPage
        {
            get
            {
                return _currentPage;
            }
            private set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    VerifyPropertyName(CurrentPagePropertyName);
                    RaisePropertyChanged(CurrentPagePropertyName);
                }
            }

        }
        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
            : base(Messenger.Default)
        {
            CurrentPage = ViewModelLocator.ConnectionsStatic;
            MessengerInstance.Register<NavigateToPage>(this, (msg) =>
                {
                    if (msg.Content != null)
                        CurrentPage = msg.Content;
                });
        }        
    }
}