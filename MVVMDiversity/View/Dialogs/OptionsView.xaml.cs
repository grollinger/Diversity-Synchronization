using System.Windows;
using MVVMDiversity.ViewModel;
using MVVMDiversity.Interface;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.IO;
using System;

namespace MVVMDiversity.View
{
    /// <summary>
    /// Description for OptionsView.
    /// </summary>
    public partial class OptionsView : Window, ICloseableView
    {
        private ViewModelLocator Locator { get { return FindResource("Locator") as ViewModelLocator; } }

        private OptionsViewModel VM { get { return DataContext as OptionsViewModel; } }
        

        /// <summary>
        /// Initializes a new instance of the OptionsView class.
        /// </summary>
        public OptionsView()
        {
            InitializeComponent();

            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void ICloseableView.Close()
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null && VM.SaveOptions.CanExecute(null))
            {
                VM.SaveOptions.Execute(null);
                this.Close();
            }
        }

        private void Browse_MobileDB(object sender, RoutedEventArgs e)
        {

            string initialDirectory = containingDirectory(VM.Options.Paths.MobileDB);

            var odf = new OpenFileDialog()
            {
                CheckFileExists = true,
                DefaultExt = "sdf"
            };

            if (initialDirectory != null)
                odf.InitialDirectory = initialDirectory;

            if (odf.ShowDialog() ?? false)
                VM.Options.Paths.MobileDB = odf.FileName;
        }

        private string containingDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var dir = Path.GetDirectoryName(filePath);
                if (Directory.Exists(dir))
                    return dir;
            }
            return null;
        }

        private void Browse_MobileTaxa(object sender, RoutedEventArgs e)
        {
            string initialDirectory = containingDirectory(VM.Options.Paths.MobileTaxa);

            var odf = new OpenFileDialog()
            {
                CheckFileExists = true,
                DefaultExt = "sdf"
            };

            if (initialDirectory != null)
                odf.InitialDirectory = initialDirectory;

            if (odf.ShowDialog() ?? false)
                VM.Options.Paths.MobileTaxa = odf.FileName;
        }        
    }
}