using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    internal class SerializationMetaDataManager
    {
        private List<WeakReference> _managedMetaDataKeys;
        private Dictionary<WeakReference, SerializationMetaData> _managedMetaData;

        internal SerializationMetaDataManager()
        {
            _managedMetaDataKeys = new List<WeakReference>();
            _managedMetaData = new Dictionary<WeakReference, SerializationMetaData>();
        }

        internal SerializationMetaData GetSerializationMetaData(ISerializableObject obj)
        {
            //hole den Schlüssel für die Metadaten
            WeakReference key = GetManagedMetaDataKey(obj);
            SerializationMetaData res;

            //Wenn es bereits Metadaten gibt, dann gebe diese zurück
            if (_managedMetaData.TryGetValue(key, out res))
            {
                return res;
            }
            //Ansonsten neue Metadaten anlegen und zurückgeben
            else
            {
                res = new SerializationMetaData();
                _managedMetaData.Add(key, res);
                return res;
            }
        }

        private WeakReference GetManagedMetaDataKey(ISerializableObject obj) 
        {
            WeakReference tmp = null;
            List<WeakReference> obsoleteKeys = new List<WeakReference>();
            foreach (WeakReference r in _managedMetaDataKeys)
            {
                if (r.Target == obj)
                {
                    tmp = r;
                }
                //speichere obsolete keys
                if (!r.IsAlive)
                {
                    obsoleteKeys.Add(r); 
                }
            }

            //entferne alle obsoleten keys
            foreach (WeakReference r in obsoleteKeys)
            {
                _managedMetaDataKeys.Remove(r);
                _managedMetaData.Remove(r);
            }

            //Wenn der Metadaten schlüssel nicht vorhanden ist, dann erzeuge einen neuen
            if (tmp == null)
            {
                tmp = new WeakReference(obj);
                _managedMetaDataKeys.Add(tmp);
            }
            return tmp;
        }
    }
}
