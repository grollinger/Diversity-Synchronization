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
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using System.Diagnostics;

namespace WPFLocalization
{      
    public class LocBindingExtension : MarkupExtension
    {
        public LocBindingExtension()
        {

        }
        public LocBindingExtension(PropertyPath path)        
        {
            Path = path;
        }       
    
        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }

        public string Default { get; set; }

        private LocalizationInstance _locInstance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service != null && service.TargetProperty != null)
            {                           
                var targetObject = service.TargetObject as FrameworkElement;
                if (targetObject != null)
                {
                    _locInstance = LocalizationInstance.fromTargetProperty(service.TargetProperty);
                    _locInstance.AddTarget(targetObject);
                    
                    var bTarget = new LocBindingTarget(_locInstance, targetObject, Path);
                   
                    return _locInstance.GetValue();
                }
                else
                    return this;
            }
            return null;            
        }
    }
}
