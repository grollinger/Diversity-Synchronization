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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    public class Conflict
    {
        private Type ownerType;
        private FieldInfo field;
        private Object value1;
        private Object value2;

        internal Conflict(Type t, FieldInfo f, Object v1, Object v2)
        {
            this.ownerType = t;
            this.field = f;
            this.value1 = v1;
            this.value2 = v2;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(ownerType.Name);
            sb.Append(",").Append(field.Name).Append(",");
            if (value1 != null)
            {
                sb.Append((Object)value1.ToString());
            }
            else
            {
                sb.Append("NULL");
            }
            sb.Append(",");
            if (value2 != null)
            {
                sb.Append(value2.ToString());
            }
            else
            {
                sb.Append("NULL");
            }
            return sb.ToString();
        }
    }
}
