//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util
{
    public class FastSearchDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private List<KeyValuePair<TKey, TValue>> _list;
        private IComparer _comparer;

        public FastSearchDictionary(IComparer comparer)
        {
            _comparer = comparer;
            _list = new List<KeyValuePair<TKey, TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int i = BinaryInsertIndexSearch(key);

            if (_list.Count > i && _comparer.Compare(_list[i].Key, key) == 0)
            {
                throw new ArgumentException();
            }

            _list.Insert(i, new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException();
            return BinaryIndexSearch(key) != -1;
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            int i = BinaryIndexSearch(pair.Key);
            if (i == -1) return false;
            return _list[i].Equals(pair);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (Count > array.Length - index)
            {
                throw new ArgumentException();
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            foreach (KeyValuePair<TKey, TValue> p in _list)
            {
                array[index] = p;
                index++;
            }
        }

        public int Count { get { return _list.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException();
            int i = BinaryIndexSearch(key);
            if (i == -1) return false;
            _list.RemoveAt(i);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            int i = BinaryIndexSearch(pair.Key);
            if (i == -1) return false;
            if (_list[i].Equals(pair))
            {
                _list.RemoveAt(i);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException();

            int i = BinaryIndexSearch(key);
            if (i == -1)
            {
                value = default(TValue);
                return false;
            }
            else
            {
                value = _list[i].Value;
                return true;
            }
        }

        public void Clear() { _list.Clear(); }

        public ICollection<TKey> Keys
        {
            get
            {
                List<TKey> ret = new List<TKey>(_list.Count);

                foreach (KeyValuePair<TKey, TValue> p in _list)
                {
                    ret.Add(p.Key);
                }
                return ret;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue ret;
                bool b = TryGetValue(key, out ret);
                if (b == false)
                {
                    throw new KeyNotFoundException();
                }
                return ret;
            }
            set
            {
                OverrideKey(key, value);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> ret = new List<TValue>(_list.Count);

                foreach (KeyValuePair<TKey, TValue> p in _list)
                {
                    ret.Add(p.Value);
                }
                return ret;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int BinaryIndexSearch(TKey key)
        {
            return BinaryIndexSearch(key, 0, _list.Count - 1);
        }

        private int BinaryIndexSearch(TKey key, int l, int r)
        {
            if (_list.Count == 0)
            {
                return -1;
            }
            int m = (l + r) / 2;
            if (l > r)
            {
                return -1;
            }
            else
            {
                TKey tmp = _list[m].Key;
                if (_comparer.Compare(key, tmp) < 0)
                {
                    return BinaryIndexSearch(key, l, m - 1);
                }
                else if (_comparer.Compare(key, tmp) > 0)
                {
                    return BinaryIndexSearch(key, m + 1, r);
                }
                else
                {
                    return m;
                }
            }
        }

        private void OverrideKey(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int i = BinaryInsertIndexSearch(key);
            if (_list.Count > i && key.Equals(_list[i].Key))
            {
                _list[i] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                _list.Insert(i, new KeyValuePair<TKey,TValue>(key, value));
            }
        }

        public int BinaryInsertIndexSearch(TKey key)
        {
            return BinaryInsertIndexSearch(key, 0, _list.Count - 1);
        }

        private int BinaryInsertIndexSearch(TKey key, int l, int r)
        {
            if (_list.Count == 0)
            {
                return 0;
            }
            int m = (l + r) / 2;
            if (l > r)
            {
                if (_comparer.Compare(key, _list[m].Key) < 0)
                {
                    return m;
                }
                return m + 1;
            }
            else
            {
                TKey tmp = _list[m].Key;
                if (_comparer.Compare(key, tmp) < 0)
                {
                    return BinaryInsertIndexSearch(key, l, m - 1);
                }
                else if (_comparer.Compare(key, tmp) > 0)
                {
                    return BinaryInsertIndexSearch(key, m + 1, r);
                }
                else
                {
                    return m;
                }
            }
        }
    }
}
