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

using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;


namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    public class ObjectSyncList
    {
        internal bool initialized;
        internal List<ISerializableObject> objectList;
        internal Dictionary<Guid, ISerializableObject> objectGuidMapping;       

        public ObjectSyncList()
        {
            initialized = false;
            objectList = new List<ISerializableObject>();
            objectGuidMapping = new Dictionary<Guid, ISerializableObject>();
        }

        public void addObject(ISerializableObject iso)
        {
            Guid g = iso.Rowguid;
            if (!objectGuidMapping.ContainsKey(g))
            {
                initialized = false;
                objectList.Add(iso);
                objectGuidMapping.Add(g, iso);
            }
        }

        public void addList(IList<ISerializableObject> list)
        {
            if (list.Count == 0)
                return;
            foreach (ISerializableObject iso in list)
            {
                addObject(iso);
            }
        }

        public void initialize(List<Type> classHierarchyList,Dictionary<Type,String> reflexiveFieldsOfType, Dictionary<Type,String> idStorageOfType)
        {
            //Fehlt Konistenzprüfung ob für alle Objecte auch die KLassen in classes stehen.
            objectList=sortParentsUp(objectList, classHierarchyList, reflexiveFieldsOfType, idStorageOfType);
            initialized = true;
        }

       
        public void Load(Type t, Serializer serializer)
        {
            IRestriction r = RestrictionFactory.TypeRestriction(t);
            IList<ISerializableObject> list = serializer.Connector.LoadList(t, r);
            addList(list);
        }

        //Der Algorithmus basiert darauf, dass Objekte die von anderen Objekten abhängen erst nach diesen Objekten synchronisiert werden.
        //Deswegen muss die Liste nach Abhängigkeiten synchronisert werden.
        public static List<ISerializableObject> sortParentsUp (List<ISerializableObject> unsorted,List<Type> classes,Dictionary<Type,String> reflexivRelation,Dictionary<Type,String> idStorage)
        {
            List<ISerializableObject> sorted =new List<ISerializableObject>();
            foreach (Type t in classes)
            {
                bool reflexiveClass = reflexivRelation.ContainsKey(t);
                if (reflexiveClass == false)
                {
                    foreach (ISerializableObject iso in unsorted)
                    {
                        if (iso.GetType().Equals(t))
                        {
                            sorted.Add(iso);
                        }
                    }
                }
                else
                {
                    List<ISerializableObject> tmpIn = new List<ISerializableObject>();
                    List<ISerializableObject> tmpOut= new List<ISerializableObject>();
                    foreach (ISerializableObject iso in unsorted)
                    {
                        if (iso.GetType().Equals(t))
                        {
                            tmpIn.Add(iso);
                        }
                    }
                    while (tmpIn.Count > 0)//Achtung bei inkonsistenten Daten kann eine Endlosschleife entsehen
                    {
                        int count = 0;
                        String autoMapping = reflexivRelation[t];
                        FieldInfo am = t.GetField(autoMapping,BindingFlags.Instance | BindingFlags.NonPublic);
                        String idField = idStorage[t];
                        FieldInfo id = t.GetField(idField,BindingFlags.Instance | BindingFlags.NonPublic);
                        foreach(ISerializableObject iso in tmpIn)
                        {
                            if (am.GetValue(iso) == null || containsParent(tmpOut, id, am.GetValue(iso)) == true)
                            {                              
                                tmpOut.Add(iso);
                                count++;
                            }
                        }
                        foreach (ISerializableObject iso in tmpOut)
                        {
                            tmpIn.Remove(iso);
                        }
                        if (count == 0) //In der vorangegangenen Iterartion wurde kein neues Element gefunden->Inkonsistenz->Endlosschleife wird durch Exception durchbrochen
                            throw new Exception("Es existiert kein Vaterobjekt! in der Datenbank");
                            //tmpIn.Clear();
                            //TODO: Abhängige Objekte aus Liste löschen
                    }
                    foreach (ISerializableObject iso in tmpOut)
                    {
                        sorted.Add(iso);
                    }
                }
            }
            return sorted;
        }

        private static bool containsParent(List<ISerializableObject> list, FieldInfo fi, Object value)
        {
            foreach(ISerializableObject iso in list)
            {
                if (fi.GetValue(iso).Equals(value))
                    return true;
            }
            return false;
        }

        public List<ISerializableObject> orderedObjects
        {
            get
            {
                if (this.initialized == true)
                    return this.objectList;
                else return null;
            }
        }
    }
}
