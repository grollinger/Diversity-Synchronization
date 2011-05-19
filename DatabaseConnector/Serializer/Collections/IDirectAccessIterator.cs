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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections
{
    public interface IDirectAccessIterator<T> : IEnumerable<T> where T : ISerializableObject
    {
        T First();
        T Last();

        T Next();
        T Prev();

        void Reset();
        void Refresh();

        bool HasNext();
        bool HasPrev();

        void SetIteratorTo(T t);

        // bereinigte Position des Objects im Interator
        int CurrentPosition();

        // bereinigte Anzahl Objects im Interator
        int CountItems();

        T[] ToArray();
    }

    public interface IDirectAccessIteratorConfiguration {
        Serializer Serializer { set; }
        IRestriction Restriction { set; }
    }

    public class DirectAccesIteratorImpl<T> : IDirectAccessIterator<T>, IDirectAccessIteratorConfiguration where T : ISerializableObject 
    {
        private Serializer _serializer;
        private IRestriction _restriction;
        private int _readerPosition;
        private bool _positionUpdated = false;

        private IList<CsvPimaryKey> _keyList = null;

        
        public DirectAccesIteratorImpl()
        {
            _readerPosition = -1;
            
        }


        public Serializer Serializer { set { _serializer = value; } }

        public IRestriction Restriction { set { _restriction = value;  } }

        private void InitializeList() 
        {
            if (_keyList == null)
            {
                _keyList = _serializer.GetCSVKeys(typeof(T), this._restriction, _serializer.Connector.Transaction);
            }
        }

        private void TryExtendEnd()
        {
            InitializeList();

            IList<CsvPimaryKey> tmpList = _serializer.GetCSVKeys(typeof(T), this._restriction, _serializer.Connector.Transaction);

            foreach (CsvPimaryKey key in tmpList)
            {
                if (!this._keyList.Contains(key))
                {
                    this._keyList.Add(key);
                }
            }
        }

        public void Refresh()
        {
            TryExtendEnd();
        }

        public T First()
        {
            InitializeList();
            
            this._readerPosition = -1;

            try
            {
                return Next();
            } 
            catch(ArgumentOutOfRangeException) 
            {
                return default(T);
            }
        }

        public T Last()
        {
            InitializeList();
            TryExtendEnd();

            this._readerPosition = this._keyList.Count;

            try
            {
                return Prev();
            }
            catch (ArgumentOutOfRangeException)
            {
                return default(T);
            }
        }

        public T Next()
        {
            InitializeList();
            

            this._readerPosition++;

            if (this._readerPosition > this._keyList.Count - 1)
            {
                TryExtendEnd();
            }

            T ret = default(T);
            while (ret == null)
            {
                CsvPimaryKey key = this._keyList[this._readerPosition];
                IRestriction r = RestrictionFactory.CsvKeyRestriction(typeof(T), key);

                ret = this._serializer.Connector.Load<T>(r);

                if (ret == null)
                {
                    _readerPosition++;
                }
            }

            return ret;
        }

        public T Prev()
        {
            InitializeList();

            this._readerPosition--;

            T ret = default(T);
            while (ret == null)
            {
                CsvPimaryKey key = this._keyList[this._readerPosition];
                IRestriction r = RestrictionFactory.CsvKeyRestriction(typeof(T), key);

                ret = this._serializer.Connector.Load<T>(r);

                if (ret == null)
                {
                    _readerPosition--;
                }
            }

            return ret;
        }

        public bool HasPrev()
        {
            int tmp = this._readerPosition;
            try
            {
                Prev();
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            finally
            {
                this._readerPosition = tmp;
            }

            return true;
        }

        public bool HasNext()
        {
            int tmp = this._readerPosition;
            try
            {
                Next();
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            finally
            {
                this._readerPosition = tmp;
            }

            return true;
        }

        public void SetIteratorTo(T t)
        {
            InitializeList();
            TryExtendEnd();

            CsvPimaryKey key = new CsvPimaryKey(t, this._serializer.Target);

            int i = 0;
            foreach (CsvPimaryKey csv in this._keyList)
            {
                if (key.Equals(csv))
                {
                    this._readerPosition = i;
                    return;
                }
                i++;
            }
        }

        public void Reset()
        {
            _readerPosition = -1;
        }

        public int CurrentPosition()
        {
            int position = -1;

            T ret = default(T);
            try
            {
                CsvPimaryKey key1 = this._keyList[this._readerPosition];
                IRestriction restrict = RestrictionFactory.CsvKeyRestriction(typeof(T), key1);
                ret = this._serializer.Connector.Load<T>(restrict);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }

            foreach (CsvPimaryKey key in this._keyList)
            {
                IRestriction r = RestrictionFactory.CsvKeyRestriction(typeof(T), key);

                T tmp = this._serializer.Connector.Load<T>(r);

                if (tmp != null)
                {
                    position++;

                    if (tmp.Equals(ret))
                    {
                        return position + 1;
                    }
                }
            }

            return 0;
        }

        public int CountItems()
        {
            int position = -1;
            foreach (CsvPimaryKey key in this._keyList)
            {
                IRestriction r = RestrictionFactory.CsvKeyRestriction(typeof(T), key);

                T tmp = this._serializer.Connector.Load<T>(r);

                if (tmp != null)
                {
                    position++;
                }
            }

            return position + 1;
        }

        public T[] ToArray()
        {
            InitializeList();
            TryExtendEnd();

            List<T> tmpList = new List<T>();
            foreach(CsvPimaryKey key in this._keyList) 
            {
                IRestriction r = RestrictionFactory.CsvKeyRestriction(typeof(T), key);

                T tmp = this._serializer.Connector.Load<T>(r);

                if (tmp != null) 
                {
                    tmpList.Add(tmp);
                }
            }

            return tmpList.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorImpl<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorImpl<T>(this);
        }

        private class EnumeratorImpl<T> : IEnumerator<T> where T : ISerializableObject
        {
            private DirectAccesIteratorImpl<T> _it;
            private T _current;

            public EnumeratorImpl(DirectAccesIteratorImpl<T> owner)
            {
                _it = owner;
                _it.Reset();
            }

            public T Current
            {
                get
                {
                    return _current;
                }
            }

            Object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose() 
            {
                _it.Reset();
            }

            public bool MoveNext()
            {
                if (_it.HasNext())
                {
                    _current = _it.Next();
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }


        }
    }
}
