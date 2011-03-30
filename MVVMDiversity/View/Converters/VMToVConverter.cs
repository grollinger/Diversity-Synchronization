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
