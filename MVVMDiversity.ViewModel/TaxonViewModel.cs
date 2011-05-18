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

using GalaSoft.MvvmLight;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using MVVMDiversity.Model;
using System;
using MVVMDiversity.Messages;
using log4net;
using GalaSoft.MvvmLight.Messaging;
using System.Collections;
using System.Linq;
using GalaSoft.MvvmLight.Threading;

namespace MVVMDiversity.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class TaxonViewModel : ViewModelBase
    {
        private ILog _Log = LogManager.GetLogger(typeof(TaxonViewModel));

        private ITaxonListService _taxonListSvc = null;
        [Dependency]
        public ITaxonListService TaxonListSvc 
        {
            get
            {
                return _taxonListSvc;
            }
            set
            {
                if (_taxonListSvc != value)
                {
                    _taxonListSvc = value;

                    updateTaxonLists();
                }
            }
        }

        private bool _expectClose = false;

        

        /// <summary>
        /// The <see cref="TaxonLists" /> property's name.
        /// </summary>
        public const string TaxonListsPropertyName = "TaxonLists";

        private IList<TaxonList> _taxonLists = null;

        /// <summary>
        /// Gets the TaxonLists property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IList<TaxonList> TaxonLists
        {
            get
            {
                return _taxonLists;
            }

            set
            {
                if (_taxonLists == value)
                {
                    return;
                }

                var oldValue = _taxonLists;
                _taxonLists = value;

                // Verify Property Exists
                VerifyPropertyName(TaxonListsPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(TaxonListsPropertyName);

            }
        }

        public ICommand DownloadTaxa { get; private set; }

        public void OnClose()
        {
            if (!_expectClose)
                sendLists(new List<TaxonList>());
            else
                _expectClose = false;
        }

      

        /// <summary>
        /// Initializes a new instance of the TaxonViewModel class.
        /// </summary>
        public TaxonViewModel(IMessenger msngr)
        {
            MessengerInstance = msngr;

            MessengerInstance.Register<ConnectionStateChanged>(this,
                (msg) =>
                {
                    if((msg.Content & ConnectionState.ConnectedToRepTax) == ConnectionState.ConnectedToRepTax)
                        updateTaxonLists();
                });

            DownloadTaxa = new RelayCommand<IList>(
                (selection) =>
                {
                    if (selection != null)
                    {
                        var typedSelection = from object list in selection
                                             where list is TaxonList
                                             select list as TaxonList;
                        IList<TaxonList> finalList = new List<TaxonList>(typedSelection);
                        _expectClose = true;
                        sendLists(finalList);
                    }                   
                });           
        }        

        private void updateTaxonLists()
        {
            if (TaxonListSvc != null)
            {
                new Action(() =>
                {
                    var taxa = TaxonListSvc.getAvailableTaxonLists();
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        TaxonLists = taxa;
                    });
                }).BeginInvoke(null, null);
            }
            else
                _Log.Info("TaxonListService N/A");

        }

        private void sendLists(IList<TaxonList> list)
        {
            MessengerInstance.Send<SelectedTaxonLists>(new SelectedTaxonLists(list));
        }
    }
}