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
    public delegate void AsyncOperationFinishedHandler(AsyncOperationInstance operation);
    public delegate void AsyncOperationFinishedHandler<T>(AsyncOperationInstance<T> operation, T result);

    public class AsyncOperationInstance
    {
        private AsyncOperationFinishedHandler _finished;       
        
        public AsyncOperationInstance(bool canBeCanceled, AsyncOperationFinishedHandler finishedCallback)
        {
            _finished = finishedCallback;
            CanBeCanceled = canBeCanceled;
        }

        public virtual void success()
        {            
            if (!operationFinished())
            {
                State = OperationState.Succeeded;
            }      
        }

        public virtual void failure(string reason,string output)
        {
            if (!operationFinished())
            {
                StatusDescription = reason;
                StatusOutput = output;
                State = OperationState.Failed;
            }    
        }


        #region Properties

        /// <summary>
        /// The <see cref="State" /> property's name.
        /// </summary>
        public const string StatePropertyName = "OperationState";

        private OperationState _state = OperationState.Running;

        /// <summary>
        /// Represents the current Status of this Operation
        /// </summary>
        public OperationState State
        {
            get
            {
                return _state;
            }

            private set
            {
                if (_state == value || operationFinished())
                {
                    return;
                }
                
                _state = value;

                if (_finished != null)
                    _finished(this);

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
       /// The Progress of this Operation in Percent
       /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                if (_progress == value || operationFinished())
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
        /// The <see cref="Status" /> property's name.
        /// </summary>
        public const string StatusPropertyName = "StatusDescription";

        private string _status = "";

        /// <summary>
        /// A Localization index string used to display a 
        /// description of the current operation to the user
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }
               
                _status = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(StatusPropertyName);
            }
        }

      

        /// <summary>
        /// The <see cref="StatusOutput" /> property's name.
        /// </summary>
        public const string StatusOutputPropertyName = "StatusOutput";

        private string _statusOut = "";

        /// <summary>
        /// An additional String that is displayed as is without localization
        /// Should be used, to provide additional Progress information to the user.
        /// </summary>
        public string StatusOutput
        {
            get
            {
                return _statusOut;
            }

            set
            {
                if (_statusOut == value)
                {
                    return;
                }
               
                _statusOut = value;
               
                // Update bindings, no broadcast
                RaisePropertyChanged(StatusOutputPropertyName);             
            }
        }

        /// <summary>
        /// The <see cref="IsProgressIndeterminate" /> property's name.
        /// </summary>
        public const string IsProgressIndeterminatePropertyName = "IsProgressIndeterminate";

        private bool _indet = true;

        /// <summary>
        /// Indicates, whether the Progress can be accurately determined (Percentage) 
        /// or an indeterminate Progress bar should be displayed
        /// </summary>
        public bool IsProgressIndeterminate
        {
            get
            {
                return _indet;
            }

            set
            {
                if (_indet == value || operationFinished())
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
        #endregion

        protected bool operationFinished()
        {
            return State != OperationState.Running;
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

    public class AsyncOperationInstance<T> : AsyncOperationInstance
    {
        private AsyncOperationFinishedHandler<T> _finished;

        public AsyncOperationInstance(bool canBeCanceled, AsyncOperationFinishedHandler<T> finishedCallback)
            : base(canBeCanceled,null)
        {
            _finished = finishedCallback;
        }

        public void success(T result)
        {
            if (_finished != null && !operationFinished())
            {
                base.success();
                _finished(this, result);
            }
        }

        public override void success()
        {            
            success(default(T));
        }

        public void failure(string reason, string output, T result)
        {
            if (_finished != null && !operationFinished())
            {
                base.failure(reason, output);
                _finished(this, result);
            }
        }
        public override void failure(string reason, string output)
        {
            failure(reason, output,default(T));
        }
    } 
}
