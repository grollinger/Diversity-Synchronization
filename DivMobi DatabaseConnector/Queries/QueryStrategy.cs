using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public abstract class QueryStrategy
    {
        protected ISerializableObject data;
        protected string table;
        protected PropertyInfo[] properties;

        protected QueryStrategy(ISerializableObject data)
        {
            this.data = data;
            table = AttributeUtilities.GetTableMapping(data);
            properties = data.GetType().GetProperties();
        }

        protected List<String> GetConstrainingAttributes(String op)
        {
            List<string> list = new List<string>();
            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute attribute =
                    (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

                
                if (attribute != null)
                {
                    MethodInfo method = property.GetGetMethod();
                    Object ret = method.Invoke(data, null);
                    SqlConvert(ref ret);
                    if (data.ContainsConstrainingAttribute(QueryStrategyConfiguration.PRIMARYKEY) && attribute is IPrimaryKeyAttribute)
                    {
                        StringBuilder tmp = new StringBuilder().Append(AttributeUtilities.GetColumnMapping(property));
                        string equal = " "+op+" ";
                        if (ret.ToString().Equals("NULL"))
                        {
                            equal = " IS ";
                        }

                        tmp.Append(equal).Append(ret.ToString());
                        list.Add(tmp.ToString());
                    }
                    else if (
                        data.ContainsConstrainingAttribute(property.Name) || 
                        data.ContainsConstrainingAttribute(AttributeUtilities.GetColumnMapping(property))
                        )
                    {
                        StringBuilder tmp = new StringBuilder().Append(AttributeUtilities.GetColumnMapping(property));
                        string equal = " " + op + " ";
                        if (ret.ToString().Equals("NULL"))
                        {
                            equal = " IS ";
                        }

                        tmp.Append(equal).Append(ret.ToString());
                        list.Add(tmp.ToString());
                    }
                }
            }
            return list;
        }
             
        protected static void SqlConvert(ref Object o) {
            if (o == null)
            {
                o = "NULL";
            }
            else if (o is DateTime)
            {
                StringBuilder tmp = new StringBuilder();
                DateTime d = (DateTime)o;
                tmp.Append("'").Append(
                    d.ToString("yyyy-MM-dd HH:mm:ss", 
                    System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))).Append("'");
                o = tmp.ToString();
            }
            else
            {
                StringBuilder tmp = new StringBuilder();
                tmp.Append("'").Append(o.ToString()).Append("'");
                o = tmp.ToString();
            }
        }

    }
}
