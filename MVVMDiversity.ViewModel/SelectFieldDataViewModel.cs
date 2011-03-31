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

using MVVMDiversity.Model;
using System.Collections.Generic;
using MVVMDiversity.Interface;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using log4net;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using MVVMDiversity.Messages;
using System.Collections.ObjectModel;
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
    public class SelectFieldDataViewModel : PageViewModel
    {
        ILog _Log = LogManager.GetLogger(typeof(SelectFieldDataViewModel));

        #region Dependencies

        private IFieldDataService _fd;
        [Dependency]
        public IFieldDataService FieldData 
        {
            get
            {
                return _fd;
            }
            set
            {
                if (value != null)
                {
                    _fd = value;

                    VerifyPropertyName(SearchTypesPropertyName);

                    RaisePropertyChanged(SearchTypesPropertyName);
                }
            }
        }

        [Dependency]
        public IUserProfileService UserProfileSvc { get; set; }

        [Dependency]
        public IISOViewModelStore ISOStore { get; set; }
        #endregion   
     
        BackgroundOperation _progress;

        /// <summary>
        /// Initializes a new instance of the SelectFieldDataViewModel class.
        /// </summary>
        public SelectFieldDataViewModel()
            : base("SelectFD_Next","SelectFD_Previous","SelectFD_Title","SelectFD_Description")
        {
            NextPage = Messages.Page.FinalSelection;
            PreviousPage = Messages.Page.Actions;

            CanNavigateBack = true;
            CanNavigateNext = true;

            

            QueryDatabase = new RelayCommand(() =>
            {
                if (FieldData != null)
                {
                    if (UserProfileSvc != null)
                    {
                        IsBusy = true;
                        _progress = FieldData.executeSearch(ConfiguredSearch, UserProfileSvc.ProjectID,
                            (result) =>
                            {
                                List<IISOViewModel> selectionList = buildVMList(result);
                                DispatcherHelper.CheckBeginInvokeOnUI(
                                    () =>
                                    {      

                                        _queryResult = selectionList;                                     

                                        QueryResultTree = new TreeViewModel(ISOStore);

                                        queryResultChanged();

                                        MessengerInstance.Send<HideProgress>(new HideProgress());
                                        IsBusy = false;
                                    });
                            });
                        MessengerInstance.Send<ShowProgress>(_progress);
                    }
                    else
                        _Log.Error("UserProfileService N/A");
                }
                else
                    _Log.Error("FieldDataService N/A");
            });

            AddToSelection = new RelayCommand<IList>((selection) =>
            {
                if(SelectionTree == null)
                    SelectionTree = new TreeViewModel(ISOStore);

                if (_queryResult != null && selection != null)
                {
                    var typedSelection = Enumerable.Cast<IISOViewModel>(selection);                   

                    foreach (var generator in typedSelection)
                    {
                        if (!_selection.Contains(generator)) 
                            _selection.Add(generator);                        
                    }
                    if (typedSelection.Count() > 0)
                    {
                        RaiseSelectionChanged();                       
                    }
                }

            });

            RemoveFromSelection = new RelayCommand<IList>((selection) =>
                {
                    if(_queryResult != null && selection != null)
                    {
                        var typedSelection = Enumerable.Cast<IISOViewModel>(selection);                        

                        foreach (var generator in typedSelection)
                        {                            
                            _selection.Remove(generator);                            
                        }
                        if (selection.Count > 0)
                        {
                            RaiseSelectionChanged();                            
                        }
                    }
                });
        }

        private List<IISOViewModel> buildVMList(IList<ISerializableObject> result)
        {
            

            List<IISOViewModel> list = new List<IISOViewModel>(result.Count());
            ProgressInterval localProgress = null;
            if (_progress != null)
            {
                _progress.ProgressDescriptionID = "SelectFD_Status_FillingResults";
                _progress.Progress = 0;
                _progress.IsProgressIndeterminate = false;
                localProgress = new ProgressInterval(_progress,100f,result.Count());
            }

            var conversionQuery = from obj in result
                                  select ISOStore.addOrRetrieveVMForISO(obj);

            foreach(var line in conversionQuery)
            {
                list.Add(line);
                if (localProgress != null)
                    if (localProgress.IsCancelRequested)
                        break;
                    else
                        localProgress.advance();
            }
            return list;
        }

        private void queryResultChanged()
        {
            // Verify Property Exists
            VerifyPropertyName(QueryResultPropertyName);

            // Update bindings, no broadcast
            RaisePropertyChanged(QueryResultPropertyName);
        }

        private void RaiseSelectionChanged()
        {
            VerifyPropertyName(SelectionPropertyName);
            RaisePropertyChanged(SelectionPropertyName);
        }

        #region Properties
        public ICommand QueryDatabase { get; private set; }      

        public ICommand AddToSelection { get; private set; }

        public ICommand RemoveFromSelection { get; private set; }

        /// <summary>
        /// The <see cref="SearchTypes" /> property's name.
        /// </summary>
        public const string SearchTypesPropertyName = "SearchTypes";       

        /// <summary>
        /// Gets the SearchTypes property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<SearchSpecification> SearchTypes
        {
            get
            {
                return (FieldData != null) ? FieldData.SearchTypes : null;
            }            
        }

        /// <summary>
        /// The <see cref="ConfiguredSearch" /> property's name.
        /// </summary>
        public const string ConfiguredSearchPropertyName = "ConfiguredSearch";

        private SearchSpecification _configuredSearch = null;

        /// <summary>
        /// Gets the ConfiguredSearch property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public SearchSpecification ConfiguredSearch
        {
            get
            {
                return _configuredSearch;
            }

            set
            {
                if (_configuredSearch == value)
                {
                    return;
                }

                var oldValue = _configuredSearch;
                _configuredSearch = value;                

                // Verify Property Exists
                VerifyPropertyName(ConfiguredSearchPropertyName);
                

                // Update bindings, no broadcast
                RaisePropertyChanged(ConfiguredSearchPropertyName);
                
            }
        }


        /// <summary>
        /// The <see cref="QueryResult" /> property's name.
        /// </summary>
        public const string QueryResultPropertyName = "QueryResult";

        private List<IISOViewModel> _queryResult = null;

        /// <summary>
        /// Gets the QueryResult property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<IISOViewModel> QueryResult
        {
            get
            {
                if (_queryResult != null)
                    foreach (var item in _queryResult)
                        yield return item;
                else
                    yield return null;
            }            
        }

        /// <summary>
        /// The <see cref="QueryResultTree" /> property's name.
        /// </summary>
        public const string QueryResultTreePropertyName = "QueryResultTree";

        private ITreeViewModel _qrTree = null;

        /// <summary>
        /// Gets the QueryResultTree property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ITreeViewModel QueryResultTree
        {
            get
            {
                return _qrTree;
            }

            set
            {
                if (_qrTree == value)
                {
                    return;
                }

                var oldValue = _qrTree;
                _qrTree = value;


                // Verify Property Exists
                VerifyPropertyName(QueryResultTreePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(QueryResultTreePropertyName);

            }
        }

        /// <summary>
        /// The <see cref="Selection" /> property's name.
        /// </summary>
        public const string SelectionPropertyName = "Selection";

        private Collection<IISOViewModel> _selection = new Collection<IISOViewModel>();

        /// <summary>
        /// Gets the Selection property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IEnumerable<IISOViewModel> Selection
        {
            get
            {
                foreach (var gen in _selection)
                    yield return gen;
            }           
        }

        /// <summary>
        /// The <see cref="SelectionTree" /> property's name.
        /// </summary>
        public const string SelectionTreePropertyName = "SelectionTree";

        private ITreeViewModel _selectionTree = null;

        /// <summary>
        /// Gets the SelectionTree property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ITreeViewModel SelectionTree
        {
            get
            {
                return _selectionTree;
            }

            private set
            {
                if (_selectionTree == value)
                {
                    return;
                }

                var oldValue = _selectionTree;
                _selectionTree = value;                

                // Verify Property Exists
                VerifyPropertyName(SelectionTreePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectionTreePropertyName);                
            }
        }       

        #endregion

        public void QuerySelectionChanged(IEnumerable<IISOViewModel> added, IEnumerable<IISOViewModel> removed)
        {
            foreach (var i in added)
                QueryResultTree.addGenerator(i);
            foreach (var i in removed)
                QueryResultTree.removeGenerator(i);
        }

        public void SelectionChanged(IEnumerable<IISOViewModel> added, IEnumerable<IISOViewModel> removed)
        {
            foreach (var i in added)
                SelectionTree.addGenerator(i);
            foreach (var i in removed)
                SelectionTree.removeGenerator(i);
        }

        protected override bool OnNavigateNext()
        {
            MessengerInstance.Send<Messages.Selection>(_selection);

            return base.OnNavigateNext();
        }
        

        
    }
}