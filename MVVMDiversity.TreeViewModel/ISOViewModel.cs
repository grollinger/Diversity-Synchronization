using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Interface;

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
                if (obj is CollectionEventSeries)
                    return new EventSeriesVM(obj as CollectionEventSeries);
                if (obj is CollectionEvent)
                    return new CollectionEventVM(obj as CollectionEvent);
                if (obj is CollectionSpecimen)
                    return new CollectionSpecimenVM(obj as CollectionSpecimen);
                if (obj is IdentificationUnitAnalysis)
                    return new IUAnalysisVM(obj as IdentificationUnitAnalysis);
                if (obj is IdentificationUnitGeoAnalysis)
                    return new IUGeoAnalysisVM(obj as IdentificationUnitGeoAnalysis);



                if (obj is IdentificationUnit)
                    return new IdentificationUnitVM(obj as IdentificationUnit);


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

        public abstract ISerializableObject Parent
        {
            get;
        } 

        public abstract IEnumerable<ISerializableObject> Properties
        {
            get;
        }

        public abstract IEnumerable<ISerializableObject> Children
        {
            get;
        }

        protected abstract string getName();

        protected abstract ISOIcon getIcon();
    }
}
