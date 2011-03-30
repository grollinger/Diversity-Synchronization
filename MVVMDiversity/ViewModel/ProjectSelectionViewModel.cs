using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using System.Collections.ObjectModel;
using MVVMDiversity.Messages;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

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
        [Dependency]
        public IProjectService ProjectProvider { get; set; }

        [Dependency]
        public IUserOptionsService UserOptions { get; set; }

        [Dependency]
        public IUserProfileService ProfileProvider { get; set; }

        protected override bool CanNavigateBack
        {
            get { return true; }
        }

        protected override bool CanNavigateNext
        {
            get { return Selection != null; }
        }

        public ObservableCollection<Project> Projects { get; private set; }

        public Project Selection { get; set; }

        /// <summary>
        /// Initializes a new instance of the ProjectSelectionViewModel class.
        /// </summary>
        public ProjectSelectionViewModel()
            : base("ProjectSelection_NextText", "ProjectSelection_PreviousText", "ProjectSelection_Title", "ProjectSelection_Description")
        {
            Projects = new ObservableCollection<Project>();
            

            if (IsInDesignMode)
            {
                Projects.Add(new Project() { ID = 1337, Title = "Sample Project", Description = "This is a sample" });
            }
            else
            {
                MessengerInstance.Register<ConnectionStateChanged>(this, (msg) =>
                {
                    fillProjects();
                });
            }
        }

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
                    if (projects != null)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Projects.Clear();
                                foreach (var project in projects)
                                    Projects.Add(project);
                            });
                    }
                }).BeginInvoke(null, null);
            }
        }

        protected override bool OnNavigateNext()
        {
            
            return base.OnNavigateNext();
        }       
    }       
}
