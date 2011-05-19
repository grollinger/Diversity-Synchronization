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
    public interface IVirtualKey
    {
        void ApplyVirtualKeyToTarget(ISerializableObject target);
        void RemoveVirtualKeyFromTarget(ISerializableObject target);
        void InitVirtualKeyBySource(ISerializableObject source);
        void InitVirtualKeyByTarget(ISerializableObject target);


        FieldInfo SourceField { get; }
        FieldInfo TargetField { get; }

        Type SourceType { get; }
        Type TargetType { get; }

        String ToSqlStringForward(String target);
        String ToSqlStringBackward(String target);

        QueryGenerator ToSqlRestriction(String target, Type from, Type to, Serializer.Serializer ser);
    }

    internal class VirtualKey : IVirtualKey
    {
        private Dictionary<String, Object> _virtualKeyValues;
        private Dictionary<String, FieldInfo> _virtualDefKeyFields;
        private Dictionary<String, FieldInfo> _virtualJoinKeyFields;
        private Dictionary<String, String> _keyTranslation;


        private FieldInfo _sourceField;
        private FieldInfo _targetField;
        private Type _sourceType;
        private Type _targetType;


        internal VirtualKey()
        {
            _keyTranslation = new Dictionary<String, String>(1);
            _virtualDefKeyFields = new Dictionary<String, FieldInfo>(1);
            _virtualJoinKeyFields = new Dictionary<String, FieldInfo>(1);
        }


        public void ApplyVirtualKeyToTarget(ISerializableObject target)
        {
            if (_targetType != target.GetType())
            {
                throw new Serializer.SerializerException();
            }

            foreach (String key in _virtualKeyValues.Keys)
            {
                Object val = _virtualKeyValues[key];
                String tranlatedKey = _keyTranslation[key];
                _virtualJoinKeyFields[tranlatedKey].SetValue(target, val);
            }
        }
        public void RemoveVirtualKeyFromTarget(ISerializableObject target)
        {
            if (_targetType != target.GetType())
            {
                throw new Serializer.SerializerException();
            }

            foreach (String key in _virtualJoinKeyFields.Keys)
            {
                _virtualJoinKeyFields[key].SetValue(target, null);
            }
        }


        public void InitVirtualKeyBySource(ISerializableObject source)
        {
            if (_virtualKeyValues == null)
            {
                throw new Serializer.SerializerException();
            }

            if (_sourceType != source.GetType())
            {
                throw new Serializer.SerializerException();
            }

            foreach (String key in _virtualDefKeyFields.Keys)
            {
                Object val = _virtualDefKeyFields[key].GetValue(source);

                if (val == null)
                {
                    throw new InvalidOperationException("Virtual Keys can only be initialized by persitent objects ...");
                }

                _virtualKeyValues[key] = val;
            }
        }
        public void InitVirtualKeyByTarget(ISerializableObject target)
        {
            if (_virtualKeyValues == null)
            {
                throw new Serializer.SerializerException();
            }

            if (_targetType != target.GetType())
            {
                throw new Serializer.SerializerException();
            }

            foreach (String key in _virtualJoinKeyFields.Keys)
            {
                Object val = _virtualJoinKeyFields[key].GetValue(target);

                if (val == null)
                {
                    throw new InvalidOperationException("Virtual Keys can only be initialized by persitent objects ...");
                }

                String _translatedKey = null;
                foreach (String k in _keyTranslation.Keys)
                {
                    if (_keyTranslation[k] == key)
                    {
                        _translatedKey = k;
                    }
                }

                if (_translatedKey == null)
                {
                    throw new Serializer.SerializerException();
                }

                _virtualKeyValues[_translatedKey] = val;
            }
        }


        public String ToSqlStringForward(String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            StringBuilder tmp = new StringBuilder();

            bool start = true;
            foreach (String key in _virtualKeyValues.Keys)
            {
                if (!start)
                {
                    tmp.Append(" AND ");
                }
                start = false;
                Object val = _virtualKeyValues[key];

                if (val == null) throw new InvalidOperationException("The key is not initialized");

                String translatedKey = _keyTranslation[key];
                FieldInfo joinField = _virtualJoinKeyFields[translatedKey];
                String column = w.GetColumnMapping(joinField);
                tmp.Append(column).Append("=").Append(SqlUtil.SqlConvert(val));
            }

            return tmp.ToString();
        }
        public String ToSqlStringBackward(String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            StringBuilder tmp = new StringBuilder();

            bool start = true;
            foreach (String key in _virtualKeyValues.Keys)
            {
                if (!start)
                {
                    tmp.Append(" AND ");
                }
                start = false;
                Object val = _virtualKeyValues[key];
                FieldInfo defField = _virtualDefKeyFields[key];
                String column = w.GetColumnMapping(defField);
                tmp.Append(column).Append("=").Append(SqlUtil.SqlConvert(val));
            }

            return tmp.ToString();
        }


        public QueryGenerator ToSqlRestriction(String target, Type from, Type to, Serializer.Serializer ser)
        {
            if (from == _sourceType && to == _targetType)
            {
                return ToSqlRestrictionForward(target, ser);
            }
            else if (from == _targetType && to == _sourceType)
            {
                return ToSqlRestrictionBackward(target, ser);
            }

            throw new Exception();
        }
        private QueryGenerator ToSqlRestrictionBackward(String target, Serializer.Serializer ser)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);
            QueryGenerator gen = new QueryGenerator();
            gen.AddQueryComponent(new SqlStringComponent(" WHERE "));

            bool start = true;
            foreach (String key in _virtualDefKeyFields.Keys)
            {
                if (!start)
                {
                    gen.AddQueryComponent(new SqlStringComponent(" AND "));
                    //ret.Append(" AND ");
                }
                else
                {
                    gen.AddQueryComponent(new SqlStringComponent("("));
                    //ret.Append("(");
                }
                start = false;

                //Object val = _virtualKeyValues[key];
                String translatedKey = _keyTranslation[key];
                FieldInfo joinField = _virtualJoinKeyFields[translatedKey];
                String joinMapping = w.GetColumnMapping(joinField);

                FieldInfo defField = _virtualDefKeyFields[key];
                String defMapping = w.GetColumnMapping(defField);

                StringBuilder tmp = new StringBuilder();
                tmp.Append(joinMapping).Append(" IN (SELECT ").Append(defMapping).Append(" FROM ");
                tmp.Append(w.GetTableMapping(SourceType, ser.Praefix));
                gen.AddQueryComponent(new SqlStringComponent(tmp));
                gen.AddQueryComponent(new PlaceHolderComponent());
                gen.AddQueryComponent(new SqlStringComponent(")"));
            }
            gen.AddQueryComponent(new SqlStringComponent(")"));
            return gen;
        }

        private QueryGenerator ToSqlRestrictionForward(String target, Serializer.Serializer ser)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            QueryGenerator gen = new QueryGenerator();
            gen.AddQueryComponent(new SqlStringComponent(" WHERE "));

            bool start = true;
            foreach (String key in _virtualDefKeyFields.Keys)
            {
                if (!start)
                {
                    gen.AddQueryComponent(new SqlStringComponent(" AND "));
                }
                else
                {
                    gen.AddQueryComponent(new SqlStringComponent("("));
                }
                start = false;

                String translatedKey = _keyTranslation[key];
                FieldInfo joinField = _virtualJoinKeyFields[translatedKey];
                String joinMapping = w.GetColumnMapping(joinField);

                FieldInfo defField = _virtualDefKeyFields[key];
                String defMapping = w.GetColumnMapping(defField);

                StringBuilder tmp = new StringBuilder();

                tmp.Append(defMapping).Append(" IN (SELECT ").Append(joinMapping).Append(" FROM ");
                tmp.Append(w.GetTableMapping(TargetType,ser.Praefix));
                gen.AddQueryComponent(new SqlStringComponent(tmp));
                gen.AddQueryComponent(new PlaceHolderComponent());
                gen.AddQueryComponent(new SqlStringComponent(")"));
            }

            gen.AddQueryComponent(new SqlStringComponent(")"));
            return gen;

            //ret.Append(")");
            //return ret.ToString();
        }


        public FieldInfo SourceField { get { return _sourceField; } internal set { _sourceField = value; } }
        public FieldInfo TargetField { get { return _targetField; } internal set { _targetField = value; } }
        public Type TargetType
        {
            get { return _targetType; }
            internal set { _targetType = value; }
        }
        public Type SourceType
        {
            get { return _sourceType; }
            internal set { _sourceType = value; }
        }

        #region VirtualKeyTemplate Initialisierung

        internal void AddDefKeyField(String name, FieldInfo fi)
        {
            _virtualDefKeyFields[name] = fi;
        }
        internal void AddJoinKeyField(String name, FieldInfo fi)
        {
            _virtualJoinKeyFields[name] = fi;
        }
        internal void AddKeyTranslation(String definingKeyName, String joinKeyName)
        {
            _keyTranslation[definingKeyName] = joinKeyName;
        }

        #endregion

        #region FactoryMethode

        public VirtualKey CreateFromTemplate()
        {
            VirtualKey tmp = new VirtualKey();
            tmp._targetField = this._targetField;
            tmp._sourceField = this._sourceField;
            tmp._keyTranslation = this._keyTranslation;
            tmp._virtualDefKeyFields = this._virtualDefKeyFields;
            tmp._virtualJoinKeyFields = this._virtualJoinKeyFields;
            tmp.SourceType = this.SourceType;
            tmp.TargetType = this.TargetType;


            tmp._virtualKeyValues = new Dictionary<String, Object>(1);

            return tmp;
        }

        #endregion

        #region Hashcode und Equal Implementierung

        public override int GetHashCode()
        {
            if (_virtualKeyValues == null)
            {
                throw new InvalidOperationException();
            }

            int h = 0;

            foreach (Object o in _virtualKeyValues)
            {
                h = h ^ o.GetHashCode();
            }

            foreach (String key in _virtualJoinKeyFields.Keys)
            {
                h = h ^ key.GetHashCode();
            }

            foreach (String key in _virtualDefKeyFields.Keys)
            {
                h = h ^ key.GetHashCode();
            }

            return h;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return (this == (VirtualKey)obj);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public static bool operator ==(VirtualKey a, VirtualKey b)
        {
            if (Object.ReferenceEquals(a, b)) return true;

            bool r = true;

            r = r && (a.SourceType == b.SourceType);
            r = r && (a.TargetType == b.TargetType);

            try
            {
                foreach (String key in a._virtualKeyValues.Keys)
                {
                    r = r && (a._virtualKeyValues[key].Equals(b._virtualKeyValues[key]));
                }
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                foreach (String key in a._virtualDefKeyFields.Keys)
                {
                    r = r && (a._virtualDefKeyFields[key] == b._virtualDefKeyFields[key]);
                }
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                foreach (String key in a._virtualJoinKeyFields.Keys)
                {
                    r = r && (a._virtualJoinKeyFields[key] == b._virtualJoinKeyFields[key]);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return r;
        }

        public static bool operator !=(VirtualKey a, VirtualKey b)
        {
            return !(a == b);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();

            tmp.Append(typeof(VirtualKey).Name).Append(": ");
            tmp.Append(_sourceType.Name).Append(" -> ").Append(_targetType.Name);
            return tmp.ToString();
        }
    }

    public class VirtualKeyFactory
    {
        private IDictionary<Type, IDictionary<Type, VirtualKey>> _templates;
        private IDictionary<Type, List<String>> _foreignKeys;
        private AttributeWorker _worker;

        public VirtualKeyFactory(AttributeWorker worker)
        {
            _worker = worker;
            _templates = new Dictionary<Type, IDictionary<Type, VirtualKey>>();
            _foreignKeys = new Dictionary<Type, List<String>>();
        }

        internal AttributeWorker Worker
        {
            get { return _worker; }
        }

        public void RegisterType(Type type)
        {
            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(type);

            foreach (FieldInfo fi in fis)
            {
                RelationalAttribute r = AttributeWorker.GetRelationAttribute(fi);

                if (r != null)
                {
                    VirtualKey template;

                    if (r is OneToManyAttribute)
                    {
                        template = RegisterOneToManyVKey(type, fi);
                    }

                    else if (r is ManyToOneAttribute)
                    {
                        template = RegisterManyToOneVKey(type, fi);
                    }

                    else if (r is OneToOneDefAttribute)
                    {

                        if (((OneToOneDefAttribute)r).Reflexive)
                            template = RegisteReflexiveOneToOneDefVKey(ref type, fi);
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

            }
        }

        private VirtualKey RegisteReflexiveOneToOneDefVKey(ref Type type, FieldInfo fi)
        {
            VirtualKey template;
            Type navigatedType = fi.FieldType;


            template = CreateTemplateEntry(type, navigatedType);

            JoinColumsAttribute[] jcs = AttributeWorker.GetJoinColumnsAttributes(fi);
            if (jcs.Length <= 0)
            {
                throw new Serializer.SerializerException();
            }

            bool dependencyResolved = false;
            foreach (JoinColumsAttribute jc in jcs)
            {
                if (_worker.RetrieveField(type, jc.DefColumn, false) != null
                    && _worker.RetrieveField(navigatedType, jc.JoinColumn, false) != null)
                {
                    dependencyResolved = true;
                    continue;
                }
                else if (_worker.RetrieveField(navigatedType, jc.DefColumn, false) != null
                    && _worker.RetrieveField(type, jc.JoinColumn, false) != null)
                {
                    Type tmp = navigatedType;
                    navigatedType = type;
                    type = tmp;
                    dependencyResolved = true;
                    continue;
                }
            }

            if (!dependencyResolved)
            {
                throw new Serializer.SerializerException();
            }

            foreach (JoinColumsAttribute jc in jcs)
            {
                Console.WriteLine(type + ":" + _worker.RetrieveField(type, jc.DefColumn, false));
                Console.WriteLine(navigatedType + ":" + _worker.RetrieveField(navigatedType, jc.JoinColumn, false));

                FieldInfo tmp = _worker.RetrieveField(type, jc.DefColumn, false);
                if (tmp == null)
                {
                    throw new Serializer.SerializerException();
                }
                template.AddDefKeyField(jc.DefColumn, tmp);
                template.SourceType = type;


                tmp = _worker.RetrieveField(navigatedType, jc.JoinColumn, false);
                if (tmp == null)
                {
                    throw new Serializer.SerializerException();
                }
                template.AddJoinKeyField(jc.JoinColumn, tmp);
                template.TargetType = navigatedType;

                List<String> tmpList;
                try
                {
                    tmpList = _foreignKeys[navigatedType];
                }
                catch (KeyNotFoundException)
                {
                    tmpList = new List<String>();
                    _foreignKeys[navigatedType] = tmpList;
                }
                tmpList.Add(jc.JoinColumn);

                template.AddKeyTranslation(jc.DefColumn, jc.JoinColumn);
                template.SourceField = fi;
                template.TargetField = fi;
            }
            return template;
        }

        private VirtualKey RegisterOneToManyVKey(Type type, FieldInfo fi)
        {
            VirtualKey template;
            if (!fi.FieldType.IsGenericType)
            {
                throw new Serializer.SerializerException();
            }

            if (fi.FieldType.GetGenericArguments().Length != 1)
            {
                throw new Serializer.SerializerException();
            }

            if (AttributeWorker.GetMappedByAttribute(fi) != null)
            {
                throw new Serializer.SerializerException();
            }

            Type navigatedType = fi.FieldType.GetGenericArguments()[0];

            template = CreateTemplateEntry(type, navigatedType);

            JoinColumsAttribute[] jcs = AttributeWorker.GetJoinColumnsAttributes(fi);
            if (jcs.Length <= 0)
            {
                throw new Serializer.SerializerException();
            }

            foreach (JoinColumsAttribute jc in jcs)
            {
                Console.WriteLine(type + ":" + _worker.RetrieveField(type, jc.DefColumn, false));
                Console.WriteLine(navigatedType + ":" + _worker.RetrieveField(navigatedType, jc.JoinColumn, false));

                FieldInfo tmp = _worker.RetrieveField(type, jc.DefColumn, false);
                if (tmp == null)
                {
                    throw new Serializer.SerializerException();
                }
                template.AddDefKeyField(jc.DefColumn, tmp);
                template.SourceType = type;

                tmp = _worker.RetrieveField(navigatedType, jc.JoinColumn, false);
                if (tmp == null)
                {
                    throw new Serializer.SerializerException("Field " + jc.JoinColumn + " in " + navigatedType.FullName + " not found!");
                }

                template.AddJoinKeyField(jc.JoinColumn, tmp);
                template.TargetType = navigatedType;
                List<String> tmpList;
                try
                {
                    tmpList = _foreignKeys[navigatedType];
                }
                catch (KeyNotFoundException)
                {
                    tmpList = new List<String>();
                    _foreignKeys[navigatedType] = tmpList;
                }
                tmpList.Add(jc.JoinColumn);

                template.AddKeyTranslation(jc.DefColumn, jc.JoinColumn);

                template.SourceField = fi;
            }
            return template;
        }

        private VirtualKey RegisterManyToOneVKey(Type type, FieldInfo fi)
        {
            VirtualKey template;
            MappedByAttribute a = AttributeWorker.GetMappedByAttribute(fi);
            if (a != null)
            {
                Type fType = fi.FieldType;

                FieldInfo tmp = _worker.RetrieveField(fType, a.MappedBy, true);

                if (tmp == null)
                {
                    throw new Serializer.SerializerException();
                }

                RelationalAttribute rTmp = AttributeWorker.GetRelationAttribute(tmp);
                if (rTmp == null || !(rTmp is OneToManyAttribute))
                {
                    throw new Serializer.SerializerException();
                }

                if (AttributeWorker.GetJoinColumnsAttributes(fi).Length > 0)
                {
                    throw new Serializer.SerializerException();
                }

                template = CreateTemplateEntry(type, fType);
                template.TargetField = fi;
            }
            else
            {
                Type navigatedType = fi.FieldType;

                template = CreateTemplateEntry(type, navigatedType);


                JoinColumsAttribute[] jcs = AttributeWorker.GetJoinColumnsAttributes(fi);
                if (jcs.Length <= 0)
                {
                    throw new Serializer.SerializerException();
                }

                foreach (JoinColumsAttribute jc in jcs)
                {
                    Console.WriteLine(type + ":" + _worker.RetrieveField(type, jc.JoinColumn, false));
                    Console.WriteLine(navigatedType + ":" + _worker.RetrieveField(navigatedType, jc.DefColumn, false));

                    FieldInfo tmp = _worker.RetrieveField(type, jc.JoinColumn, false);
                    if (tmp == null)
                    {
                        throw new Serializer.SerializerException();
                    }

                    template.AddJoinKeyField(jc.JoinColumn, tmp);
                    template.TargetType = type;
                    List<String> tmpList;
                    try
                    {
                        tmpList = _foreignKeys[type];
                    }
                    catch (KeyNotFoundException)
                    {
                        tmpList = new List<String>();
                        _foreignKeys[type] = tmpList;
                    }
                    tmpList.Add(jc.JoinColumn);


                    tmp = _worker.RetrieveField(navigatedType, jc.DefColumn, false);
                    if (tmp == null)
                    {
                        throw new Serializer.SerializerException();
                    }

                    template.AddDefKeyField(jc.DefColumn, tmp);
                    template.SourceType = navigatedType;

                    template.AddKeyTranslation(jc.DefColumn, jc.JoinColumn);
                    template.TargetField = fi;
                }
            }

            return template;
        }



        private VirtualKey CreateTemplateEntry(Type type, Type navigatedType)
        {
            if (!_templates.ContainsKey(type))
            {
                _templates[type] = new Dictionary<Type, VirtualKey>();
            }

            if (!_templates.ContainsKey(navigatedType))
            {
                _templates[navigatedType] = new Dictionary<Type, VirtualKey>();
            }

            try
            {
                return _templates[type][navigatedType];
            }
            catch (KeyNotFoundException)
            {
                VirtualKey vKey = new VirtualKey();
                _templates[type][navigatedType] = vKey;
                _templates[navigatedType][type] = vKey;
                return vKey;
            }
        }

        internal VirtualKey GetVirtualKeyTemplate(Type source, Type target)
        {
            try
            {
                VirtualKey ret = _templates[source][target];
                return ret;
            }
            catch (KeyNotFoundException ex)
            {
                throw new Serializer.SerializerException("virtual key template not found", ex);
            }
        }

        internal List<Type> ComputeVirtualKeyPath(Type from, Type to, IList<Type> visited)
        {
            List<Type> l = new List<Type>();

            if (from == to)
            {
                l.Add(from);
                return l;
            }

            if (visited.Contains(from)) return l;

            visited.Add(from);

            IDictionary<Type, VirtualKey> d = _templates[from];

            foreach (Type tmp in d.Keys)
            {
                if (tmp == to)
                {
                    l.Add(from);
                    l.Add(tmp);
                    return l;
                }

                List<Type> l1 = ComputeVirtualKeyPath(tmp, to, visited);

                if (l1.Count > 0)
                {
                    l.Add(from);
                    l.AddRange(l1);
                    return l;
                }
            }
            return l;
        }

        internal bool IsForeignKey(FieldInfo fi)
        {
            try
            {
                return _foreignKeys[fi.DeclaringType].Contains(fi.Name);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}