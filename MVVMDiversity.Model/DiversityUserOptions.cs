using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace MVVMDiversity.Model
{
    public class DiversityUserOptions : IDataErrorInfo, INotifyPropertyChanged
    {
        #region Properties

        DBPaths _paths = new DBPaths();

        public bool PasswordVisible { get; set; }

        /// <summary>
        /// The <see cref="UseSqlAuthentification" /> property's name.
        /// </summary>
        public const string UseSqlAuthentificationPropertyName = "UseSqlAuthentification";

        private bool _useSql = true;

        /// <summary>
        /// Gets the UseSqlAuthentification property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool UseSqlAuthentification
        {
            get
            {
                return _useSql;
            }

            set
            {
                if (_useSql == value)
                {
                    return;
                }

                var oldValue = _useSql;
                _useSql = value;
               

                // Update bindings, no broadcast
                RaisePropertyChanged(UseSqlAuthentificationPropertyName);               
            }
        }

        public string Username { get; set; }

        public ConnectionProfile CurrentConnection { get; set; }


        private static readonly string PathsName = "Paths";
        public DBPaths Paths
        { 
            get { return _paths; }
            set
            {
                if (_paths != value)
                {
                    _paths = value;                    
                    RaisePropertyChanged(PathsName);
                }
            }
        }          

        public bool UseDeviceDimensions { get; set; }

        #endregion
       
        #region IDataErrorInfo
        private string _err;

        public string Error
        {
            get 
            {
                _err = null;

                checkUsername();
                if(UseSqlAuthentification)
                    checkPaths();

                return _err;
            }
        }

        private void checkPaths()
        {
            if (Paths == null)
                _err = "Validation_Options_FieldEmpty";
            else
            {
                nonEmptyAndExists(Paths.MobileDB);
                nonEmptyAndExists(Paths.MobileTaxa);
            }
        }

        public string this[string columnName]
        { 
            get 
            {
                _err = null;

                if (columnName == "Username")
                    checkUsername();
                else if (columnName == PathsName)
                    checkPaths();
                
                

                return _err;
            }
        }

        private void checkUsername()
        {
            if(UseSqlAuthentification && string.IsNullOrEmpty(Username))
                _err = "Validation_Options_UsernameEmpty";
        }
        private void nonEmptyAndExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                _err = "Validation_Options_FieldEmpty";
            else if (!File.Exists(path))
                _err = "Validation_Options_FileNotFound";
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
