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
using MVVMDiversity.Interface;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using Microsoft.Practices.Unity;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using log4net;
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;

namespace MVVMDiversity.Services
{
    public class UserProfileProvider : IUserProfileService
    {
        private IConnectionProvider _connections = null;
        private IUserOptionsService _userOptions = null;
        private DiversityUserOptions _settings = null;
        private Serializer _mobileSerializer = null;
        private Serializer _repositorySerializer = null;
        private Serializer _definitionsSerializer = null;
        private UserProfile _profile;


        private ILog _Log = LogManager.GetLogger(typeof(UserProfileProvider));        

        [Dependency]
        public IUserOptionsService Settings 
        {
            get
            {
                return _userOptions;
            }
            set
            {
                _userOptions = value;
                updateProfile();
            }
        }

        [Dependency]
        public IConnectionProvider Connections
        {
            get
            {
                return _connections;
            }
            set
            {
                _connections = value;               
                updateProfile();
            }
        }

        [Dependency]
        public IMessenger MessengerInstance
        {
            set
            {
                if (value != null)
                    value.Register<ConnectionStateChanged>(this,
                        (msg) =>
                        {
                            if ((msg.Content & ConnectionState.FullyConnected) == ConnectionState.FullyConnected)
                            {
                                new Action(() =>
                                    {
                                        lock (this)
                                        {
                                            updateProfile();
                                        }
                                    }).BeginInvoke(null, null);

                            }
                        });
            }
        }

        

        

        private void updateProfile()
        {
             _profile = null;
             if (Connections != null)
             {
                 
                 if (Settings != null && (_settings = Settings.getOptions()) != null)
                 {
                     if ((_mobileSerializer = Connections.MobileDB) != null)
                     {
                         //Prüfen ob ein UserProfile zum LoginNamen existiert.                
                         IList<UserProfile> profiles = new List<UserProfile>();

                         IRestriction r = RestrictionFactory.Eq(typeof(UserProfile), "_LoginName", _settings.Username);
                         //IRestriction r = RestrictionFactory.Eq(typeof(UserProfile), "_LoginName", "TestEditor");
                         profiles = _mobileSerializer.Connector.LoadList<UserProfile>(r);
                         
                         if (profiles.Count > 0)
                         {
                             _profile = profiles[0];
                         }
                         else
                         {
                             _profile = createProfile();
                         }

                     }
                     else
                         _Log.Error("Mobile DB not connected");
                 }
                 else
                     _Log.Error("Settings N/A");
             }
             else
                 _Log.Error("Connections N/A");
                
        }        

        public int ProjectID
        {
            get
            {
                return (_profile != null) ? _profile.ProjectID ?? -1 : -1;
            }
            set
            {
                if (_profile != null)
                {
                    _profile.ProjectID = value;
                    if (_mobileSerializer != null)
                    {
                        _mobileSerializer.Connector.Save(_profile);
                    }
                    else
                        _Log.Error("Can't Save. No Connection.");
                }
                else
                    _Log.Error("No Profile");
            }

        }

        public string HomeDB
        {
            get { return (_profile != null) ? _profile.HomeDB : null; }
        }

        public string UserNr
        {
            get
            {
                return (_profile == null || _profile.AgentURI == null) ? "" : _profile.AgentURI.Replace(@"http://id.snsb.info/Agents/", "");
            }
        }



        private UserProfile createProfile()
        {
            UserProfile newProfile = null;
            if ((_repositorySerializer = Connections.Repository) != null)
            {
                if ((_definitionsSerializer = Connections.Definitions) != null)
                {

                    
                    UserProxy proxy;
                    
                    try
                    {
                        newProfile = _mobileSerializer.CreateISerializableObject<UserProfile>();
                        
                        //Zuerst korrespondierenden Userproxy holen                
                        IRestriction r = RestrictionFactory.Eq(typeof(UserProxy), "_LoginName", _settings.Username);
                        //IRestriction r = RestrictionFactory.Eq(typeof(UserProxy), "_LoginName", @"TestEditor");
                        proxy = _repositorySerializer.Connector.Load<UserProxy>(r);
                        
                        newProfile.LoginName = _settings.Username;
                        //newProfile.LoginName = @"TestEditor";
                        string agentName = null;
                        
                        using (var conn = _definitionsSerializer.CreateConnection())
                        {
                            try
                            {
                                conn.Open();
                                
                                var cmd = conn.CreateCommand();
                                cmd.CommandText = "SELECT [AgentName] FROM [" + _settings.CurrentConnection.TaxonNamesInitialCatalog + "].[dbo].[IBFagents] WHERE [AgentURI] = '" + proxy.AgentURI + "'";
                                _Log.DebugFormat("Select AgentName Command: [{0}]", cmd.CommandText);
                                
                                agentName = cmd.ExecuteScalar() as string;
                                _Log.DebugFormat("AgentName: [{0}]", agentName);
                                
                            }
                            finally
                            {
                                conn.Close();                                
                            }
                        }

                        newProfile.CombinedNameCache = (!string.IsNullOrEmpty(agentName)) ? agentName : proxy.CombinedNameCache;
                        
                        newProfile.HomeDB = _settings.CurrentConnection.InitialCatalog;
                        
                        newProfile.AgentURI = proxy.AgentURI;
                       
                    }
                    catch (Exception ex)
                    {
                        newProfile = null;

                        _Log.ErrorFormat("Error Creating Profile: {0}", ex);
                    }
                    
                }
                else
                    _Log.Error("Definitions N/A");
            }
            else
                _Log.Error("Repository N/A");

            return newProfile;
        }
    }
}
