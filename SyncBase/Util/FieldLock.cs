//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.SyncBase.Util
{
    public class FieldLock
    {
        private Type _lockedType;
        private Dictionary<FieldInfo, bool> _lockedFields;

        public FieldLock(Type lockedType)
        {
            
            _lockedFields = new Dictionary<FieldInfo, bool>();
            _lockedType = lockedType;
        }

        public void LockField(String fieldName)
        {
            FieldInfo fi = _lockedType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            _lockedFields[fi] = true;
        }

        public void LockField(FieldInfo fi)
        {
            fi = _lockedType.GetField(fi.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            _lockedFields[fi] = true;

        }

        public void UnLockField(String fieldName)
        {
            FieldInfo fi = _lockedType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            _lockedFields[fi] = false;
        }

        public void UnLockField(FieldInfo fi)
        {
            fi = _lockedType.GetField(fi.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            _lockedFields[fi] = false;
        }

        public bool IsLocked(String fieldName)
        {
            try
            {
                FieldInfo fi = _lockedType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                return _lockedFields[fi];
            }
            catch (KeyNotFoundException ex)
            {
                return false;
            }
        }

        public bool IsLocked(FieldInfo fi)
        {
            try
            {
                return _lockedFields[fi];
            }
            catch (KeyNotFoundException ex)
            {
                return false;
            }
        }
    }
}
