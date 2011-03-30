using System;
using System.Windows.Input;
namespace MVVMDiversity.Interface
{
    public interface IPageViewModel
    {
        string DescriptionTextID { get; }
        ICommand NavigateBack { get; }
        ICommand NavigateNext { get; }
        string NextTextID { get; }
        string PreviousTextID { get; }
        string TitleTextID { get; }
    }
}
