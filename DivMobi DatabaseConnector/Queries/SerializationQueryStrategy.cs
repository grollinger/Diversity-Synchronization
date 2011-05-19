using System;
using System.Reflection;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public abstract class SerializationQueryStrategy : QueryStrategy
    {
        public const string UPDATE_IF_DESERIALIZED = "upifde";
        public const string INSERT = "insert";
        public const string REMOVE = "remove";
        public const string UPDATE_BY_PRIMARYKEY = "upbypk";

        public const string DEFAULT = UPDATE_IF_DESERIALIZED;

        protected SerializationQueryStrategy(ISerializableObject data) : base(data) {}

        public static SerializationQueryStrategy CreateStrategy(ISerializableObject data, SerializationMetaData metaData)
        {
            switch (data.SerializationStrategy)
            {
                case DEFAULT:
                    return new UpdateIfDeserializedSerializationQueryStrategy(data, metaData);
                case UPDATE_BY_PRIMARYKEY:
                    return new UpdateByPrimaryKeySerializationQueryStrategy(data);
                case INSERT:
                    return new InsertSerializationQueryStrategy(data);
                case REMOVE:
                    return new RemoveSerializationQueryStrategy(data);
            }

            throw new StrategyNotFoundException("Serialization strategy \""+data.SerializationStrategy+"\" not found");
            
        }

        public abstract string CreateSerializationQuery();
    }

    class InsertSerializationQueryStrategy : SerializationQueryStrategy
    {
        public InsertSerializationQueryStrategy(ISerializableObject data) : base(data) {}

        public override string CreateSerializationQuery()
        {
            StringBuilder queryPt1 = new StringBuilder();
            queryPt1.Append("INSERT INTO ").Append(base.table).Append(" (");
            StringBuilder queryPt2 = new StringBuilder();
            queryPt2.Append(") VALUES (");


            bool start = true;
            for (int i = 0; i < base.properties.Length; i++)
            {
                ColumnAttribute attribute = AttributeUtilities.GetColumnAttribute(base.properties[i]);

                if(attribute != null && !(attribute is AutoIncPrimaryKeyAttribute)) {
                    if (!start)
                    {
                        queryPt1.Append(", ");
                        queryPt2.Append(", ");

                    }
                    start = false;

                    MethodInfo m = base.properties[i].GetGetMethod();
                    Object ret = m.Invoke(base.data, null);

                    if(attribute is IPrimaryKeyAttribute && ret == null) 
                    {
                        throw new SerializerException("All non-auto-incrementing primary keys must be set for InsertSerializationQueryStrategy to work!");
                    }

                    SqlConvert(ref ret);

                    queryPt1.Append(AttributeUtilities.GetColumnMapping(properties[i]));
                    queryPt2.Append(ret.ToString());
                }
            }

            queryPt2.Append(")");

            return queryPt1.Append(queryPt2).ToString();
        }
    }

    class RemoveSerializationQueryStrategy : SerializationQueryStrategy
    {
        public RemoveSerializationQueryStrategy(ISerializableObject data) : base(data) { }

        public override string CreateSerializationQuery()
        {
            StringBuilder queryPt1 = new StringBuilder();
            queryPt1.Append("DELETE FROM ").Append(base.table).Append(" WHERE (");


            bool start = true;
            for (int i = 0; i < base.properties.Length; i++)
            {
                ColumnAttribute attribute = AttributeUtilities.GetColumnAttribute(base.properties[i]);

                if (attribute is IPrimaryKeyAttribute)
                {
                    if(!start)
                        queryPt1.Append(" AND ");
                    
                    MethodInfo m = base.properties[i].GetGetMethod();
                    Object ret = m.Invoke(base.data, null);

                    if (ret == null)
                    {
                        throw new SerializerException("All primary keys must be set for RemoveSerializationQueryStrategy to work!");
                    }

                    SqlConvert(ref ret);

                    queryPt1.Append(AttributeUtilities.GetColumnMapping(properties[i]));
                    queryPt1.Append(" = ");
                    queryPt1.Append(ret.ToString());
                    start = false;
                }
            }

            queryPt1.Append(")");

            return queryPt1.ToString();
        }
    }

    class UpdateByPrimaryKeySerializationQueryStrategy : SerializationQueryStrategy
    {
        public UpdateByPrimaryKeySerializationQueryStrategy(ISerializableObject data) : base(data) {}

        public override string CreateSerializationQuery()
        {
            StringBuilder queryPt1 = new StringBuilder();
            StringBuilder queryPt2 = new StringBuilder();

            queryPt1.Append("UPDATE ").Append(base.table).Append(" SET ");
            queryPt2.Append(" WHERE ");

            bool startPt1 = true;
            bool startPt2 = true;
            for (int i = 0; i < base.properties.Length; i++)
            {
                ColumnAttribute attribute = AttributeUtilities.GetColumnAttribute(base.properties[i]);
                
                if (attribute != null)
                {
                    Object ret = base.properties[i].GetGetMethod().Invoke(base.data, null);
                    if (!(attribute is IPrimaryKeyAttribute) && ret != null)
                    {
                        if (!startPt1)
                        {
                            queryPt1.Append(", ");
                        }
                        startPt1 = false;
                        SqlConvert(ref ret);
                        queryPt1.Append(AttributeUtilities.GetColumnMapping(base.properties[i])).Append(" = ").Append(ret);
                    }
                    else if (attribute is IPrimaryKeyAttribute && ret != null)
                    {
                        if (!startPt2)
                        {
                            queryPt2.Append(" AND ");
                        }
                        startPt2 = false;
                        SqlConvert(ref ret);
                        queryPt2.Append(AttributeUtilities.GetColumnMapping(base.properties[i])).Append(" = ").Append(ret);
                    }
                    else if (attribute is IPrimaryKeyAttribute && ret == null)
                    {
                        throw new SerializerException("All primary keys must be set for UpdateByPrimaryKeySerializationStrategy to work!");
                    }
                }

            }

            return queryPt1.Append(queryPt2).ToString();
        }
    }

    class UpdateIfDeserializedSerializationQueryStrategy : SerializationQueryStrategy
    {
        private SerializationMetaData _metaData;

        public UpdateIfDeserializedSerializationQueryStrategy(ISerializableObject data, SerializationMetaData metaData) : base(data)
        {
            this._metaData = metaData;
        }

        public override string CreateSerializationQuery()
        {
            if (_metaData.Persistent)
            {
                UpdateByPrimaryKeySerializationQueryStrategy tmp = new UpdateByPrimaryKeySerializationQueryStrategy(data);
                return tmp.CreateSerializationQuery();
            }
            else
            {
                InsertSerializationQueryStrategy tmp = new InsertSerializationQueryStrategy(data);
                return tmp.CreateSerializationQuery();
            }
        }
    }
}
