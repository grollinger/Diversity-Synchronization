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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer;
using UBT.AI4.Bio.DivMobi.ListSynchronization.MediaServiceProxy;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;

namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    public class SNSBPictureTransfer:IPictureTransfer
    {
        private String _userName;
        private string _pictureDirectory;
        private int _project;        
        Serializer _sourceSerializer;

        public SNSBPictureTransfer(String userName, int projectID, Serializer sourceSerializer, string pictureDirectory)
        {            
            this._userName = userName;
            this._project = projectID;
            this._sourceSerializer = sourceSerializer;
            this._pictureDirectory = pictureDirectory;
        }

        /*public bool testPictureTranfer(String fullPath)
        {
            bool success = false;
            string rowGuid = Guid.Empty.ToString();
            string pathName = String.Empty;
            string fileName = String.Empty;
            string type = String.Empty;
            float latitude = 0;
            float longitude = 0;
            float altitude = 0;
            string author = String.Empty;
            string timestamp = string.Empty;
            int projectId = 0;
            author = "Test";

            pathName = fullPath;

            

            
            FileStream fileStrm = null;
            BinaryReader rdr = null;
            byte[] data = null;
            DateTime start = DateTime.Now;
            String retString = String.Empty;
            try
            {
                // Create stream and reader for file data
                fileStrm = new FileStream(pathName, FileMode.Open, FileAccess.Read);
                rdr = new BinaryReader(fileStrm);

            }
            catch(Exception e)
            {
                
                StringBuilder sb = new StringBuilder("Picture not found: ").Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append(",");
                    sb.Append(e.InnerException.Message);
                }
                if (rdr != null)
                    rdr.Close();
                throw new Exception(sb.ToString());
            }


            try
            {
                // Number of bytes to be transferred
                long numBytes = fileStrm.Length;

                // Package counter
                int count = 0;
                // Return string
                fileName = Path.GetFileName(pathName);

                if (numBytes > 0)
                {
                    data = rdr.ReadBytes((int)numBytes);
                    count++;
                    retString = _service.Submit(fileName, fileName, type, latitude, longitude, altitude, author, timestamp, projectId, data); // IDs 372, 373, 374
                }
                TimeSpan dif = DateTime.Now - start;

                if (retString.StartsWith("Success"))
                {
                    MessageBox.Show(retString);
                    MessageBox.Show(dif.ToString() + " msec  -  " + count.ToString() + " packets transmitted");
                    success=true;
                }
                else
                {
                    MessageBox.Show("ERROR: " + retString);
                }

                // Close reader and stream
                rdr.Close();
                fileStrm.Close();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder("Transfer Error: ").Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append(",");
                    sb.Append(e.InnerException.Message);
                }
                if (rdr != null)
                    rdr.Close();
                if (fileStrm != null)
                    fileStrm.Close();
                throw new Exception(sb.ToString());
            }
            finally
            {
                // Abort faulted proxy
                if (_service.State == System.ServiceModel.CommunicationState.Faulted)
                {
                    // Webservice method call
                    // proxy.Rollback();
                    _service.Abort();
                }
                // Close proxy
                else if (_service.State == System.ServiceModel.CommunicationState.Opened)
                {
                    _service.Close();
                }
            }
            return success;
        }*/

        string _rowGuid;
        string _pathName;
        string _fileName;
        string _type;
        float _latitude;
        float _longitude;
        float _altitude;
        string _author;
        string _timestamp;
        int _projectId;
        DiversityMediaServiceClient _service;

        public void transferPicture(ISerializableObject iso)
        {            
            resetInformation();

            _service = new DiversityMediaServiceClient();
            

            if (_service.State != System.ServiceModel.CommunicationState.Opened)
            {
                try
                {
                    _service.Open();
                }
                catch (Exception e)
                {
                    StringBuilder sb = new StringBuilder("Cant Open Connection: ").Append(e.Message);
                    if (e.InnerException != null)
                    {
                        sb.Append(",");
                        sb.Append(e.InnerException.Message);
                    }

                    throw new Exception(sb.ToString());
                }
            }
            _author = this._userName;
            _projectId = this._project;            
            //Fallunterscheidung nach ImageTypeClasse um benötigte informationen zu bekommen
            try
            {
                if (iso is CollectionEventImage)
                {
                    CollectionEventImage cei = (CollectionEventImage)iso;
                    _rowGuid = cei.Rowguid.ToString();
                    string pureFileName = System.IO.Path.GetFileName(cei.URI);
                    string path = System.IO.Directory.GetCurrentDirectory();
                    StringBuilder sb = new StringBuilder(_pictureDirectory);
                    sb.Append("\\");
                    sb.Append(pureFileName);
                    _pathName = sb.ToString();
                    _type = cei.ImageType;
                    IRestriction re = RestrictionFactory.Eq(typeof(CollectionEvent), "_CollectionEventID", cei.CollectionEventID);
                    CollectionEvent ce = _sourceSerializer.Connector.Load<CollectionEvent>(re);
                    IRestriction r1 = RestrictionFactory.Eq(typeof(CollectionEventLocalisation), "_CollectionEventID", cei.CollectionEventID);
                    IRestriction r2 = RestrictionFactory.Eq(typeof(CollectionEventLocalisation), "_LocalisationSystemID", 8);
                    IRestriction r = RestrictionFactory.And().Add(r1).Add(r2);
                    CollectionEventLocalisation cel = _sourceSerializer.Connector.Load<CollectionEventLocalisation>(r);
                    if (cel != null)
                    {
                        if (cel.AverageAltitudeCache != null)
                            _longitude = (float)cel.AverageLongitudeCache;
                        if (cel.AverageLatitudeCache != null)
                            _latitude = (float)cel.AverageLatitudeCache;
                        if (cel.AverageLongitudeCache != null)
                            _altitude = (float)cel.AverageAltitudeCache;
                    }
                    _timestamp = cei.LogTime.ToString();
                }
                else if (iso.GetType().Equals(typeof(CollectionSpecimenImage)))
                {
                    CollectionSpecimenImage csi = (CollectionSpecimenImage)iso;
                    _rowGuid = csi.Rowguid.ToString();
                    string pureFileName = System.IO.Path.GetFileName(csi.URI);
                    string path = System.IO.Directory.GetCurrentDirectory();
                    StringBuilder sb = new StringBuilder(_pictureDirectory);
                    sb.Append("\\");
                    sb.Append(pureFileName);
                    _pathName = sb.ToString();
                    _type = csi.ImageType;
                    IRestriction re = RestrictionFactory.Eq(typeof(CollectionSpecimen), "_CollectionSpecimenID", csi.CollectionSpecimenID);
                    CollectionSpecimen cs = _sourceSerializer.Connector.Load<CollectionSpecimen>(re);
                    CollectionEvent ce = cs.CollectionEvent;
                    IRestriction r1 = RestrictionFactory.Eq(typeof(CollectionEventLocalisation), "_CollectionEventID", ce.CollectionEventID);
                    IRestriction r2 = RestrictionFactory.Eq(typeof(CollectionEventLocalisation), "_LocalisationSystemID", 8);
                    IRestriction r = RestrictionFactory.And().Add(r1).Add(r2);
                    CollectionEventLocalisation cel = _sourceSerializer.Connector.Load<CollectionEventLocalisation>(r);
                    if (cel != null)
                    {
                        if (cel.AverageAltitudeCache != null)
                            _longitude = (float)cel.AverageLongitudeCache;
                        else
                            _longitude = 0;
                        if (cel.AverageLatitudeCache != null)
                            _latitude = (float)cel.AverageLatitudeCache;
                        else
                            _latitude = 0;
                        if (cel.AverageLongitudeCache != null)
                            _altitude = (float)cel.AverageAltitudeCache;
                        else
                            _altitude = 0;
                    }
                    else
                    {
                        _latitude = _longitude = _altitude = 0;
                    }
                    _timestamp = csi.LogTime.ToString();
                }
                else
                {
                    throw new TransferException("ImageClass not Supported");
                }
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder("Corresponding data not found: ").Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append(",");
                    sb.Append(e.InnerException.Message);
                }

                throw new Exception(sb.ToString());
            }
            FileStream fileStrm = null;
            BinaryReader rdr = null;
            byte[] data = null;
            DateTime start = DateTime.Now;
            String retString = String.Empty;
            try
            {
                // Create stream and reader for file data
                fileStrm = new FileStream(_pathName, FileMode.Open, FileAccess.Read);
                rdr = new BinaryReader(fileStrm);

            }
            catch(Exception e)
            {
                
                StringBuilder sb = new StringBuilder("Picture not found: ").Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append(",");
                    sb.Append(e.InnerException.Message);
                }
                if (rdr != null)
                    rdr.Close();
                throw new Exception(sb.ToString());
            }
            try
            {
                // Number of bytes to be transferred
                long numBytes = fileStrm.Length;

                // Package counter
                int count = 0;
                // Return string
                _fileName = Path.GetFileName(_pathName);

                if (numBytes > 0)
                {
                    data = rdr.ReadBytes((int)numBytes);
                    count++;
                    //retString = f.ReadFileAndTransfer(pathName);
                    retString = _service.Submit(_fileName, _fileName, _type, _latitude, _longitude, _altitude, _author, _timestamp, _projectId, data); // IDs 372, 373, 374
                }
                TimeSpan dif = DateTime.Now - start;

                if (retString.StartsWith("http"))
                {
                    MessageBox.Show(retString);
                    MessageBox.Show(dif.ToString() + " msec  -  " + count.ToString() + " packets transmitted");
                }
                else
                {
                    MessageBox.Show("ERROR: " + retString);
                }

                // Close reader and stream
                rdr.Close();
                fileStrm.Close();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder("Transfer Error: ").Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append(",");
                    sb.Append(e.InnerException.Message);
                }
                if (rdr != null)
                    rdr.Close();
                if (fileStrm != null)
                    fileStrm.Close();
                throw new Exception(sb.ToString());
            }
            finally
            {
                // Abort faulted proxy
                if (_service.State == System.ServiceModel.CommunicationState.Faulted)
                {
                    // Webservice method call
                    // proxy.Rollback();
                    _service.Abort();
                }
                // Close proxy
                else if (_service.State == System.ServiceModel.CommunicationState.Opened)
                {
                    _service.Close();
                }
            }
            if (iso.GetType().Equals(typeof(CollectionEventImage)))
            {
                CollectionEventImage cei = (CollectionEventImage)iso;
                cei.URI = retString;
            }
            if (iso.GetType().Equals(typeof(CollectionSpecimenImage)))
            {
                CollectionSpecimenImage csi = (CollectionSpecimenImage)iso;
                csi.URI = retString;
            }
            // Close reader and stream
            rdr.Close();
            fileStrm.Close();
        }

        private void resetInformation()
        {
            _rowGuid = Guid.Empty.ToString();
            _pathName = String.Empty;
            _fileName = String.Empty;
            _type = String.Empty;
            _latitude = 0f;
            _longitude = 0f;
            _altitude = 0f;
            _author = String.Empty;
            _timestamp = string.Empty;
            _projectId = 0;
        }

        //public System.ServiceModel.ICommunicationObject TransferService
        //{
        //    get
        //    {
        //        return _service;
        //    }
        //    set
        //    {
        //        if (value.GetType().Equals(typeof(DiversityMediaServiceClient)))
        //        {
        //            _service = (DiversityMediaServiceClient)value;
        //        }
        //        else
        //        {
        //            throw new TransferException();
        //        }
        //    }
        //}
    }
}
