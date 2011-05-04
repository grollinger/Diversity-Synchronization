using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFLocalization;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using MVVMDiversity.View.Dialogs;
using GalaSoft.MvvmLight.Threading;


namespace MVVMDiversity.View
{
    /// <summary>
    /// Interaktionslogik für FrameWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<CustomDialog>(this,
                (msg) =>
                {
                    switch (msg.Content)
                    {
                        case Dialog.Options:
                            new OptionsView().ShowDialog();
                            break;
                        case Dialog.Taxon:
                            new TaxonView().ShowDialog();
                            break;
                        case Dialog.About:
                            new AboutView().ShowDialog();
                            break;
                        default:
                            break;
                    }
                });
            Messenger.Default.Register<DialogMessage>(this,
                (msg) =>
                {
                    switch (msg.Button)
                    {                                               
                        case MessageBoxButton.YesNo:
                            new YesNoWindow(msg).ShowDialog();
                            break;
                        default:
                            new MessageBoxWindow(msg).ShowDialog();
                            break; 
                    }
                });

            Messenger.Default.Register<ApplicationClosing>(this, (msg) =>
                {
                    if (!msg.WarningOnly)
                    {
                        finalShutdown = true;
                        DispatcherHelper.CheckBeginInvokeOnUI(()=> Application.Current.Shutdown());                       
                    }
                });

        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<CustomDialog>(Dialog.Options);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<CustomDialog>(Dialog.About);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool finalShutdown = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !finalShutdown;
            new Action(() => { Messenger.Default.Send<ApplicationClosing>(new ApplicationClosing(!finalShutdown)); }).BeginInvoke(null, null);
        }

        

        
        
    }
}
