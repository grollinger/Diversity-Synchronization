using System;
using System.Reflection;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    public class AttributeUtilities
    {
        /// <summary>
        /// Returns the table name for the passed object.
        /// </summary>
        /// <param name="data">The ISerializableObject object.</param>
        /// <returns>string</returns>
        public static string GetTableMapping(ISerializableObject data) 
        {
            Attribute a = Attribute.GetCustomAttribute(data.GetType(), typeof(TableAttribute));
            string ret = null;
            if (a != null) ret = ((TableAttribute)a).TableMapping;
            if (ret == null) ret = data.GetType().Name;
            return ret;
        }

        /// <summary>
        /// Returns the column name for the passed property.
        /// </summary>
        /// <param name="p">The PropertyInfo object.</param>
        /// <returns>string</returns>
        public static string GetColumnMapping(PropertyInfo p)
        {
            ColumnAttribute tmp = GetColumnAttribute(p);
            if(tmp == null) return null;
            string ret = tmp.ColumnMapping;
            if (ret == null) ret = p.Name;
            return ret;
        }

        /// <summary>
        /// Returns the column attribute for the passed property.
        /// </summary>
        /// <param name="p">The PropertyInfo object.</param>
        /// <returns>ColumnAttribute</returns>
        public static ColumnAttribute GetColumnAttribute(PropertyInfo p)
        {
            return (ColumnAttribute)Attribute.GetCustomAttribute(p, typeof(ColumnAttribute));
        }

        public static string[] TryGetForeignKeyMapping(PropertyInfo p)
        {
            IForeignKeyAttribute f = GetForeignKeyAttribute(p);
            if (f == null)
            {
                return null;
            }
            else
            {
                string[] ret = f.ForeignKeyMapping;
                if (ret == null)
                {
                    ret = new string[] { p.Name };
                }
                else
                {
                    string[] tmp = ret;
                    ret = new string[ret.Length+1];
                    System.Array.Copy(tmp, ret, tmp.Length);
                    ret[ret.Length - 1] = p.Name;
                }
                return ret;
            }
        }

        public static IForeignKeyAttribute GetForeignKeyAttribute(PropertyInfo p)
        {
            ColumnAttribute tmp = (ColumnAttribute)Attribute.GetCustomAttribute(p, typeof(ColumnAttribute));
            if (tmp is IForeignKeyAttribute)
            {
                return (IForeignKeyAttribute)tmp;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Gets the AutoIncPrimaryKey setter.
        /// </summary>
        /// <param name="data">The ISerializableObject object.</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetAutoIncPrimaryKeySetter(ISerializableObject data)
        {
            PropertyInfo[] properties = data.GetType().GetProperties();

            foreach (PropertyInfo p in properties)
            {
                ColumnAttribute attribute = GetColumnAttribute(p);

                if (attribute != null && attribute is AutoIncPrimaryKeyAttribute)
                {
                    return p.GetSetMethod();
                }
            }
            
            return null;
        }

        /// <summary>
        /// Determines whether the ISerializableObject object contains an AutoIncPrimaryKey.
        /// </summary>
        /// <param name="data">The ISerializableObject object.</param>
        /// <returns>
        /// 	<c>true</c> if the ISerializableObject object contains an AutoIncPrimaryKey; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAutoIncPrimaryKey(ISerializableObject data)
        {
            PropertyInfo[] properties = data.GetType().GetProperties();

            foreach (PropertyInfo p in properties)
            {
                ColumnAttribute attribute = GetColumnAttribute(p);

                if (attribute != null && attribute is AutoIncPrimaryKeyAttribute)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
