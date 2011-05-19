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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util
{
    public interface IObeservable
    {
        void AddObserver(IObeserver observer);
        void RemoveObserver(IObeserver observer);
        void RemoveAll();
        void NotifyObservers(Object message);
    }

    public class ObservableDelegate : IObeservable
    {
        private IObeservable _owner;
        private List<IObeserver> _observers = new List<IObeserver>();


        public ObservableDelegate(IObeservable observable)
        {
            _owner = observable;
        }

        public void AddObserver(IObeserver o)
        {
            _observers.Add(o);
        }

        public void RemoveObserver(IObeserver o)
        {
            _observers.Remove(o);
        }

        public void RemoveAll()
        {
            _observers.Clear();
        }

        public void NotifyObservers(Object message) {
            foreach (IObeserver o in _observers)
            {
                o.Update(_owner, message);
            }
        }
    }
}
