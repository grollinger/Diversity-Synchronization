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
