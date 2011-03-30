using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MVVMDiversity.ViewModel
{
    public class NotifyingCollection<T> : Collection<T>
    {
        protected override void ClearItems()
        {
            base.ClearItems();
            RaiseCollectionChanged();
        }
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            RaiseCollectionChanged();
        }
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            RaiseCollectionChanged();
        }
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            RaiseCollectionChanged();
        }

        private void RaiseCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this);
        }

        public delegate void CollectionChangedEventHandler(Collection<T> sender);

        public event CollectionChangedEventHandler CollectionChanged;
    }
}
