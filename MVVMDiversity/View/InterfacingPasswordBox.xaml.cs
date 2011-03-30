using System;
using System.Collections.Generic;
using System.Linq;
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
using MVVMDiversity.Interface;
using System.Security;
using System.ComponentModel;

namespace MVVMDiversity.View
{
    /// <summary>
    /// Interaktionslogik für InterfacingPasswordBox.xaml
    /// </summary>
    public partial class InterfacingPasswordBox : UserControl, INotifyPropertyChanged
    {
        public InterfacingPasswordBox()
        {
            InitializeComponent();

            InnerPB.PasswordChanged += (sender, args) => 
            {
                if(PropertyChanged != null)
                    PropertyChanged(this,new PropertyChangedEventArgs("Password"));
            };
        }

       

        public SecureString Password
        {
            get
            {
                return InnerPB.SecurePassword;
            }
            set
            {
                InnerPB.Password = value.ToString() ;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        
            
    }
}
