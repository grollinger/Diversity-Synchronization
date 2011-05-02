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
using System.ComponentModel;
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


        public BackgroundOperation loadDefinitions(Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            new Action<BackgroundOperation>(_defLoader.loadCollectionDefinitions).BeginInvoke(progress,(res) =>
                {
                    if( finishedCallback != null)
                        finishedCallback();
                }, null);
            return progress;
        }

        public BackgroundOperation loadTaxonLists(IEnumerable<Model.TaxonList> taxa, Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            progress.ProgressDescriptionID = "Services_Definitions_LoadingTaxa";
            new Action(()=>
            {
                _taxLoader.startTaxonDownload(taxa,progress);
            }).BeginInvoke(
            (res) => 
            {
                if (finishedCallback != null)
                    finishedCallback();
            },null);

            return progress;
        }

        public BackgroundOperation loadProperties(Action finishedCallback)
        {
            var progress = BackgroundOperation.newUninterruptable();
            new Action<BackgroundOperation>(_propLoader.updateProperties).BeginInvoke(progress, (res) =>
            {
                if (finishedCallback != null)
                    finishedCallback();
            }, null);
            return progress;
        }
    }
}
