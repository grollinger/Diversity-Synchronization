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
using System.ComponentModel;

namespace MVVMDiversity.Model
{
    public class BackgroundOperation 
    {
        public enum State
        {
            Running,
            Succeeded,
            Failed
        }

        public static BackgroundOperation newUninterruptable()
        {
            return new BackgroundOperation(false);
        }

        public static BackgroundOperation newInterruptable()
        {
            return new BackgroundOperation(true);
        }
        
        private BackgroundOperation(bool canBeCanceled)
        {
            CanBeCanceled = canBeCanceled;
        }


        /// <summary>
        /// The <see cref="State" /> property's name.
        /// </summary>
        public const string StatePropertyName = "OperationState";

        private State _state = BackgroundOperation.State.Running;

        /// <summary>
        /// Gets the State property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public State OperationState
        {
            get
            {
                return _state;
            }

            set
            {
                if (_state == value)
                {
                    return;
                }
                
                _state = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StatePropertyName);
              
            }
        }


        /// <summary>
        /// The <see cref="Progress" /> property's name.
        /// </summary>
        public const string ProgressPropertyName = "Progress";

        private int _progress = 0;

        /// <summary>
        /// Gets the Progress property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                if (_progress == value)
                {
                    return;
                }

                var oldValue = _progress;
                _progress = value;                               

                // Update bindings, no broadcast
                RaisePropertyChanged(ProgressPropertyName);
            }
        }        
     

        /// <summary>
        /// The <see cref="ProgressDescriptionID" /> property's name.
        /// </summary>
        public const string ProgressDescriptionIDPropertyName = "ProgressDescriptionID";

        private string _descID = "";

        /// <summary>
        /// Gets the ProgressDescriptionID property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string ProgressDescriptionID
        {
            get
            {
                return _descID;
            }

            set
            {
                if (_descID == value)
                {
                    return;
                }

                var oldValue = _descID;
                _descID = value;             

               
                // Update bindings, no broadcast
                RaisePropertyChanged(ProgressDescriptionIDPropertyName);                
            }
        }
   

        /// <summary>
        /// The <see cref="ProgressOutput" /> property's name.
        /// </summary>
        public const string ProgressOutputPropertyName = "ProgressOutput";

        private string _progOut = "";

        /// <summary>
        /// Gets the ProgressOutput property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string ProgressOutput
        {
            get
            {
                return _progOut;
            }

            set
            {
                if (_progOut == value)
                {
                    return;
                }

                var oldValue = _progOut;
                _progOut = value;

               

                // Update bindings, no broadcast
                RaisePropertyChanged(ProgressOutputPropertyName);

            }
        }      

        /// <summary>
        /// The <see cref="IsProgressIndeterminate" /> property's name.
        /// </summary>
        public const string IsProgressIndeterminatePropertyName = "IsProgressIndeterminate";

        private bool _indet = true;

        /// <summary>
        /// Gets the IsProgressIndeterminate property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsProgressIndeterminate
        {
            get
            {
                return _indet;
            }

            set
            {
                if (_indet == value)
                {
                    return;
                }

                var oldValue = _indet;
                _indet = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(IsProgressIndeterminatePropertyName);
          
            }
        }

        public bool CanBeCanceled { get; private set; }

        private bool _canceled;
        public bool IsCancelRequested 
        {
            get
            {
                return _canceled;
            }
            set
            {
                _canceled = value && CanBeCanceled;
            }
        }


        #region INPC Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
