using System.Windows;
using MVVMDiversity.ViewModel;

namespace MVVMDiversity.View
{
    /// <summary>
    /// Description for TaxonView.
    /// </summary>
    public partial class TaxonView : Window
    {

        private TaxonViewModel VM { get { return DataContext as TaxonViewModel; } }
        /// <summary>
        /// Initializes a new instance of the TaxonView class.
        /// </summary>
        public TaxonView()
        {
            InitializeComponent();            
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null && VM.DownloadTaxa != null)
            {
                VM.DownloadTaxa.Execute(taxonLists.SelectedItems);
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VM != null)
                VM.OnClose();
        }
    }
}