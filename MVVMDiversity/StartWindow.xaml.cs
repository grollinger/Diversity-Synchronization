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
using System.Windows.Shapes;
using System.Windows.Threading;
using GalaSoft.MvvmLight;



namespace MVVMDiversity.View
{
    /// <summary>
    /// Interaktionslogik für StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {    
        private DispatcherTimer _countdown = null;
        private bool _closing = false;

        public StartWindow()
        {

            InitializeComponent();
            
        }        

        /// <summary>
        /// Startet Timer für die Dauer der Anzeige des Startbildschirms
        /// </summary>       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._closing)
            {
                if (ViewModelBase.IsInDesignModeStatic)
                    Countdown_Tick(null, null);
                else
                {
                    _countdown = new DispatcherTimer();
                    _countdown.Interval = TimeSpan.FromMilliseconds(3000);
                    _countdown.Tick += new EventHandler(Countdown_Tick);
                    _countdown.Start();
                }
            }
            else
                this.Close();
        }

        /**
         * Beendet Countdown und ruft Hauptfenster auf.
         */
        void Countdown_Tick(object sender, EventArgs e)
        {
            _countdown.Stop();
            new MainWindow().Show();
            this.Close();
        }
    }
}
