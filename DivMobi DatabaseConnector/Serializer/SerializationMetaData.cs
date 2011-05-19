using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public class SerializationMetaData
    {
        public const int NOT_READ = -1;

        private bool _persistent;
        private bool _navigated;
        private int _readerPosition;
        private List<string> _navigatedAttributes;
        private string _navigationQuery;


        public SerializationMetaData()
        {
            _persistent = false;
            _navigated = false;
            _navigationQuery = null;
            _navigatedAttributes = new List<string>();
            _readerPosition = NOT_READ;
        }

        public bool Persistent { get { return this._persistent; } set { this._persistent = value; } }
        public bool Navigated { get { return this._navigated; } set { this._navigated = value; } }
        public int ReaderPosition{ get { return this._readerPosition; } set { this._readerPosition = value; }}
        public string NavigationQuery { get { return this._navigationQuery; } set { this._navigationQuery = value; } }
        public void AddNavigatedAttribute(string attribute)
        {
            _navigatedAttributes.Add(attribute);
        }
        public void RemoveNavigatedAttribute(string attribute) {
            _navigatedAttributes.Remove(attribute);
        }
        public void ClearNavigatedAttributes()
        {
            _navigatedAttributes.Clear();
        }

        public List<string> NavigatedAttributes { get { return _navigatedAttributes; } }


        public void SetSerializationMetaData(SerializationMetaData metaData)
        {
            this._persistent = metaData._persistent;
            this._navigated = metaData._navigated;
            this._readerPosition = metaData._readerPosition;
            this._navigationQuery = metaData._navigationQuery;
            this._navigatedAttributes = metaData._navigatedAttributes;
        }

    }
}
