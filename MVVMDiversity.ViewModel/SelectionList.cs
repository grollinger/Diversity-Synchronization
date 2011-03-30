using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MVVMDiversity.ViewModel
{
    public class SelectionList<T> : IEnumerable<SelectionItem<T>>
    {
        private Collection<SelectionItem<T>> _backingStore = new Collection<SelectionItem<T>>();

        #region Properties

        /// <summary>
        /// Returns the selected items in the list
        /// </summary>
        public IEnumerable<T> SelectedItems
        {
            get { return _backingStore.Where(x => x.IsSelected).Select(x => x.Item); }
        }

        /// <summary>
        /// Returns all the items in the SelectionList
        /// </summary>
        public IEnumerable<T> AllItems
        {
            get { return _backingStore.Select(x => x.Item); }
        }

        #endregion

        #region ctor

        public SelectionList(IEnumerable<T> col)
            
        {
            foreach (var item in col)
                Add(item);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var newItem = new SelectionItem<T>(item);
            newItem.SelectionChanged += (sender,args)=>
            {
                if(SelectionChanged != null && sender != null && sender is SelectionItem<T>)
                {
                    var sItem = (sender as SelectionItem<T>);
                    SelectionChanged(sItem.Item,sItem.IsSelected);
                }
            };
            _backingStore.Add(newItem);
        }

        /// <summary>
        /// Checks if the item exists in the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return AllItems.Contains(item);
        }

        /// <summary>
        /// Selects all the items in the list
        /// </summary>
        public void SelectAll()
        {
            foreach (SelectionItem<T> selectionItem in _backingStore)
            {
                selectionItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Unselects all the items in the list
        /// </summary>
        public void UnselectAll()
        {
            foreach (SelectionItem<T> selectionItem in _backingStore)
            {
                selectionItem.IsSelected = false;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates an SelectionList from any IEnumerable
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static IList<SelectionItem<T>> toSelectionItemList(IEnumerable<T> items)
        {
            List<SelectionItem<T>> list = new List<SelectionItem<T>>();
            foreach (T item in items)
            {
                SelectionItem<T> selectionItem = new SelectionItem<T>(item);
                list.Add(selectionItem);
            }
            return list;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<SelectionItem<T>> GetEnumerator()
        {
            return _backingStore.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        public delegate void SelectionChangedEventHandler(T sender, bool IsNowSelected);

        public event SelectionChangedEventHandler SelectionChanged;
        
    }

}
