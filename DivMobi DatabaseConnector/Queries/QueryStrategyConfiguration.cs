using System;
using System.Collections.Generic;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public class QueryStrategyConfiguration : IQueryStrategyConfiguration
    {
        public const string PRIMARYKEY = "$$$$$$$$";

        private List<string> _constrainingAttributes;
        private string _deserializationStrategy;
        private string _serializationStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdTextCreationConf"/> class.
        /// </summary>
        public QueryStrategyConfiguration()
        {
            this._constrainingAttributes = new List<string>();
            this._deserializationStrategy = DeserializationQueryStrategy.DEFAULT;
            this._serializationStrategy = SerializationQueryStrategy.DEFAULT;
        }

        /// <summary>
        /// Adds a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public void AddConstrainingAttribute(string constraint)
        {
            this._constrainingAttributes.Add(constraint);
        }

        /// <summary>
        /// Removes a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public void RemoveConstrainingAttribute(string constraint)
        {
            this._constrainingAttributes.Remove(constraint);
        }

        /// <summary>
        /// Clears all constraining attributes.
        /// </summary>
        public void ClearConstrainingAttributes()
        {
            _constrainingAttributes.Clear();
        }

        /// <summary>
        /// Determines whether this ISerializableObject object contains the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>
        /// 	<c>true</c> if this ISerializableObject object contains the specified constraint; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsConstrainingAttribute(string constraint)
        {
            return this._constrainingAttributes.Contains(constraint);
        }

        /// <summary>
        /// Gets or sets the deserialization strategy.
        /// </summary>
        /// <value>The deserialization strategy.</value>
        public string DeserializationStrategy { get { return this._deserializationStrategy; } set { this._deserializationStrategy = value; } }

        /// <summary>
        /// Gets or sets the serialization strategy.
        /// </summary>
        /// <value>The serialization strategy.</value>
        public string SerializationStrategy { get { return this._serializationStrategy; } set { this._serializationStrategy = value; } }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public Object Clone()
        {
            QueryStrategyConfiguration tmp = (QueryStrategyConfiguration)this.MemberwiseClone();
            List<string> tmpConstrainingAttributes = new List<string>();
            foreach (string s in _constrainingAttributes)
            {
                tmpConstrainingAttributes.Add(s);
            }
            tmp._constrainingAttributes = tmpConstrainingAttributes;
            tmp._deserializationStrategy = (string)this._deserializationStrategy.Clone();
            return tmp;
        }
    }
}
