using GalaSoft.MvvmLight;
using MVVMDiversity.Model;
using System.Collections.Generic;

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
    public class SelectFieldDataViewModel : PageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the SelectFieldDataViewModel class.
        /// </summary>
        public SelectFieldDataViewModel()
            : base("SelectFD_Next","SelectFD_Previous","SelectFD_Title","SelectFD_Description")
        {

        }

        public IList<Restriction> Restrictions { get; private set; }

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