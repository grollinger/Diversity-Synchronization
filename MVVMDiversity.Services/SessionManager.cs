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
using System.IO;
using log4net.Appender;
using log4net;
using MVVMDiversity.Model;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using System.Xml.Serialization;

namespace MVVMDiversity.Services
{
    public class SessionManager : ISessionManager
    {
        IMessenger _msngr = null;
        [Dependency]
        public IMessenger MessengerInstance 
        {
            get
            {
                return _msngr;
            }
            set
            {
                if (_msngr == value)
                    return;
                _msngr = value;
                if (_msngr != null)
                    MessengerInstance.Register<SyncStepFinished>(this, (msg) =>
                    {
                        Sync |= msg.Content;
                    });
            }
        }

        private const int MAX_SESSIONS_SAVED = 100;

        static string _currentSessionFolder;

        static FileAppender _sessionAppender;

        static FileAppender SessionAppender
        {
            get
            {
                if (_sessionAppender == null)
                    _sessionAppender = GlobalLog.getFileAppender("TransactionLogFileAppender");
                return _sessionAppender;
            }
        }

        static ILog _Log = GlobalLog.Global;

        public string SessionFolder
        {
            get
            {               
                return _currentSessionFolder;
            }
        }

        private DBPaths _paths;
        private DBPaths _workingPaths;        


        private SessionState _state = SessionState.Uninitialized;             
        public SessionState State 
        {
            get
            {
                return _state;
            }
            private set
            {
                if (_state == value)
                    return;

                _state = value;
                if (_state == SessionState.New || _state == SessionState.Cleaned)
                    Sync = SyncState.None;
            }
        }

        private SyncState _sync = SyncState.None;
        public SyncState Sync 
        {
            get
            {
                return _sync;
            }
            private set
            {
                if(_sync == value)
                    return;

                _sync = value;

                MessengerInstance.Send<SyncStateChanged>(_sync);

                if(_sync != SyncState.None)
                    State = SessionState.Dirty;

                saveSessionInfo();
            }
        }

        public SessionManager()
        {
            State = SessionState.Uninitialized;
            
        }
                
        

        public void startSession()
        {
            if (State == SessionState.Uninitialized)
            {
                MessengerInstance.Send<StatusNotification>("Services_Session_NewSession");
                //Example: 2010-10-08 2102
                var currentSession = DateTime.Now.ToString("yyyy-MM-dd HHmm");
                _currentSessionFolder = ApplicationPathManager.getFolderPath(ApplicationFolder.Sessions) + @"\" + currentSession;
                Directory.CreateDirectory(_currentSessionFolder);
                updateLogger();
                _Log.InfoFormat("Session started! [{0}]", currentSession);

                cleanUpOldSessions();

                State = SessionState.New;                
            }
            else
                _Log.Error("Cannot open new Session, while old one is still open");
        }

        private static void updateLogger()
        {
            if (SessionAppender != null)
            {
                SessionAppender.File = _currentSessionFolder + @"\log.txt";
                SessionAppender.ActivateOptions();
            }
        }

        public DBPaths createWorkingCopies(DBPaths paths)
        {
            if (initSessionFromPaths(paths))
            {
                if (tryCopyToWorking(_paths.MobileDB, _paths.MobileTaxa))
                {
                    MessengerInstance.Send<StatusNotification>("Services_Session_WorkingCopiesCreated");
                    return _workingPaths;
                }
            }
            
            return null;
           
        }

        public DBPaths createCleanWorkingCopies(DBPaths paths)
        {
            if (initSessionFromPaths(paths))
            {
                var emptyDB = ApplicationPathManager.getFilePath(ApplicationFile.EmptyDB);
                var emptyTaxa = ApplicationPathManager.getFilePath(ApplicationFile.EmptyTaxonDB);

                MessengerInstance.Send<StatusNotification>("Services_Session_CreatingCleanCopies");

                if (tryCopyToWorking(emptyDB, emptyTaxa))
                {
                    State = SessionState.Cleaned;
                    return _workingPaths;
                }
            }
            return null;
        }   

        private bool initSessionFromPaths(DBPaths paths)
        {
            if (paths != null)
            {
                _paths = paths;

                copyPictures();                

                return true;
            }
            else
                _Log.Error("Can't create Working Copies: No Settings available");

            return false;
        }

        private bool tryCopyToWorking(string mobileDB, string mobileTaxa)
        {
            bool success = true;

            var workingMobileDB = _currentSessionFolder + "\\" + ApplicationPathManager.MOBILEDB_FILE;
            success &= copyFile(mobileDB, workingMobileDB);



            var workingMobileTaxa = _currentSessionFolder + "\\" + ApplicationPathManager.TAXONDB_FILE;
            success &= copyFile(mobileTaxa, workingMobileTaxa);


            if (success)
            {
                _workingPaths = new DBPaths()
                {
                    MobileDB = workingMobileDB,
                    MobileTaxa = workingMobileTaxa
                };
            }

            return success;
        }

        public bool endSession()
        {
            bool success = true;//TODO Cleanup
            if (State == SessionState.Dirty || State == SessionState.Cleaned)
            {
                _Log.Info("Saving Session!");

                success &= copyFile(_workingPaths.MobileDB,_paths.MobileDB);
                success &= copyFile(_workingPaths.MobileTaxa,_paths.MobileTaxa);

                copyMaps();
            }
            else
                _Log.Info("Nothing To Save");
            if (success)
            {
                State = SessionState.Uninitialized;
                Sync = SyncState.None;
            }
            return success;
        }

        #region DB Operations


        public bool canResumeSession()
        {
            var sessionInfo = getSessionInfo(lastSession());
            return sessionInfo != null && sessionInfo.SState == SessionState.Dirty;
        }

        public DBPaths resumeSession(DBPaths paths)
        {
            var sessionToResume = lastSession();
            var sessionInfo = getSessionInfo(sessionToResume);
            
            var mobilePath = sessionToResume + "\\" + ApplicationPathManager.MOBILEDB_FILE;
            var taxonPath = sessionToResume + "\\" + ApplicationPathManager.TAXONDB_FILE;

            if (File.Exists(mobilePath) && File.Exists(taxonPath))
            {
                _currentSessionFolder = sessionToResume;
                updateLogger();

                _paths = paths;

                _workingPaths = new DBPaths()
                {
                    MobileDB = mobilePath,
                    MobileTaxa = taxonPath
                };
                if (sessionInfo != null)
                {
                    State = sessionInfo.SState;
                    Sync = sessionInfo.SSync;
                }
                else
                {
                    State = SessionState.New;
                    Sync = SyncState.None;
                }

                return _workingPaths;
            }
            else
            {
                _Log.Error("Can't resume. Files missing.");
                startSession();
                return createWorkingCopies(paths);
            }
            
        }
        


        #endregion

        private void copyMaps()
        {
            if (!File.Exists(_paths.MobileDB))
                return;

            string mobileMapsPath = Path.GetDirectoryName(_paths.MobileDB) + "\\maps";
            string localMapsPath = ApplicationPathManager.getFolderPath(ApplicationFolder.Maps);
            if (!Directory.Exists(mobileMapsPath))
                Directory.CreateDirectory(mobileMapsPath);
            if (Directory.Exists(localMapsPath) && Directory.Exists(mobileMapsPath))
            {
                foreach (var file in Directory.GetFiles(localMapsPath, "*.png"))
                {
                    var xmlFile = file.Replace(".png", ".xml");
                    var pngLPath = String.Format("{0}\\{1}", localMapsPath, file);
                    var pngMPath = String.Format("{0}\\{1}", mobileMapsPath, file);
                    var xmlLPath = String.Format("{0}\\{1}", localMapsPath, xmlFile);
                    var xmlMPath = String.Format("{0}\\{1}", mobileMapsPath, xmlFile);
                    if (File.Exists(xmlLPath))
                    {
                        copyFile(pngLPath, pngMPath);
                        copyFile(xmlLPath, xmlMPath);
                    }
                }

            }
        }

        private void copyPictures()
        {
            if (!File.Exists(_paths.MobileDB))
                return;

            string mobilePictureDirectory = Path.GetDirectoryName(_paths.MobileDB) + "\\pictures";
            if (Directory.Exists(mobilePictureDirectory))
            {
                var pictureDir = ApplicationPathManager.getFolderPath(ApplicationFolder.Pictures);
                foreach (var picture in Directory.GetFiles(mobilePictureDirectory))
                {
                    copyFile(picture,pictureDir + "\\" + Path.GetFileName(picture));
                }
            }
        }

        

        private bool copyFile(string from, string to)
        {
            bool res = true;
            if (File.Exists(from))
            {
                try
                {
                    File.Copy(from, to, true);
                }
                catch (Exception ex)
                {
                    _Log.ErrorFormat("Couldn't copy File: [{0}] [{1}]", from, ex);
                    res = false;
                }
            }
            else
            {
                _Log.ErrorFormat("Couldn't copy File: File not Found [{0}]", from);
                res = false;
            }
            return res;
        }

        

        private static void cleanUpOldSessions()
        {
            var sessions = sessionListAsc();
            int sessionOverhang = sessions.Count() - MAX_SESSIONS_SAVED;
            if (sessionOverhang > 0)
            {
                var superfluousSessions = sessions.Take(sessionOverhang);
                foreach (var superfluousSession in superfluousSessions)
                {
                    try
                    {
                        Directory.Delete(superfluousSession, true);
                    }
                    catch (Exception ex)
                    {
                        _Log.InfoFormat("Could not remove superfluous Session [{0}] : [{1}]", superfluousSession, ex);
                    }
                }
            }
        }

        private static string lastSession()
        {
            var sessions = sessionListAsc();
            if (sessions.Count() > 0)
                return sessions.Last();
            else
                return null;

        }

        private static IOrderedEnumerable<string> sessionListAsc()
        {
            var sessions = Directory.GetDirectories(ApplicationPathManager.getFolderPath(ApplicationFolder.Sessions));
            return (from dir in sessions
                    orderby dir ascending
                    select dir);
        }

        

        private SessionInfo getSessionInfo(string session)
        {
            string fullSessionStatePath = string.Format("{0}\\{1}",session, ApplicationPathManager.SESSIONSTATE_FILE);

            if (File.Exists(fullSessionStatePath))
            {
                try
                {
                    return (new XMLObjectStore<SessionInfo>(fullSessionStatePath)).Load();
                }
                catch (Exception ex)
                {
                    _Log.InfoFormat("Could not get State of session [{0}]:[{1}]", session, ex);
                }
            }
            return null;
        }

        private void saveSessionInfo()
        {
            string fullSessionStatePath = string.Format("{0}\\{1}", _currentSessionFolder, ApplicationPathManager.SESSIONSTATE_FILE);
            try
            {
                new XMLObjectStore<SessionInfo>(fullSessionStatePath).Store(new SessionInfo() { SState = State, SSync = Sync });
            }
            catch (Exception ex)
            {
                _Log.ErrorFormat("Error saving State: [{0}]", ex);
            }
        }

        public class SessionInfo
        {
            public SessionState SState { get; set; }
            public SyncState SSync { get; set; }
        }
       
    }
}
