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
    public partial class InterfacingPasswordBox : UserControl
    {
        public InterfacingPasswordBox()
        {
            InitializeComponent();

            InnerPB.PasswordChanged += (sender, args) => 
            {
                if (Password != InnerPB.Password) 
                    Password = InnerPB.Password;
            };

            
        }

        internal void updatePassword()
        {
            if (InnerPB.Password != Password)
                InnerPB.Password = Password;
        }



        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(InterfacingPasswordBox), new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,//Bloody important, this is!
                (sender, args)=>
                {
                    if (sender != null)
                        (sender as InterfacingPasswordBox).updatePassword();
                }));



              
            
    }
}
