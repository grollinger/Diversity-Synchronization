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
using System.Collections;
using System.Reflection;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;


namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Relations
{
    public class ResolverData<T>
    {
        private T _handledItem;
        private FieldInfo[] _fieldsToResolve;

        public T HandledItem
        {
            get { return _handledItem; }
            set { _handledItem = value; }
        }

        public FieldInfo[] FieldsToResolve
        {
            get { return _fieldsToResolve; }
            set { _fieldsToResolve = value; }
        }
    }

    public interface IRelationHandler<T>
    {
        void HandleOneToMany(T handledItem, OneToManyAttribute attr, FieldInfo field);
        void HandleManyToOne(T handledItem, ManyToOneAttribute attr, FieldInfo field);
        void HandleOneToOneReflexive(T handledItem, OneToOneDefAttribute attr, FieldInfo field);
    }

    public class LoadHandler : IRelationHandler<ISerializableObject>
    {
        private Serializer _serializer;

        public LoadHandler(Serializer serializer)
        {
            _serializer = serializer;
        }

        public void HandleOneToMany(ISerializableObject handledItem, OneToManyAttribute attr, FieldInfo field)
        {
            OneToManyStrategy s = new DirectAccessIteratorStrategy();
            Type type = field.FieldType.GetGenericArguments()[0];
            Object tmp = s.LoadOneToMany(handledItem, type, _serializer);
            field.SetValue(handledItem, tmp); 
        }

        public void HandleManyToOne(ISerializableObject handledItem, ManyToOneAttribute attr, FieldInfo field)
        {
            Type type = field.FieldType;
            IVirtualKey vKey = AttributeWorker.GetInstance(_serializer.Target).CreateVirtualKey(_serializer, type, handledItem.GetType());
            try
            {
                vKey.InitVirtualKeyByTarget(handledItem);
            }
            catch (InvalidOperationException)
            {
                field.SetValue(handledItem, null);
                return;
            }
            IRestriction r = Restrictions.RestrictionFactory.SqlRestriction(type, vKey.ToSqlStringBackward(_serializer.Target));
            ISerializableObject tmp = _serializer.Connector.Load(type, r);
            field.SetValue(handledItem, tmp);
        }

        public void HandleOneToOneReflexive(ISerializableObject handledItem, OneToOneDefAttribute attr, FieldInfo field)
        {
            Type type = field.FieldType;
            IVirtualKey vKey = AttributeWorker.GetInstance(_serializer.Target).CreateVirtualKey(_serializer, type, handledItem.GetType());
            try
            {
                vKey.InitVirtualKeyBySource(handledItem);
            }
            catch (InvalidOperationException)
            {
                field.SetValue(handledItem, null);
                return;
            }
            IRestriction r = Restrictions.RestrictionFactory.SqlRestriction(type, vKey.ToSqlStringForward(_serializer.Target));
            ISerializableObject tmp = _serializer.Connector.Load(type, r);
            field.SetValue(handledItem, tmp);
        }
    }

    public class DeleteHandler : IRelationHandler<ISerializableObject>
    {
        private Serializer _serializer;

        public const int DOWNMODE = 0;
        public const int UPMODE = 1;
        private int _mode = DOWNMODE;


        public DeleteHandler(Serializer serializer)
        {
            _serializer = serializer;
        }

        public int MODE { set {_mode=value;} }

        public void HandleOneToMany(ISerializableObject handledItem, OneToManyAttribute attr, FieldInfo field)
        {
            if (_mode == DOWNMODE)
            {
                if (attr.DeleteType == DeleteTypes.CASCADE)
                {
                    IEnumerable en = (IEnumerable)field.GetValue(handledItem);
                    List<ISerializableObject> remove = new List<ISerializableObject>();
                    foreach (ISerializableObject iso in en)
                    {
                        remove.Add(iso);
                    }

                    foreach (ISerializableObject iso in remove)
                    {
                        _serializer.Connector.Delete(iso);
                    }
                }
                else if (attr.DeleteType == DeleteTypes.NOACTION)
                {
                    //noop
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public void HandleManyToOne(ISerializableObject handledItem, ManyToOneAttribute attr, FieldInfo field)
        {
            if (_mode == UPMODE)
            {
                field.SetValue(handledItem, null);
            }
        }

        public void HandleOneToOneReflexive(ISerializableObject handledItem, OneToOneDefAttribute attr, FieldInfo field)
        {
            if (_mode == UPMODE)
            {
                if (attr.DeleteType == DeleteTypes.CASCADE)
                {
                    ISerializableObject iso = (ISerializableObject)field.GetValue(handledItem);
                    _serializer.Connector.Delete(iso);
                }
                else
                {
                    throw new NotImplementedException();
                }

            }
        }
    }

    public class UpdateHandler : IRelationHandler<ISerializableObject>
    {
        private Serializer _serializer;

        public UpdateHandler(Serializer serializer)
        {
            _serializer = serializer;
        }

        public void HandleOneToMany(ISerializableObject handledItem, OneToManyAttribute attr, FieldInfo field)
        {
            OneToManyStrategy s = new DirectAccessIteratorStrategy();
            Type type = field.FieldType.GetGenericArguments()[0];
            s.UpdateOneToMany(field.GetValue(handledItem),handledItem, type, _serializer);
        }

        public void HandleManyToOne(ISerializableObject handledItem, ManyToOneAttribute attr, FieldInfo field)
        {
            //noop;
        }

        public void HandleOneToOneReflexive(ISerializableObject handledItem, OneToOneDefAttribute attr, FieldInfo field)
        {
            //noop;
        }
    }

    public class CreateHandler : IRelationHandler<ISerializableObject>
    {
        private Serializer _serializer;

        public CreateHandler(Serializer serializer)
        {
            _serializer = serializer;
        }

        public void HandleOneToMany(ISerializableObject handledItem, OneToManyAttribute attr, FieldInfo field)
        {
            
        }

        public void HandleManyToOne(ISerializableObject handledItem, ManyToOneAttribute attr, FieldInfo field)
        {
        
        }

        public void HandleOneToOneReflexive(ISerializableObject handledItem, OneToOneDefAttribute attr, FieldInfo field)
        {
            
        }
    }




    public class RelationResolver<T>
    {
        private IRelationHandler<T> _handler;

        public IRelationHandler<T> Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public void StartRelationResolving(ResolverData<T> resolverData)
        {
            foreach (FieldInfo fi in resolverData.FieldsToResolve)
            {
                //ResolveAttribute res = AttributeWorker.GetResolveAttribute(fi);
                //if (res == null)
                //    return;
                RelationalAttribute r = AttributeWorker.GetRelationAttribute(fi);

                if (r is OneToManyAttribute)
                {
                    _handler.HandleOneToMany(resolverData.HandledItem, (OneToManyAttribute)r, fi);
                } 
                else if(r is ManyToOneAttribute) 
                {
                    _handler.HandleManyToOne(resolverData.HandledItem, (ManyToOneAttribute)r, fi);
                }
                else if (r is OneToOneDefAttribute)
                {

                    Type storedType = fi.FieldType;//.GetGenericArguments()[0];
                    if (((OneToOneDefAttribute)r).Reflexive)
                    {
                        if (storedType == resolverData.HandledItem.GetType())
                        {
                            _handler.HandleOneToOneReflexive(resolverData.HandledItem, (OneToOneDefAttribute)r, fi);
                        }
                        else
                        {
                            throw new SerializerException("Reflexive Relations are only allowed in the same type.");
                        }
                    } 
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }   
        }
    }

}
