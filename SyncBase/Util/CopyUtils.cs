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
using System.Text;
using System.Reflection;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DataLayer.SyncAttributes;


namespace UBT.AI4.Bio.DivMobi.SyncBase.Util
{
    public class CopyUtils
    {
        internal static void PlainCopy(Serializer context, ISerializableObject from, ISerializableObject to, FieldLock fieldLock, bool forceForeignKeyCopy)
        {
            AttributeWorker w = AttributeWorker.GetInstance(context.Target);

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(from.GetType());

            foreach (FieldInfo fi in fis)
            {
                if (fieldLock.IsLocked(fi)) continue;
                if (AttributeWorker.IsRelationField(fi)) continue;
                if (w.IsAutoincID(fi)) continue;
                if (!w.IsPersistentField(fi)) continue;
                if (AttributeWorker.IsRowGuid(fi)) continue;
                if (w.IsForeignKey(context, fi) && !forceForeignKeyCopy)
                {
                    DirectSyncAttribute a = (DirectSyncAttribute)Attribute.GetCustomAttribute(fi, typeof(DirectSyncAttribute));

                    if (a == null) continue;
                }


                Object val = fi.GetValue(from);
                fi.SetValue(to, val);

            }
        }

        internal static void ForeignKeyCopy(Serializer context, ISerializableObject from, ISerializableObject to, FieldLock fieldLock)
        {
            AttributeWorker w = AttributeWorker.GetInstance(context.Target);

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(from.GetType());

            foreach (FieldInfo fi in fis)
            {
                if (fieldLock.IsLocked(fi)) continue;
                if (!w.IsForeignKey(context, fi)) continue;

                Object val = fi.GetValue(from);
                fi.SetValue(to, val);

            }
        }
    }
}
