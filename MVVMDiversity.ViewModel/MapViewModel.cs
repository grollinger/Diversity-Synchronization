using GalaSoft.MvvmLight;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using MVVMDiversity.Messages;

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
    public class MapViewModel : PageViewModel
    {
        [Dependency]
        public IUserOptionsService Settings { get; set; }

        protected override bool CanNavigateBack
        {
            get { return true; }
        }

        protected override bool CanNavigateNext
        {
            get { return false; }
        }
        /// <summary>
        /// Initializes a new instance of the MapViewModel class.
        /// </summary>
        public MapViewModel()
            : base("Map_Next", "Map_Previous", "Map_Title", "Map_Description")
        {
            NextPage = Page.Actions;
            PreviousPage = Page.Actions;
        }

        protected override bool OnNavigateNext()
        {

            return base.OnNavigateNext();
        }
    }
}