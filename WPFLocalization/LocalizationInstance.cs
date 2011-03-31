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
using System.Reflection;
using System.Windows;

namespace WPFLocalization
{
    class LocalizationInstance
    {

        string _key;
        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        public string Key 
        {
            get
            { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    UpdateTargetValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the formatting string to use.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The property localized by the instance.
        /// </summary>
        PropertyInfo _targetProperty;

        /// <summary>
        /// The property localized by the instance.
        /// </summary>
        DependencyProperty _targetDProperty;

        

        /// <summary>
        /// The list of instances created by a template that own the <see cref="DependencyProperty"/>
        /// localized by the instance.
        /// </summary>
        List<WeakReference> _targetObjects = new List<WeakReference>();

        private LocalizationInstance ()
	    {
            LocalizationManager.AddLocalization(this);
	    }

        LocalizationInstance(DependencyProperty targetProperty)
            :this()
        {
            _targetDProperty = targetProperty;
        }
        LocalizationInstance(PropertyInfo targetProperty)
            :this()
        {
            _targetProperty = targetProperty;
        }

        public void AddTarget(DependencyObject t)
        {
            _targetObjects.Add(new WeakReference(t));
        }

        public static LocalizationInstance fromTargetProperty(object property)
        {
            if (property != null)
            {
                if (property is DependencyProperty)
                {
                    return new LocalizationInstance(property as DependencyProperty);
                }
                else if (property is PropertyInfo)
                {
                    return new LocalizationInstance(property as PropertyInfo);
                }
                throw new NotSupportedException();
            }
            return null;
        }
        /// <summary>
        /// Gets value indicating if the instance localized by this instance is alive.
        /// </summary>
        internal bool IsAlive
        {
            get
            {
                // Verify if the extension is used in a template

                if (_targetObjects != null)
                {
                    foreach (var item in _targetObjects)
                    {
                        if (item.IsAlive)
                        {
                            return true;
                        }
                    }                    
                }
                return false;
            }
        }


        /// <summary>
        /// Updates the value of the localized object.
        /// </summary>
        public object GetValue()
        {
            return GetValue(Key, Format);
        }

        /// <summary>
        /// Updates the value of the localized object.
        /// </summary>
        internal void UpdateTargetValue()
        {    

            if (_targetProperty != null)
            {
                foreach (var item in _targetObjects)
                {
                    var targetObject = item.IsAlive ? item.Target as DependencyObject : null;

                    if (targetObject != null)
                    {
                        _targetProperty.SetValue(targetObject, GetValue(Key, Format), null);
                    }
                }
            }
            else if(_targetDProperty != null)
            {                   
                foreach (var item in _targetObjects)
                {
                    var targetObject = item.IsAlive? item.Target as DependencyObject : null;

                    if (targetObject != null)
                    {
                        targetObject.SetValue(_targetDProperty, GetValue(Key, Format));
                    }
                }        
            }
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="key">the resource key.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        static object GetValue(string key, string format)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            var manager = LocalizationManager.ResourceManager;

            object value;

#if DEBUG
            //value = manager == null ? string.Empty : manager.GetObject(key) ?? "[Resource: " + key + "]";

            if (manager == null)
            {
                value = "";
            }
            else
            {
                value = manager.GetObject(key);

                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("key", key, "Resource not found.");
                }
            }
#else
            value = manager == null ? string.Empty : manager.GetObject(key) ?? string.Empty;
#endif

            if (string.IsNullOrEmpty(format))
            {
                return value;
            }
            else
            {
                return string.Format(format, value);
            }
        }

    }
}
