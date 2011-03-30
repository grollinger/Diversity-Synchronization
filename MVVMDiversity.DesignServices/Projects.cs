using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;

namespace MVVMDiversity.DesignServices
{
    public class Projects : IProjectService
    {
        public IList<Project> getProjectsForUser(string loginName)
        {
            return new List<Project>()
                {
                    new Project() { Description = "Description1", ID = 1, Title = "Project1"},
                    new Project() { Description = "Description2", ID = 2, Title = "Project2"},
                    new Project() { Description = "Description3", ID = 3, Title = "Project3"},
                };
        }
    }
}
