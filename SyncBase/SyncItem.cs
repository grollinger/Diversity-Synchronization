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
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Collections;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes;


namespace UBT.AI4.Bio.DivMobi.SyncBase
{
    [Table("MappingDictionary")]
    public class SyncItem : ISerializableObject
    {
        

        [Column]
        private Guid _SyncGuid;
        //[ID(Autoinc=true)]
        //[Column]
        //private int? _SyncID;
        //[Column]
        //private int? _SyncFK;
        [Column]
        private String _ClassID;
        //[Column]
        //private String _Hashcode;
        [Column]
        private DateTime _SyncTime;
        [RowGuid]
        [Column]
        private Guid _RowGuid;


        //[OneToOneDef(Reflexive = true, DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_SyncID", JoinColumn = "_SyncFK")]
        //private SyncItem _CounterPart;

        //[OneToMany(DeleteType = DeleteTypes.CASCADE)]
        //[JoinColums(DefColumn = "_SyncID", JoinColumn = "_SyncFK")]
        //private IDirectAccessIterator<FieldState> _FieldStates;


        public Guid SyncGuid { get { return _SyncGuid; } set { _SyncGuid = value; } }

        //public SyncItem CounterPart
        //{
        //    get { return _CounterPart; }
        //    set { _CounterPart = value; }
        //}

        public String ClassID 
        {
            get { return _ClassID; }
            set { _ClassID = value; }
        }

        public DateTime SyncTime
        {
            get {return _SyncTime;}
            set { _SyncTime = value; }

        }
        
        //public String Hashcode
        //{
        //    get { return _Hashcode; }
        //    set { _Hashcode = value; }
        //}

        public Guid Rowguid
        {
            get { return _RowGuid; }
            set { _RowGuid = value; }
        }
        public DateTime LogTime { get { return _SyncTime; } set { _SyncTime = value; } }
        //public IDirectAccessIterator<FieldState> FieldStates
        //{
        //    get { return _FieldStates; }
        //    set { _FieldStates = value; }
        //}


        //public bool HasChanged(String target, ISerializableObject iso)
        //{
        //    String hc = ComputeHashCode(target, iso);
        //    return hc.Equals(_Hashcode);
        //}

        public bool HasChanged(ISerializableObject iso)
        {
            return (iso.LogTime > _SyncTime);
        }
        

        public static String ComputeHashCode(String target, ISerializableObject iso)
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
    [Table("MappingDictionary")]
    public class FieldState : ISerializableObject
    {
        [ID]
        [Column]
        private int _SyncFK;
        [ID]
        [Column]
        private String _FieldName;
        [Column]
        private String _HashCode;
        [RowGuid]
        [Column]
        private Guid _RowGuid;
        //[ManyToOne]
        //[MappedBy("_FieldStates")]
        //private SyncItem _SyncItem;

        public String FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        public String HashCode
        {
            get { return _HashCode; }
            set { _HashCode = value; }
        }
        public DateTime LogTime { get { return new DateTime(1900, 1, 1); } set { } }//Die Einträge der Tabelle werden nicht synchronisiert. Sie werden nur benötigt um die Schnittstelle zu implementieren.
        public Guid Rowguid
        {
            get { return _RowGuid; }
            set { _RowGuid = value; }
        }

        //public SyncItem SyncItem
        //{
        //    get { return _SyncItem; }
        //    set { _SyncItem = value; }
        //}

        public void ComputeFieldState(FieldInfo fi, ISerializableObject iso)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            FieldName = fi.Name;
            StringBuilder b = new StringBuilder();
            b.Append(fi.GetValue(iso));
            Byte[] tmp = encoding.GetBytes(b.ToString());
            HashCode = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(tmp)).Replace("-", "").ToLower();
        }
    }
}
