using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
    public class SelectionTreeViewModel : ViewModelBase
    {
        private Dictionary<Guid, ISOViewModel> _selection = new Dictionary<Guid, ISOViewModel>();

        private bool _truncateAtGenerators = false;

        /// <summary>
        /// The <see cref="Roots" /> property's name.
        /// </summary>
        public const string RootsPropertyName = "Roots";

        private HashSet<ISOViewModel> _generators = new HashSet<ISOViewModel>();
        private HashSet<ISOViewModel> _roots = new HashSet<ISOViewModel>();

        /// <summary>
        /// Gets the Roots property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<ISOViewModel> Roots
        {
            get
            {
                var m = new MockISOVM();
                m.ViewModelChildren.Add(new MockISOVM());
                return new List<ISOViewModel>() {m };

                /*/new Iterator each time or else the binding won't update.
                foreach (var root in _roots)
                    yield return root;//*/
            }            
        }

        public ICollection<ISOViewModel> Generators
        {
            get
            { 
                return _generators; 
            }
        }

        private void RootsChanged()
        {
            VerifyPropertyName(RootsPropertyName);
            RaisePropertyChanged(RootsPropertyName);
        }

        public void addGenerator(ISerializableObject gen)
        {
            ISOViewModel genVM = getViewModelForISO(gen);

            if(genVM != null && !_generators.Contains(genVM))
            {
                genVM.IsGenerator = true;

                _generators.Add(genVM);

                addParents(genVM);



            }            
        }
         //TODO Thread-Safety nötig?
        private ISOViewModel getViewModelForISO(ISerializableObject gen)
        {
            ISOViewModel genVM = null;
            if (!_selection.TryGetValue(gen.Rowguid, out genVM))
            {
                genVM = ISOViewModel.fromISO(gen);
                genVM.FirstExpand += expandNode;
                _selection.Add(gen.Rowguid, genVM);
            }
            return genVM;
        }

        private void addParents(ISOViewModel genVM)
        {
            if (genVM.Parent == null)
            {
                if (!_roots.Contains(genVM))
                {
                    _roots.Add(genVM);                    
                    RootsChanged();
                }
            }
            else
            {
                var parentVM = getViewModelForISO(genVM.Parent);
                addParents(parentVM);
            }
        }

        public void removeGenerator(ISerializableObject gen)
        {

        }

        public IList<ISerializableObject> compileSelection()
        {
            return null;
        }

        private void expandNode(ISOViewModel sender)
        {
            foreach (var iso in sender.Children)
            {
                var isoVM = getViewModelForISO(iso);
                isoVM.InheritedExpand = true;
                if (!sender.ViewModelChildren.Contains(isoVM))
                    sender.ViewModelChildren.Add(isoVM); 

            }
        }


        public SelectionTreeViewModel()
        {

        }
    }
}
