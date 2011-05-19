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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections
{
    internal class DirectAccessIteratorStrategy : OneToManyStrategy
    {
        public override Object LoadOneToMany(ISerializableObject owner, Type storedType, Serializer serializer)
        {
            Type genType = typeof(DirectAccesIteratorImpl<>).MakeGenericType(new Type[] {storedType});
            IDirectAccessIteratorConfiguration conf = (IDirectAccessIteratorConfiguration)Activator.CreateInstance(genType);

            IVirtualKey vKey = AttributeWorker.GetInstance(serializer.Target).CreateVirtualKey(serializer, owner.GetType(), storedType);
            vKey.InitVirtualKeyBySource(owner);

            conf.Restriction = Restrictions.RestrictionFactory.SqlRestriction(storedType, vKey.ToSqlStringForward(serializer.Target));
            conf.Serializer = serializer;

            return conf;
        }

        public override void UpdateOneToMany(Object iterator, ISerializableObject owner, Type storedType, Serializer serializer)
        {
            IVirtualKey vKey = AttributeWorker.GetInstance(serializer.Target).CreateVirtualKey(serializer, owner.GetType(), storedType);
            vKey.InitVirtualKeyBySource(owner);

            ((IDirectAccessIteratorConfiguration)iterator).Restriction = Restrictions.RestrictionFactory.SqlRestriction(storedType, vKey.ToSqlStringForward(serializer.Target));
        }
    }
}
