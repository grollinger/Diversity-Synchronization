//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

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