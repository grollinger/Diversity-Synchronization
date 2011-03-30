using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MVVMDiversity.Services
{
    public class ConnectionProfileProvider : IConnectionProfilesService
    {
        private string _profileStore;
        private IList<ConnectionProfile> _profiles;

        private XmlSerializer _serializer = new XmlSerializer(typeof(List<ConnectionProfile>));


        public IList<Model.ConnectionProfile> getAvailableProfiles()
        {
            var profilesFile = ApplicationPathManager.getFilePath(ApplicationFile.ConnectionProfiles);
            if (_profileStore != profilesFile)
            {
                _profileStore = profilesFile;
                fetchProfiles();
            }
            return _profiles;
        }

        private void fetchProfiles()
        {
            if (File.Exists(_profileStore))
            {
                var xmlDoc = XmlReader.Create(new FileStream(_profileStore, FileMode.Open));

                if (_serializer.CanDeserialize(xmlDoc))
                {
                    _profiles = _serializer.Deserialize(xmlDoc) as IList<ConnectionProfile>;
                }
            }
        }
    }
}