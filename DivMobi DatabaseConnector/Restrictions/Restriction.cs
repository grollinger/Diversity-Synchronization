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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions
{
    public interface IRestriction
    {
        String ToSqlString(Serializer.Serializer serializer, Type type, String target);
    }

    public class RestrictionException : Exception
    {
        private int _errorCode;

        public RestrictionException(String message)
            : base(message)
        {
            _errorCode = -1;
        }

        public RestrictionException(String message, int errorCode)
            : base(message)
        {
            _errorCode = errorCode;
        }


        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }
    }

    public class RestrictionFactory
    {
        public static IRestriction SqlRestriction(Type type, String sqlString)
        {
            return new SqlRestriction(type, sqlString);
        }

        public static IRestriction CsvKeyRestriction(Type type, CsvPimaryKey csvKey)
        {
            return new CsvKeyRestriction(type, csvKey);
        }

        public static IRestriction TypeRestriction(Type type)
        {
            return new TypeRestriction(type);
        }

        public static LogicalRestriction Or()
        {
            return new OrRestriction();
        }

        public static LogicalRestriction And()
        {
            return new AndRestriction();
        }

        public static IRestriction Not(IRestriction r)
        {
            return new NotRestriction(r);
        }

        public static IRestriction Parenthesize(IRestriction r)
        {
            Parenthesis p = new Parenthesis(r);
            return p;
        }

        public static IRestriction Eq(Type t, String property, Object value)
        {
            return new EqualRestriction(t, property, value);
        }

        public static IRestriction Gt(Type t, String property, Object value)
        {
            return new GreaterThanRestriction(t, property, value);
        }

        public static IRestriction Lt(Type t, String property, Object value)
        {
            return new LessThanRestriction(t, property, value);
        }

        public static IRestriction Like(Type t, String property, String value)
        {
            return new LikeRestriction(t, property, value);
        }

        public static IRestriction Btw(Type t, String property, Object value1, Object value2)
        {
            return new BetweenRestriction(t, property, value1, value2);
        }
    }

    internal abstract class BaseRestriction : IRestriction
    {
        public abstract string ToSqlString(Serializer.Serializer serializer, Type type, String target);

        protected StringBuilder ComputeSqlJoin(Serializer.Serializer serializer, IList<Type> path, String restriction, String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            QueryGenerator ret = new QueryGenerator();
            ret.AddQueryComponent(new PlaceHolderComponent());

            //StringBuilder b = new StringBuilder();
            //b.Append(" @@@");

            for (int i = 0; i < path.Count - 1; i++)
            {
                Type from = path[i];
                Type to = path[i + 1];
                //if (i > 0)
                //{
                //    b.Replace(" @@@", " WHERE  @@@");
                //}

                IVirtualKey vKey = w.CreateVirtualKey(serializer, from, to);
                QueryGenerator gen1 = vKey.ToSqlRestriction(target, from, to, serializer);
                ret.ReplacePlaceHolders(gen1);
                //b.Replace(" @@@", vKey.ToSqlRestriction(target, from, to));

                if (i==path.Count-2)
                {
                    QueryGenerator gen2 = new QueryGenerator();
                    gen2.AddQueryComponent(new SqlStringComponent(" WHERE "));
                    gen2.AddQueryComponent(new SqlStringComponent(restriction));
                    ret.ReplacePlaceHolders(gen2);
                    
                    //b.Replace(" @@@", " WHERE  @@@");
                    //b.Replace(" @@@", restriction);
                }
            }

            if (path.Count == 1)
            {
                QueryGenerator gen3 = new QueryGenerator();
                gen3.AddQueryComponent(new SqlStringComponent(" WHERE "));
                gen3.AddQueryComponent(new SqlStringComponent(restriction));
                ret.ReplacePlaceHolders(gen3);

                //b.Replace(" @@@", " WHERE  @@@");
                //b.Replace(" @@@", restriction);
            }


            //Entferne das führende " WHERE "

            return ret.ToSqlString();

            //return b.Remove(0,7);
        }
        
    }

    internal class TypeRestriction : BaseRestriction
    {
        private Type _type;

        public TypeRestriction(Type type)
        {
            _type = type;
        }

        public override String ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            IList<Type> path = w.ComputeVirtualKeyPath(serializer, _type, type);

            return ComputeSqlJoin(serializer, path, "0=0", target).ToString();
        }
    }

    internal class SqlRestriction : BaseRestriction
    {
        private String _sqlString;
        private Type _type;

        public SqlRestriction(Type type, String sqlString)
        {
            _sqlString = sqlString;
            _type = type;
        }

        public override String ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            IList<Type> path = w.ComputeVirtualKeyPath(serializer, _type, type); //ComputePath(type, _type, target);

            return ComputeSqlJoin(serializer, path, _sqlString, target).ToString();
        }
    }


    internal class CsvKeyRestriction : BaseRestriction
    {
        private CsvPimaryKey _csvKey;
        private Type _type;

        public CsvKeyRestriction(Type type, CsvPimaryKey csvKey)
        {
            _csvKey = csvKey;
            _type = type;
        }

        public override String ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            IList<Type> path = w.ComputeVirtualKeyPath(serializer, _type, type);


            return ComputeSqlJoin(serializer, path, _csvKey.ToSqlString(), target).ToString();
        }
    }

    internal abstract class ComparerRestriction : BaseRestriction
    {
        protected Type _type;
        protected String _field;
        protected Object _value;
        protected String _comparer;

        public ComparerRestriction(Type t, String field, Object value)
        {
            _type = t;
            _field = field;
            _value = value;
        }

        public override String ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);
            IList<Type> path = w.ComputeVirtualKeyPath(serializer, _type, type); //ComputePath(_type, type, target);

            StringBuilder b = new StringBuilder();
            FieldInfo f = w.RetrieveField(_type, _field, false);
            if (f == null)
            {
                throw new RestrictionException("Field '" + _field + "' not found in '" + _type.ToString() + "'");
            }
            b.Append(w.GetColumnMapping(f));
            b.Append(_comparer).Append(SqlUtil.SqlConvert(_value));

            String tmp = ComputeSqlJoin(serializer, path, b.ToString(), target).ToString();
            return tmp;
            
        }

        internal Type NavType
        {
            get { return _type; }
        }
    }

    internal class EqualRestriction : ComparerRestriction
    {
        internal EqualRestriction(Type t, String property, Object value)
            : base(t, property, value)
        {
            if (value == null) _comparer = " is ";
            else _comparer = "=";
        }
    }

    internal class GreaterThanRestriction : ComparerRestriction
    {
        internal GreaterThanRestriction(Type t, String property, Object value)
            : base(t, property, value)
        {
            _comparer = ">";
        }
    }

    internal class LessThanRestriction : ComparerRestriction
    {
        internal LessThanRestriction(Type t, String property, Object value)
            : base(t, property, value)
        {
            _comparer = "<";
        }
    }

    internal class LikeRestriction : ComparerRestriction
    {
        internal LikeRestriction(Type t, String field, Object value)
            : base(t, field, value)
        {
            _comparer = " LIKE ";
        }

        public override string ToSqlString(Serializer.Serializer serializer, Type type, string target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);
            FieldInfo f = w.RetrieveField(_type, _field, false);

            if (f == null)
            {
                throw new RestrictionException("Field '" + _field + "' not found in '" + _type.ToString() + "'");
            }

            if (!(_value is String) || f.FieldType != typeof(String))
            {
                throw new RestrictionException("LIKE operator can only applied to Strings");
            }
            return base.ToSqlString(serializer, type, target);
        }
    }

    internal class BetweenRestriction : ComparerRestriction
    {
        private Object _value2;

        public BetweenRestriction(Type t, String property, Object value1, Object value2)
            : base(t, property, value1)
        {
            _value2 = value2;
        }

        public override string ToSqlString(Serializer.Serializer serializer, Type type, string target)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);
            IList<Type> path = w.ComputeVirtualKeyPath(serializer, _type, type); 


            StringBuilder b = new StringBuilder();
            FieldInfo f = w.RetrieveField(type, _field, false);

            if (f == null)
            {
                throw new RestrictionException("Field '" + _field + "' not found in '" + _type.ToString() + "'");
            }

            

            b.Append(w.GetColumnMapping(f));
            b.Append(" BETWEEN ").Append(SqlUtil.SqlConvert(_value));
            b.Append(" AND ").Append(SqlUtil.SqlConvert(_value2));

            return ComputeSqlJoin(serializer, path, b.ToString(), target).ToString();
        }
    }

    internal class NotRestriction : IRestriction
    {
        private IRestriction _restriction;

        internal NotRestriction(IRestriction restriction)
        {
            _restriction = restriction;
        }

        public String ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            StringBuilder b = new StringBuilder();

            b.Append("NOT (").Append(_restriction.ToSqlString(serializer, type, target)).Append(") ");

            return b.ToString();
        }
    }

    internal class Parenthesis : IRestriction
    {
        IRestriction _restriction;

        public Parenthesis(IRestriction r)
        {
            _restriction = r;
        }

        public string ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append("(");
            tmp.Append(_restriction.ToSqlString(serializer, type, target));
            tmp.Append(") ");
            return tmp.ToString();
        }


    }

    public abstract class LogicalRestriction : IRestriction
    {
        protected String _connector;
        protected List<IRestriction> _restriction;

        public LogicalRestriction()
        {
            _restriction = new List<IRestriction>();
        }


        public LogicalRestriction Add(IRestriction r)
        {
            _restriction.Add(r);
            return this;
        }

        public virtual string ToSqlString(Serializer.Serializer serializer, Type type, String target)
        {
            StringBuilder b = new StringBuilder();
            bool start = true;
            foreach (IRestriction r in _restriction)
            {
                if (!start)
                {
                    b.Append(_connector);
                    b.Append(r.ToSqlString(serializer, type, target).Remove(0,7));
                } else {
                    start = false;
                    b.Append(r.ToSqlString(serializer, type, target));
                }
            }
            return b.ToString();
        }
    }

    internal class AndRestriction : LogicalRestriction
    {
        public AndRestriction()
            : base()
        {
            _connector = " AND ";
        }
    }

    internal class OrRestriction : LogicalRestriction
    {
        public OrRestriction()
            : base()
        {
            _connector = " OR ";
        }
    }
}