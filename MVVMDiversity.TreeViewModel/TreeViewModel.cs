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

        
        private IISOViewModelStore _store;

        private Dictionary<Guid,NodeViewModel> _nodes = new Dictionary<Guid,NodeViewModel>();

        private HashSet<IISOViewModel> _generators;


        public TreeViewModel(IISOViewModelStore store)
        {
            _roots = new List<NodeViewModel>();
            _store = store;
            _generators = new HashSet<IISOViewModel>();
        }

        public void addGenerator(IISOViewModel vm)
        {            
            var genNode = addOrRetrieveNode(vm);
            genNode.IsGenerator = true;

            addParents(vm);            
        }

        public void removeGenerator(IISOViewModel vm)
        {
            NodeViewModel node = null;
            if (_nodes.TryGetValue(vm.Rowguid, out node))
            {
                node.IsGenerator = false;
                if(!node.isNecessary())
                    trimBranchContaining(node);
            }
        }

        ICollection<ISerializableObject> _selection;

        public IList<ISerializableObject> buildSelection()
        {
            _selection = new Collection<ISerializableObject>(); 
            foreach (var root in _roots)
            {
                recursiveAddToSelection(root);
            }
            return _selection.ToList<ISerializableObject>();
        }

        private void recursiveAddToSelection(NodeViewModel node)
        {
            //If this is a Generator, we can add all the ISOs below
            if (node.IsGenerator)
                recursiveAddToSelection(node.VM);
            else // we need to add only the injected ones
            {                
                _selection.Add(node.VM.ISO);
                addPropertiesToSelection(node.VM);
                foreach (var childNode in node.Children)
                    recursiveAddToSelection(childNode);
            }
        }

        private void addPropertiesToSelection(IISOViewModel vm)
        {
            foreach (var property in vm.Properties)
                _selection.Add(property);
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
                rootsChanged();
            }
        }
        private void removeRoot(NodeViewModel root)
        {
            if (_roots.Contains(root))
            {
                _roots.Remove(root);
                rootsChanged();
            }
        }


        private void rootsChanged()
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Roots"));

        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
