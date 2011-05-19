using System;
using System.Reflection;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Queries;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable
{
    public abstract class SerializableObject : ISerializableObject
    {

        private IQueryStrategyConfiguration _configurationDelegate;

        public SerializableObject()
        {
            this._configurationDelegate = null;
        }

        #region implementation of IQueryStrategyConfiguration
        /// <summary>
        /// Adds a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public void AddConstrainingAttribute(string constraint)
        {
            InitConfigurationDelegateIfRequired();
            this._configurationDelegate.AddConstrainingAttribute(constraint);
        }

        /// <summary>
        /// Removes a constraining attribute.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public void RemoveConstrainingAttribute(string constraint)
        {
            InitConfigurationDelegateIfRequired();
            this._configurationDelegate.RemoveConstrainingAttribute(constraint);
        }

        /// <summary>
        /// Clears a constraining attributes.
        /// </summary>
        public void ClearConstrainingAttributes()
        {
            InitConfigurationDelegateIfRequired();
            this._configurationDelegate.ClearConstrainingAttributes();
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
            InitConfigurationDelegateIfRequired();
            return this._configurationDelegate.ContainsConstrainingAttribute(constraint);
        }

        /// <summary>
        /// Gets or sets the deserialization strategy.
        /// </summary>
        /// <value>The deserialization strategy.</value>
        public string DeserializationStrategy 
        { 
            get {
                InitConfigurationDelegateIfRequired();
                return this._configurationDelegate.DeserializationStrategy; 
            }
            set {
                InitConfigurationDelegateIfRequired();
                this._configurationDelegate.DeserializationStrategy = value;  
            }
        }

        /// <summary>
        /// Gets or sets the serialization strategy.
        /// </summary>
        /// <value>The serialization strategy.</value>
        public string SerializationStrategy
        {
            get
            {
                InitConfigurationDelegateIfRequired();
                return this._configurationDelegate.SerializationStrategy;
            }
            set
            {
                InitConfigurationDelegateIfRequired();
                this._configurationDelegate.SerializationStrategy = value;
            }
        }
        #endregion

        #region implementation of ISerializableObject

        /// <summary>
        /// Gets or sets the query strategy configuration.
        /// </summary>
        /// <value>The query strategy configuration.</value>
        public IQueryStrategyConfiguration QueryStrategyConfiguration 
        {
            get {
                InitConfigurationDelegateIfRequired();
                return (IQueryStrategyConfiguration)this._configurationDelegate.Clone();  
            }
            set { this._configurationDelegate = (IQueryStrategyConfiguration)value.Clone(); }
        }


        #endregion

        #region public methods

        /// <summary>
        /// Gets or sets the shared query strategy configuration.
        /// </summary>
        /// <value>The shared query strategy configuration.</value>
        public IQueryStrategyConfiguration SharedQueryStrategyConfiguration
        {
            get
            {
                InitConfigurationDelegateIfRequired();
                return this._configurationDelegate;
            }
            set { this._configurationDelegate = value; }
        }

        public Object Clone()
        {
            throw new NotSupportedException();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            PropertyInfo[] properties = this.GetType().GetProperties();
            PropertyInfo[] compProperties = obj.GetType().GetProperties();

            if(properties.Length != compProperties.Length) 
            {
                return false;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                ColumnAttribute attribute = AttributeUtilities.GetColumnAttribute(properties[i]);
                ColumnAttribute compAttribute = AttributeUtilities.GetColumnAttribute(compProperties[i]);

                if (attribute != null)
                {
                    if (compAttribute == null)
                    {
                        return false;
                    }
                    else
                    {
                        MethodInfo m1 = properties[i].GetGetMethod();
                        MethodInfo m2 = compProperties[i].GetGetMethod();

                        Object o1 = m1.Invoke(this, null);
                        Object o2 = m2.Invoke(obj, null);

                        if (o1 == null && o2 == null)
                        {
                            continue;
                        }

                        if((o1 == null && o2 != null) || (o1 != null && o2 == null))
                        {
                            return false;
                        }

                        if (!o1.Equals(o2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region private helpers
        private void InitConfigurationDelegateIfRequired()
        {
            if (this._configurationDelegate == null)
            {
                this._configurationDelegate = new QueryStrategyConfiguration();
            }
        }
        #endregion

        #region ToString helper

        /// <summary>
        /// Toes the string helper.
        /// </summary>
        /// <param name="data">The ISerializableObject object.</param>
        /// <param name="descriptionLength">Length of the description.</param>
        /// <returns></returns>
        protected string ToStringHelper(SerializableObject data, int descriptionLength)
        {
            System.Text.StringBuilder ret = new System.Text.StringBuilder();

            for (int i = 0; i < descriptionLength; i++) ret.Append("-");
            ret.Append("\n");
            ret.Append(AttributeUtilities.GetTableMapping(data)).Append(" Row\n");
            for (int i = 0; i < descriptionLength; i++) ret.Append("-");
            ret.Append("\n");

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute attribute = AttributeUtilities.GetColumnAttribute(property);

                if (attribute != null)
                {
                    string columnName = AttributeUtilities.GetColumnMapping(property);
                    ret.Append(columnName);
                    for (int i = columnName.Length; i < descriptionLength; i++)
                    {
                        ret.Append(".");
                    }
                    ret.Append(": ");
                    MethodInfo m = property.GetGetMethod();
                    Object obj = m.Invoke(data, null);
                    ret.Append(obj);
                    ret.Append("\n");
                }
            }
            return ret.ToString();
        }

        #endregion
    }
}
