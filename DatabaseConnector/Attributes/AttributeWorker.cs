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
using System.Reflection;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    public class AttributeWorker
    {
        private String _target;
        private Dictionary<Serializer.Serializer, VirtualKeyFactory> _virtualKeyFactories;
        private static IDictionary<String, AttributeWorker> _workers = new Dictionary<String, AttributeWorker>();


        public static AttributeWorker GetInstance(String target)
        {
            if (_workers.ContainsKey(target))
            {
                return _workers[target];
            }
            else
            {
                _workers[target] = new AttributeWorker(target);
                return _workers[target];
            }

        }

        private AttributeWorker(String target)
        {
            _target = target;
            _virtualKeyFactories = new Dictionary<Serializer.Serializer, VirtualKeyFactory>();
        }

        public void AddVirtualKeyFactory(Serializer.Serializer context, VirtualKeyFactory vf)
        {
            if (vf.Worker != this)
            {
                throw new Serializer.SerializerException();
            }
            _virtualKeyFactories[context] = vf;
        }

        public IVirtualKey CreateVirtualKey(Serializer.Serializer context, Type source, Type target)
        {
            return _virtualKeyFactories[context].GetVirtualKeyTemplate(source, target).CreateFromTemplate();
        }

        public IList<Type> ComputeVirtualKeyPath(Serializer.Serializer context, Type from, Type to)
        {
            List<Type> ret = _virtualKeyFactories[context].ComputeVirtualKeyPath(from, to, new List<Type>());
            ret.Reverse();

            return ret;
        }

        //Diese Methoden sind nicht von einem Target abhängig
        internal static MappedByAttribute GetMappedByAttribute(FieldInfo fi)
        {
            return (MappedByAttribute)Attribute.GetCustomAttribute(fi, typeof(MappedByAttribute));
        }

        internal static JoinColumsAttribute[] GetJoinColumnsAttributes(FieldInfo fi)
        {
            return (JoinColumsAttribute[])Attribute.GetCustomAttributes(fi, typeof(JoinColumsAttribute));
        }

        internal static RelationalAttribute GetRelationAttribute(FieldInfo fi)
        {
            return (RelationalAttribute)Attribute.GetCustomAttribute(fi, typeof(RelationalAttribute));
        }

        internal static ResolveAttribute GetResolveAttribute(FieldInfo fi)
        {
            return (ResolveAttribute)Attribute.GetCustomAttribute(fi, typeof(ResolveAttribute));
        }



        public FieldInfo RetrieveField(Type t, String name, bool ignoreTarget)
        {
            foreach (FieldInfo fi in RetrieveAllFields(t))
            {
                if (fi != null)
                {
                    if (fi.Name.Equals(name))
                    {
                        if (IsPersistentField(fi) || ignoreTarget)
                        {
                            return fi;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsRelationField(FieldInfo fi) {
            return GetRelationAttribute(fi) != null;
        }

        public static FieldInfo[] RetrieveAllFields(Type t)
        {
            IList<FieldInfo> tmp = new List<FieldInfo>();

            if (t == typeof(Object))
            {
                return tmp.ToArray();
            }

            foreach (FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                tmp.Add(fi);
            }

            Type recursiveType = t.BaseType;
            foreach (FieldInfo fi in RetrieveAllFields(recursiveType))
            {
                tmp.Add(fi);
            }

            return tmp.ToArray();
        }

        public static FieldInfo RowGuid(Type t)
        {
            foreach (FieldInfo f in RetrieveAllFields(t))
            {
                RowGuidAttribute rga = (RowGuidAttribute)Attribute.GetCustomAttribute(f, typeof(RowGuidAttribute));
                if (rga != null)
                {
                    return f;
                }
            }

            throw new Serializer.SerializerException();
        }

        public static Guid RowGuid(ISerializableObject iso)
        {
            return (Guid)RowGuid(iso.GetType()).GetValue(iso);
        }

        public static void RowGuid(ISerializableObject iso, Guid value)
        {
            RowGuid(iso.GetType()).SetValue(iso, value);
        }

        public int ComputeHashCode(ISerializableObject iso)
        {
            int hashCode = 0;
            FieldInfo[] fis = RetrieveAllFields(iso.GetType());

            foreach (FieldInfo fi in fis)
            {
                if (IsPersistentField(fi))
                {
                    Object val = fi.GetValue(iso);
                    if (val != null) hashCode = hashCode ^ val.GetHashCode();
                }
            }

            return hashCode;
        }

        public String ComputeObjectState(ISerializableObject iso)
        {
            StringBuilder objectState = new StringBuilder();
            FieldInfo[] fis = RetrieveAllFields(iso.GetType());

            foreach (FieldInfo fi in fis)
            {
                if (IsPersistentField(fi))
                {
                    Object val = fi.GetValue(iso);
                    objectState.Append(val);
                }
            }

            return objectState.ToString();
        }

        internal IDictionary<String, FieldInfo> GetPrimaryKeyFields(Type type)
        {
            IDictionary<String, FieldInfo> ret = new Dictionary<String, FieldInfo>();
            FieldInfo[] fis = RetrieveAllFields(type);
            foreach (FieldInfo fi in fis)
            {
                if (IsID(fi))
                {
                    ret[GetColumnMapping(fi)] = fi;
                }
            }
            return ret;
        }



        //Gibt ein über Attribute gespeichertes TableMapping zurück
        internal String GetTableMapping(Type type, String dbPraefix)
        {
            TableAttribute[] a = (TableAttribute[])Attribute.GetCustomAttributes(type, typeof(TableAttribute));
            TableAttribute ta = FilterByTarget(a, _target);

            string ret = null;
            if (ta != null)
            {
                if (ta.TableMapping.Equals("MappingDictionary"))
                {
                    String mapping;
                    MappingDictionary.Mapping.TryGetValue(type,out mapping);
                    ret = mapping;
                }
                else
                    ret = ta.TableMapping;
            }
            //Es wäre ein Abfrage sinnvoll die nach einem auf Obejktebene gespeicherten Mapping sucht 
            
            if(ret == null)
            {
                ret = type.Name;
            }
            if (dbPraefix == null || dbPraefix==String.Empty)
                return ret;
            else
                return dbPraefix + ret;
        }

        public bool IsPersistentField(FieldInfo fi)
        {
            ColumnAttribute[] cols = (ColumnAttribute[])Attribute.GetCustomAttributes(fi, typeof(ColumnAttribute));
            return FilterByTarget(cols, _target) != null;
        }

        public String GetColumnMapping(FieldInfo fi)
        {
            ColumnAttribute[] cols = (ColumnAttribute[])Attribute.GetCustomAttributes(fi, typeof(ColumnAttribute));
            ColumnAttribute col = FilterByTarget(cols, _target);

            if (col == null)
            {
                throw new Serializer.SerializerException("Field is no valid Column.");
            }

            String mapping = col.Mapping;

            if(mapping == null)
            {
                mapping = fi.Name;
                if (mapping.StartsWith("_"))
                {
                    mapping = mapping.Remove(0, 1);
                }
            }
            return mapping;
        }

        public bool IsID(FieldInfo fi)
        {
            return Attribute.GetCustomAttribute(fi, typeof(IDAttribute)) != null;
        }

        public bool IsAutoincID(FieldInfo fi)
        {
            IDAttribute id = (IDAttribute)Attribute.GetCustomAttribute(fi, typeof(IDAttribute));
            if (id != null)
            {
                return id.Autoinc;
            }
            return false;
        }

        public T GetAttribute<T>(FieldInfo fi) where T : Attribute, ITarget
        {
            T[] tmp = (T[])Attribute.GetCustomAttributes(fi, typeof(T));

            return FilterByTarget(tmp, this._target);
        }

        public IList<FieldInfo> GetFieldByAttribute<T>(Type type) where T : Attribute, ITarget
        {
            FieldInfo[] fis = RetrieveAllFields(type);
            List<FieldInfo> l = new List<FieldInfo>();


            foreach (FieldInfo fi in fis)
            {
                T[] attrs = (T[])Attribute.GetCustomAttributes(fi, typeof(T));

                foreach (T attr in attrs)
                {
                    if (attr.Target == _target)
                    {
                        l.Add(fi);
                    }
                }
            }

            return l;
        }

        public bool IsForeignKey(Serializer.Serializer context, FieldInfo fi)
        {
            return _virtualKeyFactories[context].IsForeignKey(fi);
           
        }

        public static bool IsRowGuid(FieldInfo fi)
        {
            RowGuidAttribute id = (RowGuidAttribute)Attribute.GetCustomAttribute(fi, typeof(RowGuidAttribute));
            if (id != null)
            {
                return true;
            }
            return false;
        }



        internal FieldInfo GetAutoincField(Type t)
        {
            foreach (FieldInfo fi in RetrieveAllFields(t))
            {
                if (IsAutoincID(fi))
                {
                    return fi;
                }
            }

            return null;
        }

        private static P FilterByTarget<P>(P[] attributes, String target) where P : Attribute, ITarget
        {
            if (attributes == null) return null;
            if (attributes.Length == 0) return null;
            P tmp = null;
            foreach (P a in attributes)
            {
                if (target == null)
                {
                    return a;
                }
                if (a.Target == target)
                {
                    return a;
                }
                if (a.Target == AttributeConstants.DEFAULT_TARGET)
                {
                    tmp = a;
                }
            }
            
            return tmp;
        }

        public static String ToStringHelper(ISerializableObject iso, int width)
        {

            FieldInfo[] fis = iso.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            StringBuilder b = new StringBuilder();

            b.Append(System.Environment.NewLine);
            b.Append("-----------------------------------").Append(System.Environment.NewLine);
            b.Append(iso.GetType().Name).Append(System.Environment.NewLine);
            b.Append("-----------------------------------").Append(System.Environment.NewLine);

            foreach (FieldInfo fi in fis)
            {
                Object tmp = fi.GetValue(iso);
                if (tmp == null) tmp = "null";
                if (AttributeWorker.IsRelationField(fi)) continue;
                b.Append(fi.Name).Append(" = ").Append(tmp).Append(System.Environment.NewLine);
            }

            return b.ToString();
        }
    }
}
