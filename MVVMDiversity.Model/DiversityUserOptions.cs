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
        /// 
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

        private const string UsernamePropertyName = "Username";


        public string Username { get; set; }

        /// <summary>
        /// The <see cref="CurrentConnection" /> property's name.
        /// </summary>
        public const string CurrentConnectionPropertyName = "CurrentConnection";

        private ConnectionProfile _connection = null;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionProfile CurrentConnection
        {
            get
            {
                return _connection;
            }

            set
            {
                if (_connection == value)
                {
                    return;
                }

                
                _connection = value;  
                
                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentConnectionPropertyName);
            }
        }


        private const string PathsName = "Paths";
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

        /// <summary>
        /// The <see cref="UseDeviceDimensions" /> property's name.
        /// </summary>
        private const string UseDeviceDimensionsPropertyName = "UseDeviceDimensions";

        private bool _useDevDim = false;

        /// <summary>
        /// 
        /// </summary>
        public bool UseDeviceDimensions
        {
            get
            {
                return _useDevDim;
            }

            set
            {
                if (_useDevDim == value)
                {
                    return;
                }

                var oldValue = _useDevDim;
                _useDevDim = value;                  

                // Update bindings, no broadcast
                RaisePropertyChanged(UseDeviceDimensionsPropertyName);               
            }
        }

        private const string ScreenHeightPropertyName = "ScreenHeight";

        public int ScreenHeight { get; set; }

        private const string ScreenWidthPropertyName = "ScreenWidth";

        public int ScreenWidth { get; set; }

        public bool TruncateDataItems { get; set; }

        #endregion
       
        #region IDataErrorInfo
        private string _err;

        public string Error
        {
            get 
            {
                //_err = this[UsernamePropertyName];
                _err = null;

                _err = _err ?? this[PathsName];
                _err = _err ?? this[ScreenHeightPropertyName];
                _err = _err ?? this[ScreenWidthPropertyName];
                

                return _err;
            }
        }

     

        public string this[string columnName]
        { 
            get 
            {
                _err = null;

                if (columnName == UsernamePropertyName)
                    checkUsername();
                else if (columnName == PathsName)
                    checkPaths();
                else if (columnName == ScreenHeightPropertyName)
                    checkScreenDimension(ScreenHeight);
                else if (columnName == ScreenWidthPropertyName)
                    checkScreenDimension(ScreenWidth);
                
                

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

        private void checkScreenDimension(int dim)
        {
            if (UseDeviceDimensions)
                if (dim > 800 || dim < 100)
                    _err = "Validation_Options_FieldDimensionOutOfRange";

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
