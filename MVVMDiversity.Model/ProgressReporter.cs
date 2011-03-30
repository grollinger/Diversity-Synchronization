using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MVVMDiversity.Model
{
    public class ProgressReporter
    {       
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

        public bool IsCancelRequested { get; set; }


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
