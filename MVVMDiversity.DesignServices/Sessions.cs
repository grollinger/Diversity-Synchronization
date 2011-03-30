using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class Sessions : ISessionManager
    {
        SyncState _state = SyncState.None;
        public Sessions()
        {
            Messenger.Default.Register<SyncStepFinished>(this, (msg) => { Messenger.Default.Send<SyncStateChanged>(_state |= msg.Content); });
        }
        public void startSession()
        {
            
        }

        public Model.DBPaths createWorkingCopies(Model.DBPaths paths)
        {
            return paths;
        }

        public Model.DBPaths createCleanWorkingCopies(Model.DBPaths paths)
        {
            return paths;
        }

        public void endSession()
        {
            
        }

        public bool canResumeSession()
        {
            return true;
        }

        public void resumeSession()
        {
            
        }
    }
}
