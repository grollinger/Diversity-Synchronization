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
using MVVMDiversity.Model;
using MVVMDiversity.Messages;

using System;
using System.Collections.Generic;
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
    public class ProjectSelectionViewModel : PageViewModel
    {
        private IProjectService _projectSvc;
        [Dependency]
        public IProjectService ProjectProvider 
        {
            get
            {
                return _projectSvc;
            }
            set
            {                
                if (_projectSvc != value)
                {
                    _projectSvc = value;
                    fillProjects();
                }
            }
        }

        private IUserOptionsService _userOptions;
        [Dependency]
        public IUserOptionsService UserOptions 
        {
            get
            {
                return _userOptions;
            }
            set
            {
                if (_userOptions != value)
                {
                    _userOptions = value;
                    fillProjects();
                }
            }
        }

        [Dependency]
        public IUserProfileService ProfileProvider { get; set; }

        [Dependency]
        public IDefinitionsService DefinitionsProvider { get; set; }        

        /// <summary>
        /// The <see cref="Projects" /> property's name.
        /// </summary>
        public const string ProjectsPropertyName = "Projects";

        private IList<Project> _Projects = null;

        /// <summary>
        /// Gets the Projects property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public IList<Project> Projects
        {
            get
            {
                return _Projects;
            }

            set
            {
                if (_Projects == value)
                {
                    return;
                }

                var oldValue = _Projects;
                _Projects = value;


                // Verify Property Exists
                VerifyPropertyName(ProjectsPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(ProjectsPropertyName);               
            }
        }

        /// <summary>
        /// The <see cref="Selection" /> property's name.
        /// </summary>
        public const string SelectionPropertyName = "Selection";

        private Project _selection = null;

        /// <summary>
        /// Gets the Selection property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Project Selection
        {
            get
            {
                return _selection;
            }

            set
            {
                if (_selection == value)
                {
                    return;
                }

                var oldValue = _selection;
                _selection = value;                

                // Verify Property Exists
                VerifyPropertyName(SelectionPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectionPropertyName);
                updateCanNavs();
            }
        }

        private void updateCanNavs()
        {
            CanNavigateNext = Selection != null;
            CanNavigateBack = true;
        }

        /// <summary>
        /// Initializes a new instance of the ProjectSelectionViewModel class.
        /// </summary>
        public ProjectSelectionViewModel()
            : base("ProjectSelection_NextText", "ProjectSelection_PreviousText", "ProjectSelection_Title", "ProjectSelection_Description")
        {
            PreviousPage = Page.Connections;
            NextPage = Page.Actions;

            if (IsInDesignMode)
            {
                Projects = new List<Project>(){new Project() { ID = 1337, Title = "Sample Project", Description = "This is a sample" }};
            }
            else
            {
                MessengerInstance.Register<ConnectionStateChanged>(this, (msg) =>
                {
                    fillProjects();
                });
            }
        }

        BackgroundOperation _p = null;

        private void fillProjects()
        {
            if (UserOptions != null && ProjectProvider != null)
            {
                var user = UserOptions.getOptions().Username;
                IList<Project> projects;
                new Action(() =>
                {
                    //May Take a while
                    projects = ProjectProvider.getProjectsForUser(user);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Projects = projects;
                    });
                    
                }).BeginInvoke(null, null);
            }
        }

        protected override bool OnNavigateNext()
        {
            if (DefinitionsProvider != null && Selection != null)
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    _p = DefinitionsProvider.loadDefinitions(continueNavigating);
                    MessengerInstance.Send<ShowProgress>(_p);                   
                }
                else
                {
                    ProfileProvider.ProjectID = Selection.ID;
                    IsBusy = false;
                }
            }


            return !IsBusy;
        }

        private void continueNavigating()
        {
            MessengerInstance.Send<HideProgress>(new HideProgress());
            if (NavigateNext != null)
                NavigateNext.Execute(null);
        }
    }       
}
