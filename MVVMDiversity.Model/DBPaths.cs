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
    public class DBPaths : INotifyPropertyChanged
    {
        /// <summary>
        /// The <see cref="MobileDB" /> property's name.
        /// </summary>
        public const string MobileDBPropertyName = "MobileDB";

        private string _mDB = "";

        /// <summary>
        /// Gets the MobileDB property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MobileDB
        {
            get
            {
                return _mDB;
            }

            set
            {
                if (_mDB == value)
                {
                    return;
                }

                var oldValue = _mDB;
                _mDB = value;                


                // Update bindings, no broadcast
                RaisePropertyChanged(MobileDBPropertyName);
               
            }
        }

        /// <summary>
        /// The <see cref="MobileTaxa" /> property's name.
        /// </summary>
        public const string MobileTaxaPropertyName = "MobileTaxa";

        private string _mTax = "";

        /// <summary>
        /// Gets the MobileTaxa property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MobileTaxa
        {
            get
            {
                return _mTax;
            }

            set
            {
                if (_mTax == value)
                {
                    return;
                }

                var oldValue = _mTax;
                _mTax = value;
              

                // Update bindings, no broadcast
                RaisePropertyChanged(MobileTaxaPropertyName);
            }
        }

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
