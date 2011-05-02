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
using System.ComponentModel;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Interface;
using MVVMDiversity.Enums;

namespace MVVMDiversity.ViewModel
{
    public abstract partial class ISOViewModel : IISOViewModel
    {
        public ISerializableObject ISO { get; private set; }

        public ISOViewModel(ISerializableObject obj)
        {
            ISO = obj;
            Name = getName();
            Icon = getIcon();
        }

        public virtual Guid Rowguid
        {
            get { return ISO.Rowguid; }
        }          

        public static IISOViewModel fromISO(ISerializableObject obj)
        {
            if (obj != null)
            {
                if (obj is CollectionAgent)
                    return new AgentVM(obj as CollectionAgent);
                if (obj is CollectionEventLocalisation)
                    return new EventLocalisationVM(obj as CollectionEventLocalisation);
                if (obj is CollectionEventProperty)
                    return new EventPropertyVM(obj as CollectionEventProperty);
                if (obj is CollectionEventSeries)
                    return new EventSeriesVM(obj as CollectionEventSeries);
                if (obj is CollectionEvent)
                    return new EventVM(obj as CollectionEvent);
                if (obj is IdentificationUnitAnalysis)
                    return new IUAnalysisVM(obj as IdentificationUnitAnalysis);
                if (obj is IdentificationUnitGeoAnalysis)
                    return new IUGeoAnalysisVM(obj as IdentificationUnitGeoAnalysis);
                if (obj is IdentificationUnit)
                    return new IdentificationUnitVM(obj as IdentificationUnit);
                if (obj is CollectionSpecimen)
                    return new SpecimenVM(obj as CollectionSpecimen);

                return new DefaultVM(obj);


            }
            return null;
        }


        public string Name
        {
            get;
            private set;
        }

        public ISOIcon Icon
        {
            get;
            private set;
        }

        public virtual ISerializableObject Parent
        {
            get { return null; }
        } 

        public virtual IEnumerable<ISerializableObject> Properties
        {
            get { return Enumerable.Empty<ISerializableObject>(); }
        }

        public virtual IEnumerable<ISerializableObject> Children
        {
            get { return Enumerable.Empty<ISerializableObject>(); }
        }

        protected abstract string getName();

        protected abstract ISOIcon getIcon();

        protected static string formatAltitude(double alt)
        {
            return string.Format("{0} mNN", alt.ToString("F"));
        }

        protected static string formatLocalisation(double latitude, double longitude)
        {
            return string.Format("({0}{1};{2}{3})",
                           formatAsDegrees(latitude),
                           (latitude >= 0) ? "N" : "S",
                           formatAsDegrees(longitude),
                           (longitude >= 0) ? "E" : "W");
        }

        protected static string formatAsDegrees(double decimalAngle)
        {
            int deg = (int)decimalAngle;
            decimalAngle -= deg;
            decimalAngle *= 60;
            int min = (int)decimalAngle;
            decimalAngle -= min;
            decimalAngle *= 60;
            int sec = (int)decimalAngle;

            return string.Format("{0}° {1}' {2}''",
                deg,
                min,
                sec);
        }
    }
}
