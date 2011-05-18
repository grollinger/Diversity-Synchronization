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
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;
using log4net;

namespace MVVMDiversity.Services
{
    public partial class FieldDataService : IFieldDataService
    {
        private ILog _Log = LogManager.GetLogger(typeof(FieldDataService));

        [Dependency]
        public IConnectionProvider Connections { get; set; }    

        public IEnumerable<SearchSpecification> SearchTypes
        {
            get { return Search.SearchTypes; }
        }
       
        public AsyncOperationInstance uploadData(string userNr, int projectID)
        {
            var op = new AsyncOperationInstance(false, UploadFinished);
            

            new Action(() =>
            {
                new Synchronizer(this, op).uploadFieldDataWorker(userNr, projectID);
            }).BeginInvoke(null, null);

            return op;
        }

        public event AsyncOperationFinishedHandler UploadFinished;


        public AsyncOperationInstance downloadData(IList<ISerializableObject> selection)
        {
            var op = new AsyncOperationInstance(false,DownloadFinished);
            

            new Action(() =>
            {
                new Synchronizer(this, op).downloadFieldDataWorker(selection);
            }).BeginInvoke(null, null);

            return op;
        }

        public event AsyncOperationFinishedHandler DownloadFinished;


        public AsyncOperationInstance<IList<ISerializableObject>> startSearch(SearchSpecification search, int currentProjectID)
        {
            var op = new AsyncOperationInstance<IList<ISerializableObject>>(false, SearchFinished);


            new Action(() =>
            {
                new Search(this, op).executeSearch(search, currentProjectID);
            }).BeginInvoke(null, null);

            return op;
        }

        public event AsyncOperationFinishedHandler<IList<ISerializableObject>> SearchFinished;
    }
}
