using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace WPFLocalization
{
    class LocBindingTarget : DependencyObject
    {
        private static HashSet<LocBindingTarget> _lbCollection = new HashSet<LocBindingTarget>();

        LocalizationInstance _locInstance;

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(LocBindingTarget), new UIPropertyMetadata("", 
                (sender, args) => 
                { 
                    var lbt = sender as LocBindingTarget;
                    
                    if (lbt != null)
                        lbt.UpdateLocalization();
                }));

        private PropertyPath _path;
        public LocBindingTarget(LocalizationInstance loc, FrameworkElement targetObject, PropertyPath path)
        {
            _locInstance = loc;
            _lbCollection.Add(this);
            _path = path;
            
            
            targetObject.DataContextChanged += new DependencyPropertyChangedEventHandler(update_binding);

            update_binding(targetObject, new DependencyPropertyChangedEventArgs(FrameworkElement.DataContextProperty, null, targetObject.DataContext));

        }

        void update_binding(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindingOperations.ClearBinding(this, LocBindingTarget.KeyProperty);

            

            if (e.NewValue != null)
            {
                var binding = new Binding()
                {
                    BindsDirectlyToSource = true,
                    Path = _path,
                    Mode = BindingMode.OneWay,
                    Source = e.NewValue
                };
                BindingOperations.SetBinding(this, LocBindingTarget.KeyProperty, binding);
            }
            
        }



        public void UpdateLocalization()
        {
            if(_locInstance != null)
                _locInstance.Key = Key;
        }
    }
}
