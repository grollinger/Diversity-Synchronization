//#######################################################################
//WPF Localization Extension
//Based On:  http://www.codeproject.com/KB/WPF/WPF_Localization.aspx
//Copyright (C) 2011  Georg Rollinger
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

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
