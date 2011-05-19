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
using System.Reflection;
using System.Data.Common;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util
{
    public class CsvPimaryKey : IComparable
    {
        
        private String _sqlString = null;
        private String _csvKey;
        private Type _type;
        private IList<FieldInfo> _fields = new List<FieldInfo>();
        private IList<Object> _values = new List<Object>();
        

        public CsvPimaryKey(ISerializableObject iso, String target)
        {
            _type = iso.GetType();

            IDictionary<String, FieldInfo> ids = AttributeWorker.GetInstance(target).GetPrimaryKeyFields(iso.GetType());

            List<String> tmpList = new List<String>(ids.Count);
            StringBuilder csvKey = new StringBuilder();
            String tmp;

            foreach (String id in ids.Keys)
            {
                StringBuilder tmpBuilder = new StringBuilder();

                _fields.Add(ids[id]);
                _values.Add(ids[id].GetValue(iso));

                try
                {
                    //if (ids[id].GetValue(iso) is DateTime)
                    //{
                    //    DateTime dt = (DateTime)ids[id].GetValue(iso);
                    //    tmp = SqlUtil.SqlConvert(dt);
                    //}
                    //else
                    //    tmp = ids[id].GetValue(iso).ToString();
                    tmp = SqlUtil.SqlConvert(ids[id].GetValue(iso)).ToString();
                } 
                catch(NullReferenceException ex) 
                {
                    throw new KeyNotFoundException("", ex);
                }

                //Maskierung der Kommata
                tmp = tmp.Replace("/", "//");
                tmp = tmp.Replace(";", "/;");
                tmp = tmp.Replace("=", "/=");

                tmpBuilder.Append(id).Append("=");
                tmpBuilder.Append(tmp);
                tmpList.Add(tmpBuilder.ToString());
            }

            tmpList.Sort();

            bool initial = true;
            foreach (String s in tmpList)
            {
                if (!initial)
                {
                    csvKey.Append(";");
                }
                initial = false;
                csvKey.Append(s);
            }

            _csvKey = csvKey.ToString();
        }

        public CsvPimaryKey(Type type, DbDataReader reader, String target)
        {
            _type = type;

            IDictionary<String, FieldInfo> ids = AttributeWorker.GetInstance(target).GetPrimaryKeyFields(type);
            List<String> tmpList = new List<String>(ids.Count);
            StringBuilder csvKey = new StringBuilder();

            foreach (String id in ids.Keys)
            {
                StringBuilder tmpBuilder = new StringBuilder();

                _fields.Add(ids[id]);
                _values.Add(reader[id]);

                String tmp =SqlUtil.SqlConvert(reader[id]).ToString();

                //Maskierung der Kommata
                tmp = tmp.Replace("/", "//");
                tmp = tmp.Replace(";", "/;");
                tmp = tmp.Replace("=", "/=");

                tmpBuilder.Append(id).Append("=");
                tmpBuilder.Append(tmp);
                tmpList.Add(tmpBuilder.ToString());
            }

            tmpList.Sort();

            bool initial = true;
            foreach (String s in tmpList)
            {
                if (!initial)
                {
                    csvKey.Append(";");
                }
                initial = false;
                csvKey.Append(s);
            }

            _csvKey = csvKey.ToString();
        }

        public CsvPimaryKey(Type type, String csvKey, String target) 
        {
            _csvKey = csvKey;
            _type = type;
            IDictionary<String, FieldInfo> ids = AttributeWorker.GetInstance(target).GetPrimaryKeyFields(type);
            String[] split = Split(_csvKey, ";", "/;");

            foreach (String s in split)
            {
                String[] tmp = Split(s, "=", "/=");
                FieldInfo f = ids[tmp[0]];

                tmp[1] = tmp[1].Replace("/;", ";");
                tmp[1] = tmp[1].Replace("/=", "=");
                tmp[1] = tmp[1].Replace("//", "/");
                
                Object val;
                if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    Type t = Nullable.GetUnderlyingType(f.FieldType); 
                    val = Convert.ChangeType(tmp[1], t, null);
                } 
                else
                {
                    val = Convert.ChangeType(tmp[1], f.FieldType, null);
                }

                _fields.Add(f);
                _values.Add(val);
            }

        }

        public Type CSVType
        {
            get { return _type; }
        }

        public void Apply(ISerializableObject iso)
        {
            if (iso.GetType() != _type)
            {
                throw new SerializerException();
            }

            for (int i = 0; i < _fields.Count; i++)
            {
                _fields[i].SetValue(iso, _values[i]);
            }
        }

        public override string ToString()
        {
            return _csvKey;
        }

        public String ToSqlString()
        {
            if (_sqlString != null) return _sqlString;

            String[] keys = Split(_csvKey, ";", "/;");

            StringBuilder b = new StringBuilder();
            bool start = true;
            foreach (String s in keys)
            {
                if (!start)
                {
                    b.Append(" AND ");
                }
                start = false;
                String[] tmp = Split(s, "=", "/=");

                tmp[1] = tmp[1].Replace("/;", ";");
                tmp[1] = tmp[1].Replace("/=", "=");
                tmp[1] = tmp[1].Replace("//", "/");

                b.Append(tmp[0]).Append("=").Append(tmp[1]);
            }

            _sqlString = b.ToString();

            return _sqlString;
        }

        private String[] Split(String split, String seq, String except)
        {
            List<String> ret = new List<String>();
            String tmp;
            int i = -1;
            int start = 0;


            while ((i = split.IndexOf(seq, i + 1)) != -1)
            {

                try
                {
                    if ((split.IndexOf(except, i - 1) + (except.Length-1)) == i) continue;
                }
                catch (ArgumentOutOfRangeException)
                {

                }

                tmp = split.Substring(start, i - start);
                if (tmp.Length > 0)
                {

                    ret.Add(tmp);
                    //Console.WriteLine(tmp);
                }
                start = i + 1;
            }


            tmp = split.Substring(start, split.Length - start);
            if (tmp.Length > 0)
            {

                ret.Add(tmp);
                //Console.WriteLine(tmp);
            }

            return ret.ToArray();
        }

        public int CompareTo(Object o)
        {
            return o.ToString().CompareTo(ToString());
        }

        public override bool Equals(object obj)
        {
            try
            {
                return this == (CsvPimaryKey)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _csvKey.GetHashCode() ^ _type.GetHashCode();
        }

        public static bool operator ==(CsvPimaryKey a, CsvPimaryKey b)
        {
            

            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            try
            {
                if (a is CsvPimaryKey)
                {
                    return
                        ((CsvPimaryKey)a)._csvKey.Equals(b._csvKey) &&
                        ((CsvPimaryKey)a)._type == b._type;
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }

            return false;
        }

        public static bool operator !=(CsvPimaryKey a, CsvPimaryKey b)
        {
            return !(a == b);
        }
    }
}
