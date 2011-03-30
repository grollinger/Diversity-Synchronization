using System.Windows;
using MVVMDiversity.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.View.Dialogs
{
    /// <summary>
    /// Description for YesNoWindow.
    /// </summary>
    public partial class YesNoWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the YesNoWindow class.
        /// </summary>
        public YesNoWindow(DialogMessage vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        DialogMessage VM { get { return DataContext as DialogMessage; } }

        public void Yes_Click(object sender, RoutedEventArgs args)
        {
            if (VM != null && VM.Callback != null)
                VM.Callback(MessageBoxResult.Yes);
            Close();
                
        }
        public void No_Click(object sender, RoutedEventArgs args)
        {
            if (VM != null && VM.Callback != null)
                VM.Callback(MessageBoxResult.No);
            Close();
        }
    }
}