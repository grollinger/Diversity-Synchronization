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
using System.Text;
using System.Windows.Markup;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.ComponentModel;

// Register the extention in the Microsoft's default namespaces
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "WPFLocalization")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "WPFLocalization")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "WPFLocalization")]

namespace WPFLocalization
{
    /// <summary>
    /// Represents a localization makrup extension.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    [ContentProperty("Key")]
    public class LocExtension : MarkupExtension
    {
        internal LocalizationInstance _locInstance;

        public string Key { get; set; }

        public string Format { get; set; }

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        public LocExtension() { }

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        public LocExtension(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="serviceProvider">An object that can provide services for the markup extension.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service != null && service.TargetProperty != null)
            {
                if (_locInstance == null)
                {
                    _locInstance = LocalizationInstance.fromTargetProperty(service.TargetProperty);
                    _locInstance.Key = Key;
                    _locInstance.Format = Format;                    
                }
                if (service.TargetObject is DependencyObject)
                {
                    _locInstance.AddTarget(service.TargetObject as DependencyObject);
                    return _locInstance.GetValue();
                }
                else
                {
                    //Localization is used in a Template
                    return this;
                }
            }
            return null;
        }   
    }
}