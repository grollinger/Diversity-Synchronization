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

using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using GalaSoft.MvvmLight.Messaging;
namespace MVVMDiversity.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocatorTemplate in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MVVMDiversity.ViewModel"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the Main property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:MVVMDiversity.ViewModel"
    /// DataContext="{Binding Source={x:Static vm:ViewModelLocatorTemplate.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class ViewModelLocator
    {
        private static IUnityContainer _iocContainer;
        private static MainViewModel _main;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator(IUnityContainer IOC)
        {

            _iocContainer = IOC;

           

            CreateMain();
            CreateConnections();
            CreateProjectSelection();
            CreateActions();
            CreateMap();
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearMain();
            ClearConnections();
            ClearProjectSelection();
            ClearActions();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static MainViewModel MainStatic
        {
            get
            {
                if (_main == null)
                {
                    CreateMain();
                }

                return _main;
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return MainStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Main property.
        /// </summary>
        public static void ClearMain()
        {
            _main.Cleanup();
            _main = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Main property.
        /// </summary>
        public static void CreateMain()
        {
            if (_main == null)
            {
                _main = _iocContainer.Resolve<MainViewModel>();
            }
        }

        

        private static ConnectionsViewModel _connectionsVM;

        /// <summary>
        /// Gets the Connections property.
        /// </summary>
        public static ConnectionsViewModel ConnectionsStatic
        {
            get
            {
                if (_connectionsVM == null)
                {
                    CreateConnections();
                }

                return _connectionsVM;
            }
        }

        /// <summary>
        /// Gets the Connections property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ConnectionsViewModel Connections
        {
            get
            {
                return ConnectionsStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Connections property.
        /// </summary>
        public static void ClearConnections()
        {
            _connectionsVM.Cleanup();
            _connectionsVM = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Connections property.
        /// </summary>
        public static void CreateConnections()
        {
            if (_connectionsVM == null)
            {
               _connectionsVM = _iocContainer.Resolve<ConnectionsViewModel>();
            }
        }


        private static ProjectSelectionViewModel _projectSelection;



        /// <summary>
        /// Gets the ProjectSelection property.
        /// </summary>
        public static ProjectSelectionViewModel ProjectSelectionStatic
        {
            get
            {
                if (_projectSelection == null)
                {
                    CreateProjectSelection();
                }

                return _projectSelection;
            }
        }

        /// <summary>
        /// Gets the ProjectSelection property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProjectSelectionViewModel ProjectSelection
        {
            get
            {
                return ProjectSelectionStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ProjectSelection property.
        /// </summary>
        public static void ClearProjectSelection()
        {
            _projectSelection.Cleanup();
            _projectSelection = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ProjectSelection property.
        /// </summary>
        public static void CreateProjectSelection()
        {
            if (_projectSelection == null)
            {
                _projectSelection = _iocContainer.Resolve<ProjectSelectionViewModel>();
            }
        }

        private static ActionsViewModel _actions;

        /// <summary>
        /// Gets the Actions property.
        /// </summary>
        public static ActionsViewModel ActionsStatic
        {
            get
            {
                if (_actions == null)
                {
                    CreateActions();
                }

                return _actions;
            }
        }

        /// <summary>
        /// Gets the Actions property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ActionsViewModel Actions
        {
            get
            {
                return ActionsStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Actions property.
        /// </summary>
        public static void ClearActions()
        {
            _actions.Cleanup();
            _actions = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Actions property.
        /// </summary>
        public static void CreateActions()
        {
            if (_actions == null)
            {
                _actions = _iocContainer.Resolve<ActionsViewModel>();
            }
        }

        private static SelectFieldDataViewModel _selectFieldData;

        /// <summary>
        /// Gets the SelectFieldData property.
        /// </summary>
        public static SelectFieldDataViewModel SelectFieldDataStatic
        {
            get
            {
                if (_selectFieldData == null)
                {
                    CreateSelectFieldData();
                }

                return _selectFieldData;
            }
        }

        /// <summary>
        /// Gets the SelectFieldData property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SelectFieldDataViewModel SelectFieldData
        {
            get
            {
                return SelectFieldDataStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SelectFieldData property.
        /// </summary>
        public static void ClearSelectFieldData()
        {
            _selectFieldData.Cleanup();
            _selectFieldData = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SelectFieldData property.
        /// </summary>
        public static void CreateSelectFieldData()
        {
            if (_selectFieldData == null)
            {
                _selectFieldData = _iocContainer.Resolve<SelectFieldDataViewModel>();
            }
            //Needs to be alive to receive Messages
            CreateSelection();
        }


        public OptionsViewModel Options { get { return _iocContainer.Resolve<OptionsViewModel>(); } }


        private static MapViewModel _mapVM;

        /// <summary>
        /// Gets the Map property.
        /// </summary>
        public static MapViewModel getMapVMforView(IMapView view)
        {            
            CreateMap();
          
            _mapVM.View = view;

            return _mapVM;            
        }

        private static void CreateMap()
        {
            if (_mapVM == null)
                _mapVM = _iocContainer.Resolve<MapViewModel>();
        }

        public static PageViewModel MapStatic { get {
            CreateMap();
            return _mapVM; } }
        
        private static TaxonViewModel _taxonVM;

        /// <summary>
        /// Gets the Taxon property.
        /// </summary>
        public static TaxonViewModel TaxonStatic
        {
            get
            {
                if (_taxonVM == null)
                {
                    CreateTaxon();
                }

                return _taxonVM;
            }
        }

        /// <summary>
        /// Gets the Taxon property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TaxonViewModel Taxon
        {
            get
            {
                return TaxonStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Taxon property.
        /// </summary>
        public static void ClearTaxon()
        {
            _taxonVM.Cleanup();
            _taxonVM = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Taxon property.
        /// </summary>
        public static void CreateTaxon()
        {
            if (_taxonVM == null)
            {
                _taxonVM = _iocContainer.Resolve<TaxonViewModel>();
            }
        }

        private static SelectionViewModel _selection;

        /// <summary>
        /// Gets the Selection property.
        /// </summary>
        public static SelectionViewModel SelectionStatic
        {
            get
            {
                if (_selection == null)
                {
                    CreateSelection();
                }

                return _selection;
            }
        }

        /// <summary>
        /// Gets the Selection property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SelectionViewModel Selection
        {
            get
            {
                return SelectionStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Selection property.
        /// </summary>
        public static void ClearSelection()
        {
            _selection.Cleanup();
            _selection = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Selection property.
        /// </summary>
        public static void CreateSelection()
        {
            if (_selection == null)
            {
                _selection = _iocContainer.Resolve<SelectionViewModel>();
            }
        }


        
    }
}