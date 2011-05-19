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
    public class GenericWeakReferenceSet<T> : IEnumerable<T>
    {
        private List<GenericWeakReference<T>> _referencedObjects = new List<GenericWeakReference<T>>();
        private List<IEnumerator> _enumerators = new List<IEnumerator>();
        public void Add(T element)
        {
            InvalidateEnumerators();

            List<GenericWeakReference<T>> remove = new List<GenericWeakReference<T>>();

            foreach (GenericWeakReference<T> r in _referencedObjects)
            {
                T t = r.Target;

                if (t != null)
                {
                    if (Object.ReferenceEquals(t, element)) return;
                }
                else
                {
                    remove.Add(r);
                }
            }

            _referencedObjects.Add(new GenericWeakReference<T>(element));
            

            foreach (GenericWeakReference<T> r in remove)
            {
                _referencedObjects.Remove(r);
            }
        }

        public bool Remove(T element)
        {
            InvalidateEnumerators();

            foreach (GenericWeakReference<T> r in _referencedObjects)
            {
                T t = r.Target;

                if (t != null)
                {
                    if (Object.ReferenceEquals(t, element))
                    {
                        return _referencedObjects.Remove(r);
                    }
                }
            }

            return false;
        }

        public int Count { get { return _referencedObjects.Count; } }

        public bool Contains(T element) {
            foreach (GenericWeakReference<T> r in _referencedObjects)
            {
                T t = r.Target;
                if (t != null)
                {
                    if (Object.ReferenceEquals(element, t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public T this[int index] 
        {
            get 
            {
                return _referencedObjects[index].Target;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> e = new EnumeratorImpl<T>(this);
            _enumerators.Add(e);
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RemoveEnumerator(IEnumerator e)
        {
            _enumerators.Remove(e);
        }

        private void InvalidateEnumerators()
        {
            foreach (IEnumerator e in _enumerators)
            {
                ((IInvalidateable)e).Invalidate();
            }
        }

        private interface IInvalidateable
        {
            void Invalidate();
        }

        private class EnumeratorImpl<TI> : IEnumerator<TI>, IInvalidateable
        {
            private GenericWeakReferenceSet<TI> _backingSet;
            private int _currentIndex = -1;
            private TI _current = default(TI);
            private bool _invalid = false;

            public EnumeratorImpl(GenericWeakReferenceSet<TI> set) 
            {
                _backingSet = set;
            }

            public bool MoveNext()
            {
                if (_invalid)
                {
                    throw new InvalidOperationException();
                }

                _currentIndex++;
                for (; _currentIndex < _backingSet.Count; _currentIndex++)
                {
                    _current = _backingSet[_currentIndex];
                    if (_current != null) return true;
                }

                return false;
            }

            public void Reset()
            {
                _invalid = false;
                _currentIndex = -1;
                _current = default(TI);
            }

            public TI Current
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
                _current = default(TI);
                _backingSet.RemoveEnumerator(this);
                _backingSet = null;
            }

            public void Invalidate()
            {
                _invalid = true;  
            }
        }
    }
}
