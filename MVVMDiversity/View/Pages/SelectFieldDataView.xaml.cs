using System.Windows;
using System.Windows.Controls;
using MVVMDiversity.ViewModel;
using MVVMDiversity.Interface;
using System.Linq;


namespace MVVMDiversity.View
{
    /// <summary>
    /// Description for SelectFieldDataView.
    /// </summary>
    public partial class SelectFieldDataView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the SelectFieldDataView class.
        /// </summary>
        public SelectFieldDataView()
        {
            InitializeComponent();
        }

        private SelectFieldDataViewModel VM { get { return DataContext as SelectFieldDataViewModel; } }

        private void queryResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM != null)
            {
                var selected = from object item in e.AddedItems
                               where item is IISOViewModel
                               select item as IISOViewModel;
                var unselected = from object item in e.RemovedItems
                                 where item is IISOViewModel
                                 select item as IISOViewModel;

                VM.QuerySelectionChanged(selected, unselected);
            }
        }

        private void QuerySelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            queryResultList.SelectAll();
        }

        private void SelectionSelectAll_Click(object sender, RoutedEventArgs e)
        {
            selectionList.SelectAll();
        }        
    }
}