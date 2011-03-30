﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MVVMDiversity.ViewModel
{
    public class SelectionList<T> :
    ObservableCollection<SelectionItem<T>> where T : IComparable<T>
    {
        #region Properties

        /// <summary>
        /// Returns the selected items in the list
        /// </summary>
        public IEnumerable<T> SelectedItems
        {
            get { return this.Where(x => x.IsSelected).Select(x => x.Item); }
        }

        /// <summary>
        /// Returns all the items in the SelectionList
        /// </summary>
        public IEnumerable<T> AllItems
        {
            get { return this.Select(x => x.Item); }
        }

        #endregion

        #region ctor

        public SelectionList(IEnumerable<T> col)
            : base(toSelectionItemEnumerable(col))
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int i = 0;
            foreach (T existingItem in AllItems)
            {
                if (item.CompareTo(existingItem) < 0) break;
                i++;
            }
            Insert(i, new SelectionItem<T>(item));
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
            foreach (SelectionItem<T> selectionItem in this)
            {
                selectionItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Unselects all the items in the list
        /// </summary>
        public void UnselectAll()
        {
            foreach (SelectionItem<T> selectionItem in this)
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
        private static IEnumerable<SelectionItem<T>> toSelectionItemEnumerable(IEnumerable<T> items)
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
    }
}
