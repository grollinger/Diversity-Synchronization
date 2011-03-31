//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
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
using MVVMDiversity.ViewModel;
using System.Windows.Data;
using MVVMDiversity.Messages;

namespace MVVMDiversity.View.Converters
{
    [ValueConversion(typeof(Page), typeof(Uri))]
    public class VMToVConverter : IValueConverter
    {
        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Page && targetType == typeof(Uri))
            {
                var page = (Page)value;
                switch (page)
                {
                    case Page.Connections:
                        return new Uri("Pages/ConnectionsView.xaml", UriKind.Relative);
                    case Page.ProjectSelection:
                        return new Uri("Pages/ProjectSelectionView.xaml", UriKind.Relative);
                    case Page.Actions:
                        return new Uri("Pages/ActionsView.xaml", UriKind.Relative);                        
                    case Page.FieldData:
                        return new Uri("Pages/SelectFieldDataView.xaml", UriKind.Relative);
                    case Page.FinalSelection:
                        return new Uri("Pages/SelectionView.xaml", UriKind.Relative);
                    case Page.Map:
                        return new Uri("Pages/MapView.xaml", UriKind.Relative);
                    default:
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
