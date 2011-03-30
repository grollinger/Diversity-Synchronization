using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

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
