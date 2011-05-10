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
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using MVVMDiversity.Model;

namespace MVVMDiversity.Services
{
    public partial class DefinitionsService : IDefinitionsService
    {
        [Dependency]
        public IConnectionProvider Connections { get; set; }

        [Dependency]
        public IUserOptionsService Settings { get; set; }

        [Dependency]
        public IUserProfileService Profiles { get; set; }  

        DefinitionLoader _defLoader;
        PropertyLoader _propLoader;
        TaxonLoader _taxLoader;

        public DefinitionsService()
        {
            _defLoader = new DefinitionLoader(this);
            _propLoader = new PropertyLoader(this);
            _taxLoader = new TaxonLoader(this);
        }       
       

        public AsyncOperationInstance loadTaxonLists(IEnumerable<Model.TaxonList> taxa)
        {
            var op = new AsyncOperationInstance(false, TaxaLoaded);
            op.StatusDescription = "Services_Definitions_LoadingTaxa";
            new Action(()=>
            {
                _taxLoader.startTaxonDownload(taxa,op);
            }).BeginInvoke(null,null);

            return op;
        }

        public AsyncOperationInstance loadProperties()
        {
            var op = new AsyncOperationInstance(false, PropertiesLoaded);
            new Action<AsyncOperationInstance>(_propLoader.updateProperties).BeginInvoke(op, null, null);
            return op;
        }

        public AsyncOperationInstance loadDefinitions()
        {
            var op = new AsyncOperationInstance(false, DefinitionsLoaded);
            new Action<AsyncOperationInstance>(_defLoader.loadCollectionDefinitions).BeginInvoke(op,null, null);
            return op;
        }

        public event AsyncOperationFinishedHandler DefinitionsLoaded;      

        public event AsyncOperationFinishedHandler TaxaLoaded;       

        public event AsyncOperationFinishedHandler PropertiesLoaded;
    }
}
