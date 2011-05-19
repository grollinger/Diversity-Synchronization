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
#define TRACE

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Data.Common;
using System.Runtime.InteropServices;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Relations;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{


    internal class SortedSerializableObjectPool : ISerializableObjectPool, ICleanable
    {

        private FastSearchDictionary<Guid, GenericWeakReference<ISerializableObject>> _persistentObjects;

        private Cleaner _cleaner;
        private String _target = AttributeConstants.DEFAULT_TARGET;
        private Comparer _comparer;
        private Serializer _owner;


        
        private Object _lock = new Object();

        internal SortedSerializableObjectPool(Serializer owner, IList<Type> registeredTypes)
        {
            this._owner = owner;
            this._target = owner.Target;
            this._comparer = new Comparer(System.Globalization.CultureInfo.CurrentCulture);
            this._persistentObjects = new FastSearchDictionary<Guid,GenericWeakReference<ISerializableObject>>(_comparer);
        }



        //////////////////////////////////////////////

        #region properties 
        
        public Cleaner Cleaner
        {
            set 
            {
                if (_cleaner == null)
                {
                    _cleaner = value;
                    _cleaner.Cleanable = this;
                }
                else
                {
                    throw new ArgumentException("Cleaner strategy is already attached an cannot be altered!");
                }
            }
        }

        public String Target
        {
            get { return _target; }
            set { _target = value; }
        }
        #endregion

        


        public void Register(ISerializableObject iso)
        {
           AttributeWorker.RowGuid(iso, Guid.NewGuid());
        }

        #region dbaccess

        public ISerializableObject RetrieveObject(Type objectType, DbDataReader reader, ref bool isLoaded)
        {
            if (_cleaner == null)
            {
                throw new NullReferenceException("No cleaner attached SerializableObjectPool needs a Cleaner to work...");
            }

            ISerializableObject ret = null;



            FieldInfo fi = AttributeWorker.RowGuid(objectType);
            Object o = reader[AttributeWorker.GetInstance(Target).GetColumnMapping(fi)];
            Guid g = (Guid)o;


            
            try
            {
                ret = _persistentObjects[g].Target;
                isLoaded = false;

                if (ret == null) throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                ret = Deserialize(objectType, reader);
                GenericWeakReference<ISerializableObject> tmp = new GenericWeakReference<ISerializableObject>(ret);
                _persistentObjects[AttributeWorker.RowGuid(ret)] = tmp;
                MemoriseKeys(ret, AttributeWorker.GetInstance(Target), tmp);
                isLoaded = true;
            }

            _cleaner.Cleanup();

            return ret;
        }

        public void InsertObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction) 
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            DbCommand com = connection.CreateCommand();
            com.CommandText = _owner.SerializeInsert(iso);
            transaction.Guard(com);
            //Console.WriteLine(com.CommandText);
            try
            {
                //Console.WriteLine(com.ExecuteNonQuery());
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new SerializerException("insert failed", ex);
            }


            

            FieldInfo autoincField;
            Guid g = AttributeWorker.RowGuid(iso);

            if ((autoincField = w.GetAutoincField(iso.GetType())) != null)
            {
                StringBuilder b = new StringBuilder();

                b.Append("SELECT * FROM ").Append(w.GetTableMapping(iso.GetType(),_owner.Praefix)).Append(" WHERE ");
                b.Append(w.GetColumnMapping(AttributeWorker.RowGuid(iso.GetType()))).Append("='");
                b.Append(g.ToString()).Append("'");

                com.CommandText = b.ToString();
                DbDataReader r = com.ExecuteReader();

                if (r.Read())
                {
                    Object o = r[w.GetColumnMapping(w.GetAutoincField(iso.GetType()))];
                    autoincField.SetValue(iso, o);
                }
                else
                {
                    r.Close();
                    throw new SerializerException("Insert core method failed! error unknown...");
                }

                r.Close();
                 
            }


            GenericWeakReference<ISerializableObject> tmp = new GenericWeakReference<ISerializableObject>(iso);
            _persistentObjects[g] = tmp;
            MemoriseKeys(iso, w, tmp);
        }

        public void InsertObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction, string table)
        {
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            DbCommand com = connection.CreateCommand();
            com.CommandText = _owner.SerializeInsert(iso,table);
            transaction.Guard(com);
            //Console.WriteLine(com.CommandText);
            try
            {
                //Console.WriteLine(com.ExecuteNonQuery());
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new SerializerException("insert failed", ex);
            }




            FieldInfo autoincField;
            Guid g = AttributeWorker.RowGuid(iso);

            if ((autoincField = w.GetAutoincField(iso.GetType())) != null)
            {
                StringBuilder b = new StringBuilder();

                b.Append("SELECT * FROM ").Append(w.GetTableMapping(iso.GetType(),_owner.Praefix)).Append(" WHERE ");
                b.Append(w.GetColumnMapping(AttributeWorker.RowGuid(iso.GetType()))).Append("='");
                b.Append(g.ToString()).Append("'");

                com.CommandText = b.ToString();
                DbDataReader r = com.ExecuteReader();

                if (r.Read())
                {
                    Object o = r[w.GetColumnMapping(w.GetAutoincField(iso.GetType()))];
                    autoincField.SetValue(iso, o);
                }
                else
                {
                    r.Close();
                    throw new SerializerException("Insert core method failed! error unknown...");
                }

                r.Close();

                /*com.CommandText = "SELECT @@IDENTITY";
                    Object o = com.ExecuteScalar();
                    Console.WriteLine("---->"+o);
                    if (o is DBNull)
                    {
                        throw new SerializerException();
                    }
                    
                    autoincField.SetValue(iso, Decimal.ToInt32((Decimal)o));
                 */


            }


            GenericWeakReference<ISerializableObject> tmp = new GenericWeakReference<ISerializableObject>(iso);
            _persistentObjects[g] = tmp;
            MemoriseKeys(iso, w, tmp);
        }

        public UpdateStates UpdateObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction)
        {

            UpdateStates state = UpdateStates.UPDATED;
            AttributeWorker w = AttributeWorker.GetInstance(Target);
            DbCommand com = connection.CreateCommand();
            Guid g = AttributeWorker.RowGuid(iso);
            GenericWeakReference<ISerializableObject> tmp;
            try
            {
                tmp = _persistentObjects[g];
            }
            catch (KeyNotFoundException ex)
            {
                throw new UpdateException("update failed. object is not known by the system and must be loaded first", ex);
            }
            com.CommandText = _owner.SerializeUpdate(iso, ref state,tmp);
            transaction.Guard(com);
            //Console.WriteLine(com.CommandText);
            try
            {
                if (com.ExecuteNonQuery() < 1)
                {
                    state = UpdateStates.NO_ROWS_AFFECTED;
                    return state;
                }
                return state;
            }
            catch (Exception ex)
            {
                throw new SerializerException("update failed", ex);
            }
        }

        public void DeleteObject(ISerializableObject iso, DbConnection connection, ISerializerTransaction transaction)
        {
            if (AttributeWorker.RowGuid(iso) == null) throw new SerializerException();

            DbCommand com = connection.CreateCommand();
            transaction.Guard(com);
            com.CommandText = _owner.Delete(iso);
            //Console.WriteLine(com.CommandText);
            //Console.WriteLine(com.ExecuteNonQuery());
            com.ExecuteNonQuery();

            _persistentObjects.Remove(AttributeWorker.RowGuid(iso));
        }

        private static void MemoriseKeys(ISerializableObject iso, AttributeWorker w, GenericWeakReference<ISerializableObject> gwr)
        {
            IDictionary<String, FieldInfo> pkf = w.GetPrimaryKeyFields(iso.GetType());

            foreach (FieldInfo fi in pkf.Values)
            {
                if (w.IsAutoincID(fi)) continue;
                gwr.Properties[fi.Name] = fi.GetValue(iso);
            }
        }

        #endregion

        private void RestoreSnapshot(Connector connector)
        {
            
            //Hier kann eine bessere Version des Rollbacks codiert werden
            //Zur Zeit werden die Daten auf der Datenbank wieder hergestellt
            //das System wurde nicht zurückgesetzt. 
        }

        private void CleanSnapshot(Connector connector)
        {
            //Hier kann eine bessere Version des Rollbacks codiert werden
        }

        private void MakeSnapshot(Connector connector)
        {
            //Hier kann eine bessere Version des Rollbacks codiert werden
            //zum Beispiel könnte man ein abbild aller im Speicher befindlichen
            //Objekte machen und damit die anderen Objekte wieder herstellen
        }



        public bool IsManagedObject(ISerializableObject iso)
        {
            try
            {
                return _persistentObjects[AttributeWorker.RowGuid(iso)].Target != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public void Cleanup()
        {
            lock (_lock)
            {
                List<Guid> remove = new List<Guid>();
                foreach(Guid g in _persistentObjects.Keys) {
                    if (_persistentObjects[g].Target == null)
                    {
                        remove.Add(g);
                    }
                }

                foreach (Guid g in remove)
                {
                    _persistentObjects.Remove(g);
                }
            }
        }

        public void Update(IObeservable observable, Object message) 
        {
            if (observable is Connector)
            {
                if (message.ToString().Equals(Connector.ROLLBACK_MESSAGE))
                {
                    RestoreSnapshot((Connector)observable);
                } else if (message.ToString().Equals(Connector.BEGIN_TRANSACTION_MESSAGE))
                {
                    MakeSnapshot((Connector)observable);
                }
                else if (message.ToString().Equals(Connector.COMMIT_MESSAGE))
                {
                    CleanSnapshot((Connector)observable);
                }
            }
        }

        // ////////////////////////////////////////////


        
        private ISerializableObject Deserialize(Type type, DbDataReader reader)
        {
            ISerializableObject data = (ISerializableObject)Activator.CreateInstance(type);
            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(type);
            foreach (FieldInfo fi in fis)
            {
                if (AttributeWorker.GetInstance(Target).IsPersistentField(fi))
                {
                    Object o=null;
                    Type t=null;
                    try
                    {
                        o = reader[AttributeWorker.GetInstance(Target).GetColumnMapping(fi)];
                        t = o.GetType();
                        if (o is DBNull)
                        {
                            o = null;
                        }
                        fi.SetValue(data, o);
                    }
                    catch (ArgumentException ex)
                    {
                        String s = o.GetType().FullName;
                        Type targetType = FieldMapping.Mapping[o.GetType().FullName];
                        if(targetType.Equals(typeof(String)))
                        {
                            fi.SetValue(data, o.ToString());
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        System.Text.StringBuilder msg = new System.Text.StringBuilder();
                        msg.Append("column: ").Append(AttributeWorker.GetInstance(Target).GetColumnMapping(fi)).Append(" not found\n");
                        msg.Append("in table: " + AttributeWorker.GetInstance(Target).GetTableMapping(data.GetType(),_owner.Praefix)).Append("!");
                        throw new SerializerException(msg.ToString());
                    }
                }
            }
            return data;
        }

       
    }
}
