using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using System.ComponentModel;

namespace MVVMDiversity.ViewModel
{
    public partial class TreeViewModel
    {
        private class NodeViewModel : INodeViewModel
        {
            IISOViewModel _vm;
            TreeViewModel _owner;
            public NodeViewModel(IISOViewModel vm, TreeViewModel owner)
            {
                _vm = vm;
                _owner = owner;
                if(_vm.Properties != null)
                    foreach (var property in _vm.Properties)
                    {
                        _propertyVMs.Add(_owner.addOrRetrieveISOVM(property));
                    }
            }

            #region Properties

            public string Name
            {
                get { return _vm.Name; }
            }

            public ISOIcon Icon
            {
                get { return _vm.Icon; }
            }           

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

                    expandIfNecessary();                   

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
                    _isGen = value;

                    propagateGenerator();
                    expandIfNecessary();
                    expandIfNecessary();
                                     

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IsGeneratorPropertyName);
                }
            }

            
            
            /// <summary>
            /// The <see cref="Children" /> property's name.
            /// </summary>
            public const string ChildrenPropertyName = "Children";

            private IList<NodeViewModel> _childNodes = new List<NodeViewModel>();     

            /// <summary>
            /// Gets the ChildVMs property.
            /// TODO Update documentation:
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public IEnumerable<NodeViewModel> Children
            {
                get
                {
                    foreach (var child in _childNodes)
                        yield return child;
                }
            }

            private ICollection<IISOViewModel> _propertyVMs = new List<IISOViewModel>();

            public virtual IEnumerable<IISOViewModel> Properties
            {
                get
                {
                    return _propertyVMs;
                }
            }

            private bool _belowGen = false;
                

            internal bool BelowGenerator 
            {
                get
                {
                    return _belowGen;
                }
                set
                {
                    if (_belowGen != value)
                    {
                        _belowGen = value;
                        propagateGenerator();
                        expandIfNecessary();

                    }
                }
            }

           

            internal IISOViewModel VM { get { return _vm; } }
            #endregion

            private void expandIfNecessary()
            {
                if (IsExpanded)
                    performExpansion();
            }

            private void propagateGenerator()
            {
                var shouldExpand = BelowOrIsGenerator();

                foreach (var childNode in _childNodes)
                {
                    childNode.BelowGenerator = shouldExpand;
                }
            }

            /// <summary>
            /// Removes every unnecessary Node in this Subtree recursively
            /// </summary>
            /// <returns>wheter this Node is still necessary</returns>
            internal bool isNecessary()
            {

                if (BelowOrIsGenerator())
                    return true;
                else
                {
                    removeSuperfluousChildren();
                    return _childNodes.Count > 0;
                }
            
            }           

            private void removeSuperfluousChildren()
            {
                IList<NodeViewModel> survivors = new List<NodeViewModel>();
                foreach (var child in _childNodes)
                {
                    if (child.isNecessary())
                    {
                        survivors.Add(child);
                    }
                }
                if (_childNodes.Count != survivors.Count)
                {
                    _childNodes = survivors;
                    notifyChildrenChanged();
                }
            }       

            public bool BelowOrIsGenerator()
            {
                return IsGenerator || BelowGenerator;
            }

            protected void performExpansion()
            {
                bool childrenChanged = false;
                if (BelowOrIsGenerator())
                {
                    foreach (var iso in _vm.Children)
                    {
                        var childVM = _owner.addOrRetrieveISOVM(iso);
                        var childNode = _owner.addOrRetrieveNode(childVM);
                        if (!_childNodes.Contains(childNode))
                        {
                            childrenChanged = true;
                            _childNodes.Add(childNode);
                        }
                    }
                }
                if (childrenChanged)
                    notifyChildrenChanged();
            }

            internal void injectChild(NodeViewModel child)
            {
                if (!_childNodes.Contains(child))
                {
                    _childNodes.Add(child);
                    child.BelowGenerator = this.BelowOrIsGenerator();

                    notifyChildrenChanged();
                }
            }
            /*
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
            }*/

            private void notifyChildrenChanged()
            {
                RaisePropertyChanged(ChildrenPropertyName); 
            }

            #region INPC

            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }  

            public event PropertyChangedEventHandler PropertyChanged;
            #endregion

            

            IEnumerable<INodeViewModel> INodeViewModel.Children
            {
                get { return Children as IEnumerable<INodeViewModel>; }
            }
        }
    }
}
