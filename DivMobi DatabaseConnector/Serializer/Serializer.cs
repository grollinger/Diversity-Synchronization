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
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Exceptions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Relations;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using System.Runtime.CompilerServices;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class UpdateEventArgs : EventArgs
    {
        UpdateStates _updateState = UpdateStates.NO_ROWS_AFFECTED;

        internal UpdateEventArgs(UpdateStates state)
        {
            _updateState = state;
        }

        public UpdateStates UpdateState { get { return _updateState; } }
    }

    public delegate void UpdateEventHandler(Object sender, UpdateEventArgs args);

    public abstract class Serializer
    {
        private DbConnection _connection = null;
        private string _target;
        protected string _praefix = String.Empty;
        private List<Type> _registeredTypes;
        private IAdvanceProgress _ipro;
        private ISerializableObjectPool _objectPool;
        private Connector _currentConnector;
        


        protected Serializer()
        {
            _registeredTypes = new List<Type>();
        }

        #region connection

        public void RegisterType(Type t)
        {
            FieldInfo f = AttributeWorker.RowGuid(t);
            if (f == null)
            {
                throw new TypeDefException("Type "+t+" has no row guid column!");
            }

            _registeredTypes.Add(t);
        }

        public void RegisterTypes(IList<Type> list)
        {
            foreach (Type t in list)
            {
                RegisterType(t);
            }
        }

        internal IList<Type> RegisteredTypes
        {
            get { return _registeredTypes; }
        }

        public void Activate(string target)
        {
            
            _target = target;
            _connection = (DbConnection)CreateConnection();
            if(_connection.State == ConnectionState.Closed ||  _connection.State==ConnectionState.Broken)
            {
                try
                {
                    _connection.Close();
                }
                catch(Exception ex)
                {
                }   
            }
            _connection.Open();

            //Initialisierung des AttributeWorkers ...
            AttributeWorker.GetInstance(Target);
            VirtualKeyFactory vkf = new VirtualKeyFactory(AttributeWorker.GetInstance(target));
            foreach (Type t in RegisteredTypes)
            {
                 vkf.RegisterType(t);
            }
            
            AttributeWorker.GetInstance(Target).AddVirtualKeyFactory(this, vkf);

            _objectPool = new SortedSerializableObjectPool(this, _registeredTypes);
            ((SortedSerializableObjectPool)_objectPool).Cleaner = new DefaultCleaner();
            
        }

        public void Activate()
        {
            this.Activate(AttributeConstants.DEFAULT_TARGET);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        internal DbConnection DBConnection
        {
            get { return _connection; }
        }

        public abstract DbConnection CreateConnection();



        #endregion

        #region NEW

        private const String SELECT = "SELECT * FROM ";
        private const String COUNT = "SELECT COUNT(*) FROM ";
        private const String WHERE = " WHERE ";

        //Erzeugt eine SQL-Anfrage indem der Tabellenname gleich der Klasse ist oder als Attribut gespeichert wird.
        protected virtual String GenerateSelectString(Type type, IRestriction restriction)
        {
            string tableName = AttributeWorker.GetInstance(this.Target).GetTableMapping(type,_praefix);
            return GenerateSelectString(type, tableName, restriction);
        }
        //Erzeugt eine SQL Anfrage in der der Tabellenname übergeben wird
        protected virtual String GenerateSelectString(Type type, string tableName, IRestriction restriction)
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(SELECT);
            ret.Append(tableName);
            ret.Append(" ");
            if (restriction != null)
            {
                ret.Append(restriction.ToSqlString(this, type, this.Target));
            }

            return ret.ToString();
        }
        private string CountStringFromSelect(string select)
        {
            String s = select.Replace(SELECT,COUNT);
            int i = select.IndexOf('*');
            s.Remove(0, i + 1);
            s.Insert(0, COUNT);
            return s;
        }
        
        internal IList<String> selectWhatFromWhere<String>(string what, string from, string where, string value, ISerializerTransaction transaction)
        {
            List<String> ret = new List<String>();
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ").Append(what).Append(" From ").Append(from).Append(" WHERE ");
            if (value != null)
                sb.Append(where).Append("='").Append(value).Append("'");
            else
                sb.Append(where).Append(" is NULL");
            try
            {
                DbCommand com = _connection.CreateCommand();
                com.CommandText = sb.ToString();
                DbDataReader reader = com.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        ret.Add((String)reader[what]);
                    }
                    reader.Close();
                }
                catch (IndexOutOfRangeException)
                {
                    while (reader.Read())
                    {
                        ret.Add((String)reader[0]);
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                return new List<String>();
            }
            
            return ret;
        }
        
        
        //Zur Erzeugung eines DataReaders ohne explizite Tabellenangabe
        private DbDataReader CreateDataReader(Type type, IRestriction restriction, ISerializerTransaction transaction)
        {
            DbCommand com = _connection.CreateCommand();
            transaction.Guard(com);
            com.CommandText = GenerateSelectString(type, restriction);
            DbDataReader reader = com.ExecuteReader();
            return reader;
        }

        private DbDataReader CreateDataReader(Type type, String sqlString, ISerializerTransaction transaction)
        {
            DbCommand com = _connection.CreateCommand();
            transaction.Guard(com);
            com.CommandText = sqlString;
            DbDataReader reader = com.ExecuteReader();
            return reader;
        }

        //Zur Erzeugung eines DataReaders mit Tabellenangabe--wenn kein Tabellenname übergeben wird, wird die Standardmethode versucht
        private DbDataReader CreateDataReader(Type type,string tableName, IRestriction restriction, ISerializerTransaction transaction)
        {
            if (tableName != null)
            {
                DbCommand com = _connection.CreateCommand();
                transaction.Guard(com);
                com.CommandText = GenerateSelectString(type, tableName, restriction);
                DbDataReader reader = com.ExecuteReader();
                return reader;
            }
            else
                return CreateDataReader(type, restriction, transaction);
        }
        #endregion
        #region read

        internal int CountRows<T>(IRestriction restriction, ISerializerTransaction transaction) where T :ISerializableObject
        {
            DbDataReader reader = CreateDataReader(typeof(T), restriction, transaction);

            int i = 0;
            while (reader.Read())
            {
                i++;
            }
            reader.Close();
            return i;
        }

        private class ListImpl<T> : IList<T> where T : ISerializableObject
        {
            private IList<CsvPimaryKey> _keys;
            private Serializer _serializer;
            private Type _storedType;
            private string _table=null;

            public ListImpl(Serializer serializer, IList<CsvPimaryKey> keys, Type t)
            {
                _storedType = t;
                _keys = keys;
                _serializer = serializer;
            }

            public ListImpl(Serializer serializer, IList<CsvPimaryKey> keys, Type t, string table)
            {
                _storedType = t;
                _keys = keys;
                _serializer = serializer;
                _table = table;
            }

            public void Add(T iso)
            {
                throw new NotSupportedException();
            }

            public bool Remove(T iso)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int i)
            {
                throw new NotSupportedException();
            }


            public void Insert(int i, T iso)
            {
                throw new NotSupportedException();

            }

            public int IndexOf(T iso)
            {
                try
                {
                    CsvPimaryKey tmp = new CsvPimaryKey(iso, _serializer.Target);
                    return _keys.IndexOf(tmp);
                }
                catch (Exception)
                {
                    return -1;
                }
            }

            public bool Contains(T iso)
            {
                return IndexOf(iso) != -1;
            }

            public T this[int index]
            {
                get
                {
                    IRestriction r = Restrictions.RestrictionFactory.CsvKeyRestriction(_storedType, _keys[index]);
                    T tmp;
                    if(_table==null)
                        tmp = (T)_serializer.Connector.Load(_storedType, r);
                    else
                        tmp=(T) _serializer.Connector.Load(_storedType,_table,r);
                    if (tmp == null) throw new InvalidOperationException();

                    return tmp;
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

           

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return _keys.Count; }
            }

            public void CopyTo(T[] array, int i)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly
            {
                get { return true; }
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
                private ListImpl<T> _list;
                private int _index = -1;

                public EnumeratorImpl(ListImpl<T> list)
                {
                    _list = list;
                }


                public void Dispose()
                {
                    Reset();
                }

                public T Current
                {
                    get { return _list[_index]; }
                }

                Object IEnumerator.Current
                {
                    get
                    {
                        return _list[_index];
                    }
                }

                public bool MoveNext()
                {
                    _index++;
                    return _index < _list.Count;
                }

                public void Reset()
                {
                    _index = -1;
                }
            }

        }


        internal IList<ISerializableObject> LoadList(Type type, IRestriction restriction, ISerializerTransaction transaction)
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(type, restriction, transaction);

            return new ListImpl<ISerializableObject>(this, tmp, type);

        }

        internal IList<ISerializableObject> LoadList(Type type, String sqlString, ISerializerTransaction transaction)
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(type, sqlString, transaction);

            return new ListImpl<ISerializableObject>(this, tmp, type);

        }

        //Erlaubt es zur Laufzeit eine Klasse auf eine Tabelle zu mappen, welche einen anderen Namen hat als die zugehörige Klasse.
        internal IList<ISerializableObject> LoadList(Type type,string tableName, IRestriction restriction, ISerializerTransaction transaction)
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(type,tableName, restriction, transaction);

            return new ListImpl<ISerializableObject>(this, tmp, type, tableName);

        }

        internal IList<T> LoadList<T>(IRestriction restriction, ISerializerTransaction transaction) where T : ISerializableObject
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(typeof(T), restriction, transaction);

            return new ListImpl<T>(this, tmp, typeof(T));
        }

        internal IList<T> LoadList<T>(String sqlString, ISerializerTransaction transaction) where T: ISerializableObject
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(typeof(T), sqlString, transaction);

            return new ListImpl<T>(this, tmp, typeof(T));

        }

        internal IList<T> LoadList<T>(IRestriction restriction, ISerializerTransaction transaction, string table) where T : ISerializableObject
        {
            IList<CsvPimaryKey> tmp = GetCSVKeys(typeof(T),table, restriction, transaction);

            return new ListImpl<T>(this, tmp, typeof(T),table);
        }

        internal T Load<T>(IRestriction restriction, int readerPosition, ISerializerTransaction transaction) where T : ISerializableObject
        {
            if (readerPosition < 0)
            {
                throw new ArgumentOutOfRangeException("The passed reader position is less than 0.");
            }
            

            bool found = false;
            bool isLoaded = false;


            DbDataReader reader = CreateDataReader(typeof(T), restriction, transaction);
            T data = default(T);

            
            
            for (int i = 0; i <= readerPosition; i++)
            {
                if (!reader.Read())
                {
                    reader.Close();
                    throw new ArgumentOutOfRangeException("The passed reader position is too big.");
                }
            }

            data = (T)_objectPool.RetrieveObject(typeof(T), reader, ref isLoaded);
            found = true;
            reader.Close();

            if (data == null) return default(T);

            bool doRelationLoading = found && isLoaded;
            if (doRelationLoading)
            {

                ResolverData<ISerializableObject> rData = new ResolverData<ISerializableObject>();
                rData.HandledItem = data;
                rData.FieldsToResolve = AttributeWorker.RetrieveAllFields(data.GetType());

                LoadHandler handler = new LoadHandler(this);
                RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
                res.Handler = handler;
                res.StartRelationResolving(rData);
            }

            return data;
        }


        internal IList<T> Select<T>(String sql, ISerializerTransaction transaction) where T: Type
        {
            List<T> ret = new List<T>();
            DbCommand com = _connection.CreateCommand();
            com.CommandText = sql;
            DbDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                ret.Add((T) reader[0]);
            }
            reader.Close();
            return ret;
        }

        internal ISerializableObject Load(Type type, IRestriction restriction, ISerializerTransaction transaction)
        {

            bool found = false;
            bool multiple = false;
            bool isLoaded = false;


            DbDataReader reader = CreateDataReader(type, restriction, transaction);
            ISerializableObject data = null;

            if (reader.Read())
            {                
                data = _objectPool.RetrieveObject(type, reader, ref isLoaded);
                found = true;
            }



            if (reader.Read()) multiple = true;
            reader.Close();

            if (multiple) throw new MultipleResultException("More than one Result found for the given Restriction ...");

            if (data == null) return null;

            bool doRelationLoading = found && isLoaded;//Gesucht: Kriterium wann das wirklich benötigt wird. Benötige ich  bei der Synchronisation die Relationen?
            
            
            if (doRelationLoading)
            {
                
                ResolverData<ISerializableObject> rData = new ResolverData<ISerializableObject>();
                rData.HandledItem = data;
                rData.FieldsToResolve = AttributeWorker.RetrieveAllFields(data.GetType());

                LoadHandler handler = new LoadHandler(this);
                RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
                res.Handler = handler;
                res.StartRelationResolving(rData);
            }

            return data;
        }

        internal ISerializableObject Load(Type type,string table, IRestriction restriction, ISerializerTransaction transaction)
        {

            bool found = false;
            bool multiple = false;
            bool isLoaded = false;


            DbDataReader reader = CreateDataReader(type,table, restriction, transaction);
            ISerializableObject data = null;

            if (reader.Read())
            {
                data = _objectPool.RetrieveObject(type, reader, ref isLoaded);
                found = true;
            }



            if (reader.Read()) multiple = true;
            reader.Close();

            if (multiple) throw new MultipleResultException("More than one Result found for the given Restriction ...");

            if (data == null) return null;

            bool doRelationLoading = found && isLoaded;


            if (doRelationLoading)
            {

                ResolverData<ISerializableObject> rData = new ResolverData<ISerializableObject>();
                rData.HandledItem = data;
                rData.FieldsToResolve = AttributeWorker.RetrieveAllFields(data.GetType());

                LoadHandler handler = new LoadHandler(this);
                RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
                res.Handler = handler;
                res.StartRelationResolving(rData);
            }

            return data;
        }

        internal bool IsManaged(ISerializableObject iso)
        {
            return _objectPool.IsManagedObject(iso);
        }


        public int count(Type type, IRestriction restriction) 
        {
            return count(type, GenerateSelectString(type, restriction));            
        }       

        public int count(Type type, String sqlString)
        {
            int result = 0;
            using (DbCommand com = _connection.CreateCommand())
            {                
                com.CommandText = CountStringFromSelect(sqlString);
                var res = com.ExecuteScalar();
                if (res is int)
                    result = (int)res;                  
            }
            return result;
        }

        #region sqlstring generator

        //Hier Anknüpfungspunkt für Transparent
        internal virtual String SerializeInsert(ISerializableObject iso)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            StringBuilder sql = new StringBuilder();
            StringBuilder values = new StringBuilder();

            sql.Append("INSERT INTO ").Append(w.GetTableMapping(iso.GetType(),_praefix));
            sql.Append(" (");

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(iso.GetType());

            bool start = true;
            foreach (FieldInfo fi in fis)
            {
                if (w.IsAutoincID(fi)) continue;

                try
                {
                    String col = w.GetColumnMapping(fi);
                    Object val = fi.GetValue(iso);



                    if (val == null && (w.IsID(fi) && !w.IsAutoincID(fi)))
                    {
                        throw new SerializerException("PRIMARY KEY FIELD NOT SET!!!!!!!!");
                    }

                    if (!start)
                    {
                        sql.Append(", ");
                        values.Append(", ");
                    }
                    else
                    {
                        start = false;
                    }

                    sql.Append(col);
                    values.Append(SqlUtil.SqlConvert(val));

                }
                catch (SerializerException) { }
            }

            sql.Append(") VALUES (");

            sql.Append(values);

            sql.Append(")");

            return sql.ToString();
        }
        internal virtual String SerializeInsert(ISerializableObject iso, string table)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            StringBuilder sql = new StringBuilder();
            StringBuilder values = new StringBuilder();

            sql.Append("INSERT INTO ").Append(table);
            sql.Append(" (");

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(iso.GetType());

            bool start = true;
            foreach (FieldInfo fi in fis)
            {
                if (w.IsAutoincID(fi)) continue;

                try
                {
                    String col = w.GetColumnMapping(fi);
                    Object val = fi.GetValue(iso);



                    if (val == null && (w.IsID(fi) && !w.IsAutoincID(fi)))
                    {
                        throw new SerializerException("PRIMARY KEY FIELD NOT SET!!!!!!!!");
                    }

                    if (!start)
                    {
                        sql.Append(", ");
                        values.Append(", ");
                    }
                    else
                    {
                        start = false;
                    }

                    sql.Append(col);
                    values.Append(SqlUtil.SqlConvert(val));
                }
                catch (SerializerException) { }
            }

            sql.Append(") VALUES (");

            sql.Append(values);

            sql.Append(")");

            return sql.ToString();
        }
        internal virtual String SerializeUpdate(ISerializableObject iso, ref UpdateStates state, GenericWeakReference<ISerializableObject> tmp)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            Guid g = AttributeWorker.RowGuid(iso);
            FieldInfo f = AttributeWorker.RowGuid(iso.GetType());
            sql.Append("UPDATE ").Append(w.GetTableMapping(iso.GetType(),_praefix));
            sql.Append(" SET ");
            where.Append(" WHERE ").Append(AttributeWorker.GetInstance(Target).GetColumnMapping(f));
            where.Append("='").Append(g).Append("'"); ;

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(iso.GetType());

           

            bool start = true;
            foreach (FieldInfo fi in fis)
            {
                try
                {
                    String col = w.GetColumnMapping(fi);
                    Object val = fi.GetValue(iso);
                    Object oldVal;
                   

                    if (w.IsAutoincID(fi)) continue;
                    if (AttributeWorker.IsRowGuid(fi)) continue;

                    if (w.IsID(fi))
                    {
                        oldVal = tmp.Properties[fi.Name];
                        if (!Object.Equals(val, oldVal))
                        {
                            state = UpdateStates.PRIMARYKEY_MODIFIED;
                        }
                        tmp.Properties[fi.Name] = val;
                    }

                    if (!start)
                    {
                        sql.Append(", ");
                    }
                    else
                    {
                        start = false;
                    }

                    sql.Append(col).Append("=").Append(SqlUtil.SqlConvert(val));
                }
                catch (SerializerException) { }
            }

            sql.Append(where);
            return sql.ToString();
        }

        internal virtual String Delete(ISerializableObject iso)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            StringBuilder sql = new StringBuilder();

            sql.Append("DELETE FROM ").Append(w.GetTableMapping(iso.GetType(),_praefix));
            sql.Append(" WHERE ");

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(iso.GetType());
            bool start = true;
            foreach (FieldInfo fi in fis)
            {
                if (w.IsID(fi))
                {
                    String col = w.GetColumnMapping(fi);
                    Object val = fi.GetValue(iso);

                    if (val == null) throw new SerializerException("Not deletable ...");

                    if (!start)
                    {
                        sql.Append(" AND ");
                    }

                    start = false;
                    sql.Append(col).Append("=").Append(SqlUtil.SqlConvert(val));
                }
            }

            return sql.ToString();
        }

        #endregion


        public IList<CsvPimaryKey> GetCSVKeys(Type type, IRestriction restriction, ISerializerTransaction transaction)
        {
            string select = GenerateSelectString(type, restriction);
            return GetCSVKeys(type, select, transaction);
        }

        public IList<CsvPimaryKey> GetCSVKeys(Type type, String sqlString, ISerializerTransaction transaction)
        {
            List<CsvPimaryKey> ret = new List<CsvPimaryKey>();
            
            
            if (this._ipro != null)
                this._ipro.InternalTotal = this.count(type, sqlString); 
            DbDataReader reader = CreateDataReader(type, sqlString, transaction);            
            while (reader.Read())
            {

                CsvPimaryKey key = new CsvPimaryKey(type, reader, Target);
                ret.Add(key);

                if (this._ipro != null)
                {
                    this._ipro.advance();
                    if (this._ipro.IsCancelRequested)
                        break;
                }
            }

            reader.Close();
            return ret;
        }


        public IList<CsvPimaryKey> GetCSVKeys(Type type, string tableName, IRestriction restriction, ISerializerTransaction transaction)
        {
            string select = GenerateSelectString(type, tableName, restriction);
            return GetCSVKeys(type, select, transaction);
        }

        public T CreateISerializableObject<T>() where T : ISerializableObject
        {

            AttributeWorker w = AttributeWorker.GetInstance(Target);

            ISerializableObject iso = null;
            try
            {
                iso = (ISerializableObject)Activator.CreateInstance(typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new SerializerException();
            }

            _objectPool.Register(iso);

            ResolverData<ISerializableObject> data = new ResolverData<ISerializableObject>();
            data.FieldsToResolve = AttributeWorker.RetrieveAllFields(iso.GetType());
            data.HandledItem = iso;
            CreateHandler handler = new CreateHandler(this);
            RelationResolver<ISerializableObject> resolver = new RelationResolver<ISerializableObject>();
            resolver.Handler = handler;
            resolver.StartRelationResolving(data);
            return (T)iso;
        }

        public ISerializableObject CreateISerializableObject(Type type)
        {

            AttributeWorker w = AttributeWorker.GetInstance(Target);

            ISerializableObject iso = null;
            try
            {
                iso = (ISerializableObject)Activator.CreateInstance(type);
            }
            catch (InvalidCastException)
            {
                throw new SerializerException();
            }

            _objectPool.Register(iso);

            ResolverData<ISerializableObject> data = new ResolverData<ISerializableObject>();
            data.FieldsToResolve = AttributeWorker.RetrieveAllFields(iso.GetType());
            data.HandledItem = iso;
            CreateHandler handler = new CreateHandler(this);
            RelationResolver<ISerializableObject> resolver = new RelationResolver<ISerializableObject>();
            resolver.Handler = handler;
            resolver.StartRelationResolving(data);

            return iso;
        }

        public ISerializableObject CreateISerializableObject(Type type, Guid rowGuid)
        {
            ISerializableObject iso = CreateISerializableObject(type);
            iso.Rowguid = rowGuid;
            return iso;
        }
       


       

        internal bool Exists(String column, Object value, String table, ISerializerTransaction transaction)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            StringBuilder b = new StringBuilder();

            b.Append("SELECT ").Append(column).Append(" FROM ").Append(table).Append(" WHERE ").Append(column).Append("=");
            b.Append(SqlUtil.SqlConvert(value));

            DbCommand com = _connection.CreateCommand();
            transaction.Guard(com);
            com.CommandText = b.ToString();
            //Console.WriteLine(com.CommandText);

            
            DbDataReader r = com.ExecuteReader();

            if (r.Read())
            {
                r.Close();
                return true;
            }
            r.Close();
            return false;
        }


        #endregion

        #region write

        public event UpdateEventHandler UpdateEvent;
        protected virtual void OnUpdateEvent(UpdateEventArgs args) 
        {
            if (UpdateEvent != null)
            {
                UpdateEvent(this, args);
            }
        }
        

        internal void Save(ISerializableObject iso, ISerializerTransaction transaction)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);

            if (!IsManaged(iso))
            {
//                Console.WriteLine("SERIALIZE");
                _objectPool.InsertObject(iso, _connection, transaction);

                ResolverData<ISerializableObject> rData = new ResolverData<ISerializableObject>();
                rData.HandledItem = iso;
                rData.FieldsToResolve = AttributeWorker.RetrieveAllFields(iso.GetType());

                LoadHandler handler = new LoadHandler(this);
                RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
                res.Handler = handler;
                res.StartRelationResolving(rData);
            }
            else
            {
//                Console.WriteLine("UPDATE");
                UpdateStates state = _objectPool.UpdateObject(iso, _connection, transaction);

                if (state == UpdateStates.NO_ROWS_AFFECTED)
                {
                    throw new SerializerException("entry could not be updated ...");
                }
                else if (state == UpdateStates.PRIMARYKEY_MODIFIED)
                {
                    UpdateEventArgs args = new UpdateEventArgs(state);
                    OnUpdateEvent(args);
                }

                ResolverData<ISerializableObject> data = new ResolverData<ISerializableObject>();
                data.HandledItem = iso;
                data.FieldsToResolve = AttributeWorker.RetrieveAllFields(iso.GetType());

                RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
                res.Handler = new UpdateHandler(this);
                res.StartRelationResolving(data);
            }
        }

        internal void UpdatePlain(ISerializableObject iso, ISerializerTransaction transaction)
        {
            UpdateStates state = _objectPool.UpdateObject(iso, _connection, transaction);
            if (state == UpdateStates.NO_ROWS_AFFECTED)
            {
                throw new SerializerException("entry could not be updated ...");
            }
            else if (state == UpdateStates.PRIMARYKEY_MODIFIED)
            {
                UpdateEventArgs args = new UpdateEventArgs(state);
                OnUpdateEvent(args);
            }
        }

        internal void InsertPlain(ISerializableObject iso, ISerializerTransaction transaction)
        {
            _objectPool.InsertObject(iso, _connection, transaction);
        }

        internal void InsertPlain(ISerializableObject iso, ISerializerTransaction transaction,string table)
        {
            _objectPool.InsertObject(iso, _connection, transaction,table);
        }

        #endregion

        #region delete

        internal void Delete(ISerializableObject iso, ISerializerTransaction transaction)
        {
            if (!IsManaged(iso)) return;



            AttributeWorker w = AttributeWorker.GetInstance(Target);


            ResolverData<ISerializableObject> data = new ResolverData<ISerializableObject>();
            data.FieldsToResolve = AttributeWorker.RetrieveAllFields(iso.GetType());
            data.HandledItem = iso;

            DeleteHandler h = new DeleteHandler(this);
            RelationResolver<ISerializableObject> res = new RelationResolver<ISerializableObject>();
            res.Handler = h;

            
            h.MODE = DeleteHandler.DOWNMODE;
            res.StartRelationResolving(data);

            
            _objectPool.DeleteObject(iso, _connection, transaction);

            h.MODE = DeleteHandler.UPMODE;
            res.StartRelationResolving(data);
        }

        internal void Truncate(string table, ISerializerTransaction transaction)
        {

            //To Fill

        }

        #endregion

        #region relationmanagement 

        public void ConnectOneToMany(ISerializableObject source, ISerializableObject target)
        {
            IVirtualKey vKey = AttributeWorker.GetInstance(Target).CreateVirtualKey(this, source.GetType(), target.GetType());
            vKey.InitVirtualKeyBySource(source);
            vKey.ApplyVirtualKeyToTarget(target);

            try
            {
                vKey.TargetField.SetValue(target, source);
            }
            catch (Exception)
            {

            }
        }

        public void ConnectOneToOne(ISerializableObject source, ISerializableObject target)
        {
            IVirtualKey vKey = AttributeWorker.GetInstance(Target).CreateVirtualKey(this, source.GetType(), target.GetType());
            vKey.InitVirtualKeyBySource(source);
            vKey.ApplyVirtualKeyToTarget(target);

            try
            {
                vKey.TargetField.SetValue(target, source);
            }
            catch (Exception)
            {

            }
        }

        #endregion

      

        #region Properties
        public string Target { get { return _target; } set { _target = value; } }
        public String Praefix { get { return _praefix; } }
        public Connector Connector
        {

            get
            {
                if (_currentConnector == null)
                {
                    _currentConnector = new Connector(this);
                    _currentConnector.AddObserver(_objectPool);
                }
                return _currentConnector;
            }
        }
         public IAdvanceProgress Progress
            {
                get
                {
                    if (this._ipro != null)
                        return this._ipro;
                    else
                        throw new Exception("Null-Value: Set an Object which implements IReportProgress");
                }
                set
                {
                    this._ipro = value;
                }
            }
        #endregion
    }

    

}
