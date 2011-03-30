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

        [Dependency]
        public IDefinitionsService DefinitionsSvc { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the TaxonViewModel class.
        /// </summary>
        public TaxonViewModel(IMessenger msngr)
        {
            MessengerInstance = msngr;

            DownloadTaxa = new RelayCommand<IList>(
                (selection) =>
                {
                    if (selection != null)
                    {
                        var typedSelection = from object list in selection
                                             where list is TaxonList
                                             select list as TaxonList;
                        IList<TaxonList> finalList = new List<TaxonList>(typedSelection);
                        if (DefinitionsSvc != null)
                        {
                            var progress = DefinitionsSvc.loadTaxonLists(finalList,
                                () =>
                                {
                                    MessengerInstance.Send<HideProgress>(new HideProgress());
                                    MessengerInstance.Send<SyncStepFinished>(SyncState.TaxaDownloaded);
                                });
                            MessengerInstance.Send<ShowProgress>(progress);
                        }
                        else
                            _Log.Error("DefinitionsService N/A");
                    }                   
                });

            MessengerInstance.Register<ConnectionStateChanged>(this,
                (msg) =>
                {
                    if ((msg.Content & ConnectionState.ConnectedToRepTax) == ConnectionState.ConnectedToRepTax)
                        updateTaxonLists();
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

        
    }
}