using System.Windows;

namespace MVVMDiversity.View
{
    /// <summary>
    /// Description for TaxonView.
    /// </summary>
    public partial class TaxonView : Window
    {
        /// <summary>
        /// Initializes a new instance of the TaxonView class.
        /// </summary>
        public TaxonView()
        {
            InitializeComponent();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}