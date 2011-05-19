using System;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public abstract class DeserializationQueryStrategy : QueryStrategy
    {
        public const string DEFAULT = "default";
        public const string ALL_AND = DEFAULT;
        public const string ALL_AND_IS_LIKE = "all_and_is_like";
        public const string ALL_OR_IS_LIKE = "all_or_is_like";


        protected DeserializationQueryStrategy(ISerializableObject data) : base(data) {}

        public static DeserializationQueryStrategy CreateStrategy(ISerializableObject data)
        {
            switch (data.DeserializationStrategy)
            {
                case ALL_AND:
                    return new AllAndDeserializationQueryCreationStrategy(data);
                case ALL_AND_IS_LIKE:
                    return new AllAndIsLikeDeserializationQueryCreationStrategy(data);
                case ALL_OR_IS_LIKE:
                    return new AllOrIsLikeDeserializationQueryCreationStrategy(data);
            }

            throw new StrategyNotFoundException("Deserialization strategy \"" + data.DeserializationStrategy + "\" not found");
        }

        public abstract String CreateDeserializationQuery();

    }

    class AllAndDeserializationQueryCreationStrategy : DeserializationQueryStrategy
    {
        public AllAndDeserializationQueryCreationStrategy(ISerializableObject data) : base(data) { }

        public override string CreateDeserializationQuery()
        {
            StringBuilder ret = new StringBuilder("SELECT * FROM ").Append(base.table);
            List<String> list = base.GetConstrainingAttributes("=");
            if (list.Count != 0)
            {
                ret.Append(" WHERE (");
                bool start = true;
                foreach (String constraint in list)
                {
                    if (!start)
                    {
                        ret.Append(" AND (");
                    }
                    start = false;
                    ret.Append(constraint).Append(")");
                }
            }
            return ret.ToString();
        }
    }

    class AllAndIsLikeDeserializationQueryCreationStrategy : DeserializationQueryStrategy
    {
        public AllAndIsLikeDeserializationQueryCreationStrategy(ISerializableObject data) : base(data) { }

        public override string CreateDeserializationQuery()
        {
            StringBuilder ret = new StringBuilder("SELECT * FROM ").Append(base.table);
            List<String> list = base.GetConstrainingAttributes("LIKE");
            if (list.Count != 0)
            {
                ret.Append(" WHERE (");
                bool start = true;
                foreach (String constraint in list)
                {
                    if (!start)
                    {
                        ret.Append(" AND (");
                    }
                    start = false;
                    ret.Append(constraint).Append(")");
                }
            }
            return ret.ToString();
        }
    }

    class AllOrIsLikeDeserializationQueryCreationStrategy : DeserializationQueryStrategy
    {
        public AllOrIsLikeDeserializationQueryCreationStrategy(ISerializableObject data) : base(data) { }

        public override string CreateDeserializationQuery()
        {
            StringBuilder ret = new StringBuilder("SELECT * FROM ").Append(base.table);
            List<String> list = base.GetConstrainingAttributes("LIKE");
            if (list.Count != 0)
            {
                ret.Append(" WHERE (");
                bool start = true;
                foreach (String constraint in list)
                {
                    if (!start)
                    {
                        ret.Append(" OR (");
                    }
                    start = false;
                    ret.Append(constraint).Append(")");
                }
            }
            return ret.ToString();
        }
    }
}
