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
using System.Windows.Forms;//Bitte für WPF entfernen
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.SyncBase;
//using UBT.AI4.Bio.DivMobi.MediaServiceApplication;
using System.IO;
using System.Security.Cryptography;

namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    public class AnalyzeSyncObjectList
    {
        internal Serializer syncSerializer;
        internal Serializer sourceSerializer;
        internal Serializer sinkSerializer;

        internal bool analyzed;
        internal bool synchronized;
        internal List<ISerializableObject> orderedSourceObjects;
        internal List<ListContainer> analyzedList;
        internal IPictureTransfer pictrans;

        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AnalyzeSyncObjectList));
        public AnalyzeSyncObjectList(ObjectSyncList list, Serializer source, Serializer sink, Serializer sync)
        {
            if (list.initialized == false)
            {
                throw new Exception();
            }
            sourceSerializer = source;
            sinkSerializer = sink;
            syncSerializer = sync;
            orderedSourceObjects = list.objectList;
            analyzedList = new List<ListContainer>();
            analyzed = false;
        }
        public AnalyzeSyncObjectList(ObjectSyncList list, Serializer source, Serializer sink, Serializer sync,IPictureTransfer pictrans):this(list,source, sink, sync)
        {
            this.pictrans = pictrans;    
        }

        public void analyzeAll()
        {
            foreach (ISerializableObject iso in orderedSourceObjects)
            {
                analyzeObject(iso);
            }
            analyzed = true;
        }

        public void analyzeAllInsertOnly()
        {
            foreach (ISerializableObject iso in orderedSourceObjects)
            {
                analyzeObjectInsertOnly(iso);
            }
            analyzed = true;
        }

        private void analyzeObjectInsertOnly(ISerializableObject iso)
        {
            ISerializableObject source = iso;
            Guid syncGuid = source.Rowguid;
            SyncItem syncItem = FetchSyncItem(syncGuid);
            if (syncItem == null)//In der Synchronisationstabelle gibt es keinen korrepondierenden Eintrag
            {
#if DEBUG
                logger.Debug(source.GetType().Name + ", guid=" + syncGuid + " is not synchronized and will be inserted");
#endif
                analyzedList.Add(new ListContainer(iso, SyncStates_Enum.InsertState));
            }
            else
            {
#if DEBUG
                logger.Debug(syncItem.ClassID + ", guid=" + syncGuid + " has not changed and will not be synchronized");
#endif
                analyzedList.Add(new ListContainer(iso, SyncStates_Enum.IgnoreState));
            }
            //Hier CheckForeignKeyConstraintüberprüfung analog zu Daniel einbauen?
        }


        private void analyzeObject(ISerializableObject iso)
        {
            ISerializableObject source = iso;
            ISerializableObject sink;
            Guid syncGuid = source.Rowguid;
            SyncItem syncItem = FetchSyncItem(syncGuid);
            if (syncItem == null)//In der Synchronisationstabelle gibt es keinen korrepondierenden Eintrag
            {
#if DEBUG
                logger.Debug(source.GetType().Name + ", guid=" + syncGuid + " is not synchronized and will be inserted");
#endif
                analyzedList.Add(new ListContainer(iso, SyncStates_Enum.InsertState));
            }
            else //in der Synchronisationstabelle ist ein Eintrag vorhanden
            {
                sink = FetchObject(sinkSerializer, syncGuid, source.GetType());
                if (sink == null)//Wenn es nicht vorhanden ist wurde es gelöscht
                {
#if DEBUG
                    logger.Debug(syncItem.ClassID + ", guid=" + syncGuid + " does not exist, the conflict has to be resolved");
#endif

                    analyzedList.Add(new ListContainer(iso, SyncStates_Enum.DeletedState));
                }
                else
                {
                    DateTime sinkLogtime = sink.LogTime;
                    if (sinkLogtime>syncItem.LogTime)//Änderung der Daten in der Zieldatenbank--warum ist das schon ein Konflikt?
                    {
#if DEBUG
                        logger.Debug(syncItem.ClassID + ", guid=" + syncGuid + " is involved in a conflict, which has to be resolved");
#endif
                        analyzedList.Add(new ListContainer(iso, SyncStates_Enum.ConflictState));
                    }
                    else //zieldatenbank unverändert
                    {

                        DateTime sourceLogtime = source.LogTime;
                        if (sourceLogtime<=syncItem.LogTime)//Quelldatenbank unverändert
                        {
#if DEBUG
                            logger.Debug(syncItem.ClassID + ", guid=" + syncGuid + " has not changed and will not be synchronized");
#endif
                            analyzedList.Add(new ListContainer(iso, SyncStates_Enum.IgnoreState));
                        }
                        else//Quelldatenbank verändert-Zieldatenbank unverändert ->update
                        {
#if DEBUG
                            logger.Debug(syncItem.ClassID + ", guid=" + syncGuid + " has changed on the sourceside and will be updated");
#endif
                            analyzedList.Add(new ListContainer(iso, SyncStates_Enum.UpdateState));
                        }
                    }
                }
            }
        }

    //            //Fehlt SyncTime->Überarbeitung der DB
    //            private void analyzeObject2(ISerializableObject iso)
    //            {
    //                ISerializableObject source = iso;
    //                Guid commomGuid = source.Rowguid;
    //                ISerializableObject sink=FetchObject(sinkSerializer, commomGuid, source.GetType());   
    //                if (sink == null)//In der PartnerDB gibt es keinen korrespondierenden Eintrag
    //                {
    //                 if(synctime==null)
    //                     {
    //#if DEBUG
    //                         logger.Debug(source.GetType().Name + ", guid=" + commomGuid + " is not synchronized and will be inserted");
    //#endif
    //                        analyzedList.Add(new ListContainer(iso, SyncStates_Enum.InsertState));
    //                     }
    //                 else
    //                     DELETED
    //                }
    //                else //Es gibt einen Partner
    //                {
    //                    DateTime sinkLogtime = sink.LogTime;
    //                    DateTime sourceLogTime = source.LogTime;
    //                    //Fehlt neutraler Zeitpunkt syncTime
    //                    if (sinkLogtime > synctime)//Änderung der Daten in der Zieldatenbank--warum ist das schon ein Konflikt?
    //                    {
    //    #if DEBUG
    //                        logger.Debug(source.GetType().Name + ", guid=" + commomGuid + " is involved in a conflict, which has to be resolved");
    //    #endif
    //                        analyzedList.Add(new ListContainer(iso, SyncStates_Enum.ConflictState));
    //                    }
    //                    else //zieldatenbank unverändert
    //                    {
    //                        if (sourceLogTime==sinkLogtime==synctime)//Quelldatenbank unverändert
    //                        {
    //    #if DEBUG
    //                            logger.Debug(source.GetType().Name + ", guid=" + commomGuid + " has not changed and will not be synchronized");
    //    #endif
    //                            analyzedList.Add(new ListContainer(iso, SyncStates_Enum.IgnoreState));
    //                        }
    //                        else//Quelldatenbank verändert-Zieldatenbank unverändert ->update
    //                        {
    //    #if DEBUG
    //                            logger.Debug(source.GetType().Name + ", guid=" + commomGuid + " has changed on the sourceside and will be updated");
    //    #endif
    //                            analyzedList.Add(new ListContainer(iso, SyncStates_Enum.UpdateState));
    //                        }
    //                    }
    //                }
    //            }

        public List<ListContainer> getObjectsOfState(SyncStates_Enum state)
        {
            List<ListContainer> list = new List<ListContainer>();
            foreach (ListContainer lc in analyzedList)
            {
                if (lc.State.Equals(state))
                    list.Add(lc);
            }
            return list;
        }

        public void synchronizeAll()
        {
            if (analyzed == false)
                return;
            if (synchronized == true)
                return;
            //Hier prüfen, ob noch Konflikte/Deletes/Prematures vorhanden sind.
            sinkSerializer.Connector.BeginTransaction();
            syncSerializer.Connector.BeginTransaction();
            foreach (ListContainer sc in analyzedList)
            {

                switch (sc.State)
                {
                    case SyncStates_Enum.PrematureState:
                        throw new Exception("The Syncontainer is not initialized properly! Premature State is impossible after Initialization");
                        break;
                    case SyncStates_Enum.IgnoreState:
                        ignore(sc);
                        break;
                    case SyncStates_Enum.InsertState:
                        insert(sc);
                        break;
                    case SyncStates_Enum.UpdateState:
                        update(sc);
                        break;
                    case SyncStates_Enum.DeletedState:
                        //delete(sc);
                        break;
                    case SyncStates_Enum.ConflictState:
                        throw new Exception("Conflicts need to be resolved before Synchronization!");//ConflictHandling anwerfen?
                        break;
                    case SyncStates_Enum.ConflictResolvedState:
                        conflictResolved(sc);
                        break;
                    case SyncStates_Enum.SynchronizedState:
                        throw new Exception("The SyncContainer is already synchronized");
                    default:
                        throw new Exception("No SyncState assigned. Analyze first");
                        break;
                }
            }
            syncSerializer.Connector.Commit();
            
            sinkSerializer.Connector.Commit();
        }

        private void ignore(ListContainer sc)
        {
            sc.State = SyncStates_Enum.SynchronizedState;
        }

        private void insert(ListContainer sc)
        {
            ISerializableObject sourceObject = sc.iso;
            ISerializableObject sinkObject = sinkSerializer.CreateISerializableObject(sourceObject.GetType(),sc.iso.Rowguid);//Hier werden die Relationen mitgefüllt. Eigentlich ist da nicht nötig->Methode ändern? Zeitersparnis?
            //Evtl. Problem, dass nicht alle Beziehungen aufgelöst werden können, weil noch nicht alle Objekte "rübersynchronisert" wurden.
            copyToSinkObject(sourceObject, sinkObject);
            if (LookupSynchronizationInformation.getImageClasses().Contains(sourceObject.GetType()))
            {
                try
                {
                    MessageBox.Show("Insert Picture:" + sinkObject.Rowguid.ToString());
                    if (this.pictrans == null)
                        throw new TransferException("No TransferService is added");
                    insertPicture(sinkObject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Picture SyncFailure: " + ex.Message);
                    return;
                }
            }
            try
            {
                adjustAutoIncKeys(sinkObject);
                sinkSerializer.Connector.InsertPlain(sinkObject);
                SyncItem si = syncSerializer.CreateISerializableObject<SyncItem>();
                si.SyncGuid = sourceObject.Rowguid;
                si.ClassID = sourceObject.GetType().Name;
                syncSerializer.Connector.Save(si);
                sinkObject = FetchObject(sinkSerializer, sinkObject.Rowguid, sinkObject.GetType());//Mit angepasstem AutoIncSChlüssel aus der DB holen
                sc.State = SyncStates_Enum.SynchronizedState;
            }
            catch (Exception e)
            {
                MessageBox.Show("Insert Failure: " + e.Message + " All dependent Insert will also Fail and notice You");
            }
        }

        private void update(ListContainer sc)
        {
            try
            {
                ISerializableObject sourceObject = sc.iso;
                SyncItem si = FetchSyncItem(sourceObject.Rowguid);
                Guid sinkGuid = sourceObject.Rowguid;
                ISerializableObject sinkObject = FetchObject(sinkSerializer, sinkGuid, sourceObject.GetType());
                if (LookupSynchronizationInformation.getImageClasses().Contains(sourceObject.GetType()))
                {
                    //Bilder können nicht aktualisiert werden
                    ignore(sc);
                    return;
                }
                else
                {
                    copyToSinkObject(sourceObject, sinkObject);
                    adjustAutoIncKeys(sinkObject);//Kann man verhindern, dass "richtige" Schlüssel zunächst falsch überschrieben und dann wieder richt zurücküberschrieben werden?
                }
                sinkSerializer.Connector.UpdatePlain(sinkObject);
                syncSerializer.Connector.Save(si);

                sinkObject = FetchObject(sinkSerializer, sinkGuid, sourceObject.GetType());
                sc.State = SyncStates_Enum.SynchronizedState;
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Failure: " + e.Message);
            }
        }

        private void delete(ListContainer sc)
        {

            //Wenn nach Bestätigung immer noch deleted: Löscht sc.iso aus der source-Datenbank und die SyncItems aus der Synchronisationsdatenbank.
            ISerializableObject sourceObject = sc.iso;
            SyncItem syncItem = FetchSyncItem(sourceObject.Rowguid);
            sourceSerializer.Connector.Delete(sourceObject);
            syncSerializer.Connector.Delete(syncItem);
            sc.State = SyncStates_Enum.SynchronizedState;
        }

        private void conflictResolved(ListContainer sc)
        {
            //Idee: Teile diese Typ in update des sourceObjects beim ConflictHandling und update des sinkObjects bei der Synchronisation
            update(sc);
        }

        private void adjustAutoIncKeys(ISerializableObject manipulatedObject)
        {
            try
            {
                Type t = manipulatedObject.GetType();
                //1.Abhängigkeiten aus Liste holen <eigenes Feld,Klasse>
                Dictionary<String, Type> foreignKeys = LookupSynchronizationInformation.getDeterminingFields(t);
                foreach (KeyValuePair<String, Type> fk in foreignKeys)
                {
                    FieldInfo foreignAutoInc = t.GetField(fk.Key, BindingFlags.NonPublic | BindingFlags.Instance);//Enthält die Information darüber in welchem Feld das manipulatedObject einen fremden AutoInc-Key stehen hat.
                    if (foreignAutoInc.GetValue(manipulatedObject) != null)
                    {
                        //2.AutoincFeld der PartnerKlasse holen
                        String autoInc = LookupSynchronizationInformation.getAutoIncFields()[fk.Value];
                        //3.In der QuelldatenBank nach dem referenzierten AutoincSchlüssel suchen und das entsprechnde Objekt laden.
                        IRestriction r = RestrictionFactory.Eq(fk.Value, autoInc, foreignAutoInc.GetValue(manipulatedObject));
                        ISerializableObject parent = sourceSerializer.Connector.Load(fk.Value, r);
                        Guid g = parent.Rowguid;
                        //4. synchronisierten Partner in der Zieldatenbank über Guid holen
                        ISerializableObject parentPartner = FetchObject(sinkSerializer, g, fk.Value);
                        FieldInfo partnerField = fk.Value.GetField(autoInc, BindingFlags.Instance | BindingFlags.NonPublic);
                        //5. Werte anpassen
                        foreignAutoInc.SetValue(manipulatedObject, partnerField.GetValue(parentPartner));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("KeyAdjustmentError: " + e.Message);
                if (e.InnerException != null)
                    MessageBox.Show(e.InnerException.Message);
                throw new SerializerException("KeyAdjustmentError: " + e.Message);
            }
        }

        
        private void insertPicture(ISerializableObject manipulatedObject)
        {
            this.pictrans.transferPicture(manipulatedObject);
        }
        


        internal void copyToSinkObject(ISerializableObject from, ISerializableObject to)
        {
            AttributeWorker w = AttributeWorker.GetInstance(sinkSerializer.Target);//Am besten durch Kopiervorgang ersetzen, der ohne Attributeworker auskommt.
            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(from.GetType());

            foreach (FieldInfo fi in fis)
            {
                if (w.IsAutoincID(fi)) continue;//Kann man aus Lookup-Tabelle holen
                if (!w.IsPersistentField(fi)) continue;//wirklich  nötig?
                //DirectSync wird im Moment nicht unterstützt
                if (AttributeWorker.IsRowGuid(fi)) continue;//Kann man als Lookup-Tabelle definieren
                Object val = fi.GetValue(from);
                fi.SetValue(to, val);
            }
        }

        private ISerializableObject FetchObject(Serializer s, Guid guid, Type synchronizedType)//Von Daniel adaptiert->Überarbeiten
        {
            FieldInfo fi = AttributeWorker.RowGuid(synchronizedType);
            String col = AttributeWorker.GetInstance(s.Target).GetColumnMapping(fi);
            StringBuilder tmp = new StringBuilder();
            tmp.Append(col).Append("='").Append(guid).Append("'"); ;
            IRestriction res = RestrictionFactory.SqlRestriction(synchronizedType, tmp.ToString());
            ISerializableObject iso = s.Connector.Load(synchronizedType, res);
            return iso;
        }

        private SyncItem FetchSyncItem(Guid guid)
        {
            IRestriction r = RestrictionFactory.Eq(typeof(SyncItem), "_SyncGuid", guid);
            SyncItem siSource = (SyncItem)syncSerializer.Connector.Load(typeof(SyncItem), r);
            return siSource;
        }

        internal static String ComputeHashCode(String target, ISerializableObject iso)
        {
            AttributeWorker w = AttributeWorker.GetInstance(target);

            FieldInfo[] fis = AttributeWorker.RetrieveAllFields(iso.GetType());

            StringBuilder b = new StringBuilder();
            foreach (FieldInfo fi in fis)
            {
                if (w.IsPersistentField(fi) && !w.IsID(fi))
                {
                    Object val = fi.GetValue(iso);
                    b.Append(val);
                }
            }

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] tmp = encoding.GetBytes(b.ToString());


            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(tmp)).Replace("-", "").ToLower();

        }
      
    }
}
