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
using MVVMDiversity.Enums;

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

            //Break if the Image is named the same as the Enum-Member
            //Else, map it...
            switch (iSOIcon)
            {
                case ISOIcon.Unknown:
                    return "Leer";
                case ISOIcon.Event:
                    break;
                case ISOIcon.EventGrey:
                    break;
                case ISOIcon.SiteProperty: //TODO Missing?
                    break;
                case ISOIcon.SitePropertyGrey: // TODO Missing?
                    break;
                case ISOIcon.EventSeries:
                    return "Series";                    
                case ISOIcon.EventSeriesGrey:
                    return "SeriesGrey";                    
                case ISOIcon.Location:
                    return "Localisation";
                case ISOIcon.LocationGrey:
                    return "LocalisationGrey";
                case ISOIcon.Agent:
                    break;
                case ISOIcon.AgentGrey:
                    break;
                case ISOIcon.Specimen:
                    return "Barcode";
                case ISOIcon.SpecimenGrey:
                    return "BarcodeGrey";
                case ISOIcon.SpecimenRed:
                    return "BarcodeRed";
                case ISOIcon.Tree:
                    break;
                case ISOIcon.TreeGrey:
                    break;
                case ISOIcon.Branch:
                    return "Ast";
                case ISOIcon.BranchGrey:
                    return "AstGrey";
                case ISOIcon.Plant:
                    break;
                case ISOIcon.PlantGrey:
                    break;
                case ISOIcon.Other:
                    break;
                case ISOIcon.OtherGrey:
                    break;
                case ISOIcon.Analysis:
                    break;
                case ISOIcon.AnalysisGrey:
                    break;
                case ISOIcon.Foto:
                    return "Camera";
                case ISOIcon.FotoGrey:
                    return "CameraGrey";
                case ISOIcon.Alge:
                    break;
                case ISOIcon.Assel:
                    break;
                case ISOIcon.Bacterium:
                    break;
                case ISOIcon.Bird:
                    break;
                case ISOIcon.Bryophyt:
                    break;
                case ISOIcon.Fish:
                    break;
                case ISOIcon.Fungus:
                    break;
                case ISOIcon.Insect:
                    break;
                case ISOIcon.Lichen:
                    break;
                case ISOIcon.Mammal:
                    break;
                case ISOIcon.Mollusc:
                    break;
                case ISOIcon.Myxomycete:
                    break;
                case ISOIcon.Virus:
                    break;
                case ISOIcon.GeoAnalysis:
                    break;
                case ISOIcon.GeoAnalysisGrey:
                    break;
                case ISOIcon.GPS:
                    break;
                case ISOIcon.GPSGrey: //TODO fehlt?
                    break;
                case ISOIcon.Home:
                    break;
                case ISOIcon.Location0:
                    return "GPS_N0";
                case ISOIcon.Location4:
                    return "GPS_N3";
                case ISOIcon.Location5:
                    return "GPS_N5";
                case ISOIcon.Location6:
                    return "GPS_N7";                    
            }
            return iSOIcon.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}

