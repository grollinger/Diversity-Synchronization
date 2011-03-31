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
using System.Windows.Data;
using MVVMDiversity.ViewModel;
using System.Windows.Media;
using System.Drawing;

namespace MVVMDiversity.View.Converters
{
    [ValueConversion(typeof(ISOIcon), typeof(string))]
    public class IconToPathConverter : IValueConverter
    {
        private static readonly string ICON_PATH = "pack://application:,,,/Images/Icons/{0}.bmp";
        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format(ICON_PATH, fileNameFromIcon((ISOIcon)value));
        }

        private string fileNameFromIcon(ISOIcon iSOIcon)
        {
            switch (iSOIcon)
            {
                case ISOIcon.EventSeries:
                    return "Series";
                case ISOIcon.EventSeriesGrey:
                    return "SeriesGrey";

                case ISOIcon.SiteProperty:
                    break;
                case ISOIcon.SitePropertyGrey:
                    break;               
                case ISOIcon.Location:
                    return "Localisation";
                case ISOIcon.LocationGrey:
                    return "LocalisationGrey";               
                case ISOIcon.Specimen:
                    return "Barcode";
                case ISOIcon.SpecimenGrey:
                    return "BarcodeGrey";
                case ISOIcon.SpecimenRed:
                    return "BarcodeRed";                
                case ISOIcon.Branch:
                    return "Ast";
                case ISOIcon.BranchGrey:
                    return "AstGrey";            
                
                case ISOIcon.GPSGrey:
                    break;
                case ISOIcon.Home:
                    break;
                case ISOIcon.Location0:
                    break;
                case ISOIcon.Location4:
                    break;
                case ISOIcon.Location5:
                    break;
                case ISOIcon.Location6:
                    break;                
                default:
                    return iSOIcon.ToString();                    
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}

