using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace MVVMDiversity.View.Dialogs
{
    /// <summary>
    /// Description for MessageBoxWindow.
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        private bool _calledBack = false;
        private DialogMessage VM { get { return DataContext as DialogMessage; } }
        /// <summary>
        /// Initializes a new instance of the MessageBoxWindow class.
        /// </summary>
        public MessageBoxWindow( DialogMessage vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null && VM.Callback != null)
            {
                VM.Callback(MessageBoxResult.OK);
                _calledBack = true;
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_calledBack && VM != null && VM.Callback != null)
            {
                VM.Callback(VM.DefaultResult);                
            }
        }
    }
}