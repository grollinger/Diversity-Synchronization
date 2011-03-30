using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public abstract partial class ISOViewModelBase : INotifyPropertyChanged
    {


        /// <summary>
        /// The <see cref="IsExpanded" /> property's name.
        /// </summary>
        public const string IsExpandedPropertyName = "IsExpanded";

        private bool _isExpanded = false;
        
        /// <summary>
        /// Gets the IsExpanded property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if (_isExpanded == value)
                {
                    return;
                }

                var oldValue = _isExpanded;
                _isExpanded = value;

                IsBusy = true;
                new Action(expandIfNecessary).BeginInvoke(
                    (res) =>
                    {
                        IsBusy = false;
                    }, null
                );

                // Update bindings, no broadcast
                RaisePropertyChanged(IsExpandedPropertyName);
            }
        }        

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

        /// <summary>
        /// Gets the IsBusy property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            protected set
            {
                if (_isBusy == value)
                {
                    return;
                }                

                _isBusy = value;           

                // Update bindings, no broadcast
                RaisePropertyChanged(IsBusyPropertyName);            
            }
        }

        /// <summary>
        /// The <see cref="IsGenerator" /> property's name.
        /// </summary>
        public const string IsGeneratorPropertyName = "IsGenerator";

        private bool _isGen = false;

        /// <summary>
        /// Gets the IsGenerator property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event.        
        /// </summary>
        public bool IsGenerator
        {
            get
            {
                return _isGen;
            }

            set
            {
                if (_isGen == value)
                {
                    return;
                }

                var wasGenerator = _isGen;
                _isGen = value;

                if (!_isGen && wasGenerator)
                    findRoot().pruneToGenerators();              

                // Update bindings, no broadcast
                RaisePropertyChanged(IsGeneratorPropertyName);
            }
        }

        internal ISOViewModelBase findRoot()
        {
            if (!isRoot())
                return _container.getViewModelForISO(Parent).findRoot();
            else
                return this;
        }

        

        public ISOIcon Icon {get; private set;}

        public string Name { get; private set; }

        /// <summary>
        /// The <see cref="Children" /> property's name.
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        private ICollection<ISOViewModelBase> _childVMs = new HashSet<ISOViewModelBase>();     

        /// <summary>
        /// Gets the ChildVMs property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<ISOViewModelBase> Children
        {
            get
            {
                foreach (var child in _childVMs)
                    yield return child;
            }
        }

        private ICollection<ISOViewModelBase> _propertyVMs = new List<ISOViewModelBase>();

        public virtual IEnumerable<ISOViewModelBase> Properties
        {
            get
            {
                return _propertyVMs;
            }
        }


        protected abstract ISerializableObject Parent
        {
            get;
        }

        private ISerializableObject _obj;
        protected ISerializableObject ISO
        {
            get { return _obj; }
        }

        protected abstract IEnumerable<ISerializableObject> propertyISOs
        {
            get;
        }

        protected abstract string getName();

        protected abstract ISOIcon getIcon();
        

        protected abstract IEnumerable<ISerializableObject> childISOs
        {
            get;
        }

        private void fillProperties()
        {           
            if(propertyISOs != null)
                foreach (var prop in propertyISOs)
                {
                    _propertyVMs.Add(_container.getViewModelForISO(prop));
                }
        }

        private ISOViewModelContainer _container;   

        public static ISOViewModelBase fromISO(ISerializableObject obj, ISOViewModelContainer container)
        {
            if (obj != null && container != null)
            {
                if(obj is CollectionEventSeries)
                    return new EventSeriesVM(obj as CollectionEventSeries,container);
                if (obj is CollectionEvent)
                    return new CollectionEventVM(obj as CollectionEvent, container);
                if (obj is CollectionSpecimen)
                    return new CollectionSpecimenVM(obj as CollectionSpecimen, container);
                if (obj is IdentificationUnitAnalysis)
                    return new IUAnalysisVM(obj as IdentificationUnitAnalysis, container);
                if (obj is IdentificationUnitGeoAnalysis)
                    return new IUGeoAnalysisVM(obj as IdentificationUnitGeoAnalysis, container);

                

                if (obj is IdentificationUnit)
                    return new IdentificationUnitVM(obj as IdentificationUnit, container);


                return new DefaultVM(obj, container);
                
                
            }
            return null;
        }

        internal ISOViewModelBase(ISerializableObject thisObj, ISOViewModelContainer container)
        {
            _obj = thisObj;
            _container = container;
            Name = getName();
            Icon = getIcon();

            if (!isRoot())
            {
                var pVM = _container.getViewModelForISO(Parent);
                pVM.injectChild(this);
            }

            fillProperties();
            
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }       

        /// <summary>
        /// Removes every unnecessary Node in this Subtree recursively
        /// </summary>
        /// <returns>wheter this Node is still necessary</returns>
        internal bool pruneToGenerators()
        {

            if (IsGenerator)
                return true;
            else
            {
                bool thisIsStillNeeded = false;
                IList<ISOViewModelBase> survivors = new List<ISOViewModelBase>();
                foreach (var child in _childVMs)
                {
                    if (child.pruneToGenerators())
                    {
                        survivors.Add(child);
                        thisIsStillNeeded = true;                        
                    }
                }
                if (thisIsStillNeeded)
                {
                    _childVMs = survivors;
                    notifyChildrenChanged();
                }
                else
                {
                    _container.removeFromSelection(ISO);                    
                }

                return thisIsStillNeeded;
            }
            
        }

        internal bool isRoot()
        {
            return Parent == null;
        }

        public bool shouldExpand()
        {
            if (IsGenerator)
                return true;
            else if(Parent != null)
            {
                ISOViewModelBase parentVM = _container.getViewModelForISO(Parent);                
                return parentVM.shouldExpand();                  
            }     
            return false;
        }

        protected void expandIfNecessary()
        {
            bool childrenChanged = false;
            if (shouldExpand())
            {
                foreach (var iso in childISOs)
                {
                    var childVM = _container.getViewModelForISO(iso);
                    if (!_childVMs.Contains(childVM))
                    {
                        childrenChanged = true;
                        _childVMs.Add(childVM);
                    }
                }
            }
            if (childrenChanged)
                notifyChildrenChanged();
        }

        internal void injectChild(ISOViewModelBase child)
        {
            if (!_childVMs.Contains(child))
            {
                _childVMs.Add(child);
                notifyChildrenChanged();
            }
        }

        public virtual ICollection<ISerializableObject> getSelection()
        {
            expandIfNecessary();
            ICollection<ISerializableObject> currentSelection = new HashSet<ISerializableObject>() { ISO };
            
            foreach (var child in _childVMs)
            {
                mergeInto(child.getSelection(), currentSelection);
            }
            if (Properties != null)
                foreach (var property in Properties)
                    mergeInto(property.getSelection(), currentSelection);

            return currentSelection;
        }

        private static void mergeInto(ICollection<ISerializableObject> source, ICollection<ISerializableObject> target)
        {
            foreach (var item in source)
                target.Add(item);
        }

        private void notifyChildrenChanged()
        {
            RaisePropertyChanged(ChildrenPropertyName); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
