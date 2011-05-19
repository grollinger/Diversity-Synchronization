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
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class Connector : IObeservable
    {
        public const String ROLLBACK_MESSAGE = "rollback";
        public const String BEGIN_TRANSACTION_MESSAGE = "begin";
        public const String COMMIT_MESSAGE = "commit";


        private Serializer _serializer;

        
        private ISerializerTransaction _transaction;
        private ObservableDelegate _delegate;

        internal Connector(Serializer serializer)
        {
            _serializer = serializer;
            _delegate = new ObservableDelegate(this);
            _transaction = new SerializerTransactionDummy();
        }

        internal Serializer ConnectedSerializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        internal ISerializerTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }

        public void BeginTransaction()
        {
            if (!(_transaction is SerializerTransactionDummy))
            {
                throw new InvalidOperationException("a transaction is already started, commit first.");
            }
           
            _transaction = new SerializerTransaction(_serializer.DBConnection.BeginTransaction());

            NotifyObservers(BEGIN_TRANSACTION_MESSAGE);
        }

        public void Commit()
        {
            NotifyObservers(COMMIT_MESSAGE);
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = new SerializerTransactionDummy();
        }

        public void Rollback()
        {
            NotifyObservers(ROLLBACK_MESSAGE);
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = new SerializerTransactionDummy();
        }


        public String Target
        {
            get { return _serializer.Target; }
        }

        


        //LoadList


        public IList<ISerializableObject> LoadList(Type t, IRestriction restriction)
        {
            return _serializer.LoadList(t, restriction, _transaction);
        }

        //Erwartet ein Select Statement, das für den übergebenen Typ passende Objekte liefert.
        //Nur verwenden, wenn man ganz sicher ist was man tut.
        public IList<ISerializableObject> LoadList(Type t, String sqlString)
        {
            if (sqlString.ToUpper().StartsWith("SELECT") == false)
            {
                Exception ex = new Exception();
                throw ex;
            }
            return _serializer.LoadList(t, sqlString, _transaction);
        }

        //Hier ist zu entscheiden, ob mit LazyLoading gearbeitet werden soll. Evtl. ist auch ein nicht lazy Weg zu implementieren.
        public IList<ISerializableObject> LoadList(Type t,string table, IRestriction restriction)
        {
            return _serializer.LoadList(t,table, restriction, _transaction);
        }


        public IList<T> LoadList<T>(IRestriction restriction) where T : ISerializableObject
        {
            return _serializer.LoadList<T>(restriction, _transaction);
        }

        public IList<T> LoadList<T>(String sql) where T : ISerializableObject
        {
            return _serializer.LoadList<T>(sql, _transaction);
        }

        public IList<T> LoadList<T>(IRestriction restriction, string table) where T : ISerializableObject
        {
            return _serializer.LoadList<T>(restriction, _transaction,table);
        }

        //public IList<T> Select<T>(String sqlString) where T:Type
        //{
        //    return _serializer.Select<T>(sqlString, _transaction);
        //}

        //Load

        public T Load<T>(IRestriction restriction) where T : ISerializableObject
        {
            return (T)_serializer.Load(typeof(T), restriction, _transaction);
        }

        public ISerializableObject Load(Type t, IRestriction restriction)
        {
            return _serializer.Load(t, restriction, _transaction);
        }

        public ISerializableObject Load(Type t,string table, IRestriction restriction)
        {
            return _serializer.Load(t,table, restriction, _transaction);
        }

        //Select What FROM Where
        //Erlaubt eindimensionale Ausführung eines einfachen SELECT statements
        public IList<String> selectWhatFromWhere<String>(string what, string from, string where, string value)
        {
            if (what.Contains(","))
            {
                Exception ex = new Exception("Selection is supportet only one-dimensional");
                throw ex;
            }
            return _serializer.selectWhatFromWhere<String>(what, from, where, value, _transaction);
        }

        public void Save(ISerializableObject iso)
        {
            iso.LogTime = DateTime.Now;
            _serializer.Save(iso, _transaction);
        }

        public void Delete(ISerializableObject iso)
        {
            _serializer.Delete(iso, _transaction);
        }

        public void InsertPlain(ISerializableObject iso)
        {
            iso.LogTime = DateTime.Now;
            _serializer.InsertPlain(iso, _transaction);
        }

        public void InsertPlain(ISerializableObject iso,string table)
        {
            iso.LogTime = DateTime.Now;
            _serializer.InsertPlain(iso, _transaction,table);
        }


        public void UpdatePlain(ISerializableObject iso)
        {
            iso.LogTime = DateTime.Now;
            _serializer.UpdatePlain(iso, _transaction);
        }

        public void Truncate(String table)
        {
            _serializer.Truncate(table, _transaction);
        }

        public bool Exists(String column, Object value, String table)
        {
            return _serializer.Exists(column, value, table,_transaction);
        }

        public bool IsManaged(ISerializableObject iso)
        {
            return _serializer.IsManaged(iso);
        }

        #region observable

        public void AddObserver(IObeserver o)
        {
            _delegate.AddObserver(o);
        }

        public void RemoveObserver(IObeserver o)
        {
            _delegate.RemoveObserver(o);
        }

        public void RemoveAll()
        {
            _delegate.RemoveAll();
        }

        public void NotifyObservers(Object message)
        {
            _delegate.NotifyObservers(message);
        }

        #endregion
    }
}
