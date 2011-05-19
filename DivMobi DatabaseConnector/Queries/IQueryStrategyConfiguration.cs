using System;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries
{
    public interface IQueryStrategyConfiguration : ICloneable
    {
        /// <summary>
        /// Adds a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        void AddConstrainingAttribute(string constraint);

        /// <summary>
        /// Removes a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        void RemoveConstrainingAttribute(string constraint);

        /// <summary>
        /// Clears all constraining attributes.
        /// </summary>
        void ClearConstrainingAttributes();

        /// <summary>
        /// Determines whether this ISerializableObject object contains the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>
        /// 	<c>true</c> if this ISerializableObject object contains the specified constraint; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsConstrainingAttribute(string constraint);

        /// <summary>
        /// Gets or sets the deserialization strategy.
        /// </summary>
        /// <value>The deserialization strategy.</value>
        string DeserializationStrategy { get; set; }

        /// <summary>
        /// Gets or sets the serialization strategy.
        /// </summary>
        /// <value>The serialization strategy.</value>
        string SerializationStrategy { get; set; }
    }
}
