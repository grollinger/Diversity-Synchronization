using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.ViewModel
{
    public abstract partial class ISOViewModel : ViewModelBase
    {


        /// <summary>
        /// The <see cref="IsExpanded" /> property's name.
        /// </summary>
        public const string IsExpandedPropertyName = "IsExpanded";

        private bool _isExpanded = false;

        private bool _expandedBefore = false;

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

                if (fullExpandNeeded())
                {
                    _expandedBefore = true;
                    if (FirstExpand != null)
                    {
                        IsBusy = true;
                        new Action(() =>
                            {
                                FirstExpand(this);
                            }).BeginInvoke(
                            (res) => 
                            { 
                                IsBusy = false; 
                            }, null);
                    }
                    
                }

                // Verify Property Exists
                VerifyPropertyName(IsExpandedPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsExpandedPropertyName);
            }
        }

        private bool fullExpandNeeded()
        {
            return _isExpanded && (InheritedExpand || IsGenerator) && !_expandedBefore;
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


                // Verify Property Exists
                VerifyPropertyName(IsBusyPropertyName);

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

                var oldValue = _isGen;
                _isGen = value;

                if (!_isGen)
                    resetExpansion();


                // Verify Property Exists
                VerifyPropertyName(IsGeneratorPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(IsGeneratorPropertyName);
            }
        }

        //public ISOIcon Icon {get; protected set;}

        public string Name { get; protected set; }

        /// <summary>
        /// The <see cref="ChildVMs" /> property's name.
        /// </summary>
        public const string ChildVMsPropertyName = "ChildVMs";

        private NotifyingCollection<ISOViewModel> _childVMs;     

        /// <summary>
        /// Gets the ChildVMs property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<ISOViewModel> ChildVMs
        {
            get
            {
                foreach (var child in _childVMs)
                    yield return child;
            }
        }


        internal abstract ISerializableObject Parent
        {
            get;
        }

        internal abstract ISerializableObject ISO
        {
            get;
        }

        internal abstract IEnumerable<ISerializableObject> Children
        {
            get;
        }

        internal ICollection<ISOViewModel> ViewModelChildren { get { return _childVMs; } }
        

        internal delegate void FirstExpandEventHandler(ISOViewModel sender);

        internal event FirstExpandEventHandler FirstExpand;        

        private bool _inhExpand = false;

        /// <summary>
        /// Gets the InheritedExpand property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        internal bool InheritedExpand
        {
            get
            {
                return _inhExpand;
            }

            set
            {
                if (_inhExpand == value)
                {
                    return;
                }
                
                _inhExpand = value;

                if (!_inhExpand)
                    resetExpansion();                
            }
        }

        private void resetExpansion()
        {
            _expandedBefore = false;
        }

        

        

        public static ISOViewModel fromISO(ISerializableObject obj)
        {
            if (obj != null)
            {
                if(obj is CollectionEventSeries)
                    return new EventSeriesVM(obj as CollectionEventSeries);
                if (obj is IdentificationUnit)
                    return new IdentificationUnitVM(obj as IdentificationUnit);

                return new MockISOVM();
                
            }
            return null;
        }

        internal ISOViewModel()
        {
            _childVMs = new NotifyingCollection<ISOViewModel>();
            _childVMs.CollectionChanged += (sender)=>
            {
                VerifyPropertyName(ChildVMsPropertyName);
                RaisePropertyChanged(ChildVMsPropertyName);
            };
        }

        
        
    }
}
