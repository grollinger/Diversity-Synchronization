using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace MVVMDiversity.Services
{
    class XMLObjectStore<T>
    {
        private string _file;
        private T _content;

        private XmlSerializer _serializer = new XmlSerializer(typeof(T));

        public XMLObjectStore(string targetFilePath)
        {
            _file = targetFilePath;
        }


        public T Load()
        {
            if(_content == null)
            {
                if (File.Exists(_file))
                {
                    using (var fs = new FileStream(_file, FileMode.Open))
                    {
                        var xmlDoc = XmlReader.Create(fs);
                        if (_serializer.CanDeserialize(xmlDoc))
                        {
                            _content = (T)_serializer.Deserialize(xmlDoc);
                        }
                    }            
                }
            }
            return _content;
        }

        public void Store(T value)
        {
            _content = value;
            using (var xmlFile = new FileStream(_file, FileMode.OpenOrCreate))
            {
                _serializer.Serialize(xmlFile, value);
            }

        }

       
    }
}
