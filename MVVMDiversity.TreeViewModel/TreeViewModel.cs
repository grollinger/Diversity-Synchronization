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
using MVVMDiversity.Interface;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using System.Collections.ObjectModel;

namespace MVVMDiversity.ViewModel
{
  

    public partial class TreeViewModel : ITreeViewModel
    {
        private List<NodeViewModel> _roots;
        public IEnumerable<INodeViewModel> Roots 
        { 
            get 
            {
                foreach(var root in _roots)
                    yield return root;
            } 
        }

        private bool _truncate = false;
        public bool TruncateDataItems
        {
            get
            {
                return _truncate;
            }
            set
            {
                if (_truncate != value)
                {
                    _truncate = value;

                    if (_truncate)
                        foreach (var root in _roots)
                            trimBranchContaining(root);
                    else
                        foreach (var gen in _generators)
                            gen.expandIfNecessary();

                    raiseTruncateChanged();
                }
            }
        }
        
        private bool _updatesSuspended = false;
        
        private IISOViewModelStore _store;

        private Dictionary<Guid,NodeViewModel> _nodes = new Dictionary<Guid,NodeViewModel>();

        private HashSet<NodeViewModel> _generators;


        public TreeViewModel(IISOViewModelStore store)
        {
            _roots = new List<NodeViewModel>();
            _store = store;
            _generators = new HashSet<NodeViewModel>();
        }

        public virtual void addGenerator(IISOViewModel vm)
        {            
            var genNode = addOrRetrieveNode(vm);
            _generators.Add(genNode);
            genNode.IsGenerator = true;

            addParents(vm);            
        }

        public virtual void removeGenerator(IISOViewModel vm)
        {
            NodeViewModel node = null;
            if (_nodes.TryGetValue(vm.Rowguid, out node))
            {
                _generators.Remove(node);
                node.IsGenerator = false;
                if(!node.isNecessary())
                    trimBranchContaining(node);
            }
        }

        ICollection<ISerializableObject> _selection;

        public event SelectionBuildProgressChangedHandler SelectionBuildProgressChanged;       

        public virtual IList<ISerializableObject> buildSelection()
        {
            int rootsCount = _roots.Count;
            int currentRoot = 0;
            _selection = new HashSet<ISerializableObject>();
            progressChanged(rootsCount, currentRoot, null);
            foreach (var root in _roots)
            {               
                recursiveAddNodeToSelection(root);
                progressChanged(rootsCount, ++currentRoot, root);
            }
            return _selection.ToList<ISerializableObject>();
        }

        

        private void progressChanged(int roots, int currentRoot, NodeViewModel root)
        {
            if (SelectionBuildProgressChanged != null)
                SelectionBuildProgressChanged(roots, currentRoot, (root!=null)?root.VM:null);
        }

        private void recursiveAddNodeToSelection(NodeViewModel node)
        {
            //If this is a Generator, we can add all the ISOs below
            if (node.IsGenerator && !TruncateDataItems)
                recursiveAddToSelection(node.VM);
            else // we need to add only the injected ones
            {                
                _selection.Add(node.VM.ISO);
                addPropertiesToSelection(node.VM);
                foreach (var childNode in node.Children)
                    recursiveAddNodeToSelection(childNode);
            }
        }

        private void addPropertiesToSelection(IISOViewModel vm)
        {
            if (vm.Properties != null)
            {
                foreach (var property in vm.Properties)
                    _selection.Add(property);
            }
        }

        private void recursiveAddToSelection(IISOViewModel vm)
        {
            _selection.Add(vm.ISO);
            addPropertiesToSelection(vm);

            foreach (var child in vm.Children)
            {
                var childVM = addOrRetrieveISOVM(child);
                recursiveAddToSelection(childVM);
            }
        }


        private void addParents(IISOViewModel vm)
        {
            Stack<IISOViewModel> familyLine = new Stack<IISOViewModel>();
            while (!isRoot(vm))
            {
                familyLine.Push(vm);
                vm = addOrRetrieveISOVM(vm.Parent);
            }

            NodeViewModel rootNode = addOrRetrieveNode(vm);
            addRoot(rootNode);

            var currentNode = rootNode;
            while (familyLine.Count > 0)
            {
                var childNode = addOrRetrieveNode(familyLine.Pop());
                currentNode.injectChild(childNode);
                currentNode.IsExpanded = true;
                currentNode = childNode;
            }
        }

        private NodeViewModel addOrRetrieveNode(IISOViewModel vm)
        {
            NodeViewModel node = null;
            if (!_nodes.TryGetValue(vm.Rowguid, out node))
            {
                node = new NodeViewModel(vm, this);
                _nodes.Add(vm.Rowguid, node);
            }

            return node;
        }

        private void trimBranchContaining(NodeViewModel node)
        {
            NodeViewModel root = findRoot(node.VM);
            root.removeSuperfluousChildren();
            if (!root.isNecessary())
                removeRoot(root);
        }

        private NodeViewModel findRoot(IISOViewModel vm)
        {
            while (!isRoot(vm))
                vm = addOrRetrieveISOVM(vm.Parent);
            return addOrRetrieveNode(vm);
        }

        private bool isRoot(IISOViewModel vm)
        {
            return vm.Parent == null;
        }

        private IISOViewModel addOrRetrieveISOVM(ISerializableObject iso)
        {
            return _store.addOrRetrieveVMForISO(iso);
        }

        private void addRoot(NodeViewModel root)
        {
            if (!_roots.Contains(root))
            {
                _roots.Add(root);
                raiseRootsChanged();
            }
        }
        private void removeRoot(NodeViewModel root)
        {
            if (_roots.Contains(root))
            {
                _roots.Remove(root);
                raiseRootsChanged();
            }
        }


        private void raiseRootsChanged()
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Roots"));

        }

        private void raiseTruncateChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("TruncateDataItems"));
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


        
    }
}
