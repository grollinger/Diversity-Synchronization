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
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using log4net;
using System.Data;
using MVVMDiversity.Model;

namespace MVVMDiversity.Services
{
    public class ProjectProvider : IProjectService
    {
        [Dependency]
        public IConnectionProvider Connections { get; set; }

        ILog _Log = LogManager.GetLogger(typeof(ProjectProvider));

        public IList<Model.Project> getProjectsForUser(string loginName)
        {
             IDbConnection repositoryConnection = null;
            IList<Project> result = new List<Project>();

            if (Connections != null)
            {
                if(Connections.Repository != null)
                    repositoryConnection = Connections.Repository.CreateConnection();
                else
                    _Log.Info("Repository Serializer not available");
            }
            else
                _Log.Error("ConnectionsProvider not available");



            if (repositoryConnection != null)
            {
                var projectInfoCmd = repositoryConnection.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM ProjectProxy");
                sb.Append(" WHERE ProjectID IN");
                sb.Append(" (Select ProjectID From ProjectUser Where LoginName='" + loginName +"')");
                projectInfoCmd.CommandText = sb.ToString();
                //projectInfoCmd.CommandText = "SELECT ProjectID,Project,ProjectTitle FROM ProjectList";
                //projectInfoCmd.CommandText = "SELECT ProjectID,Project FROM ProjectList";

                try
                {                   
                    repositoryConnection.Open();

                    using (var projectReader = projectInfoCmd.ExecuteReader())
                    {
                        int projectIDOrdinal = projectReader.GetOrdinal("ProjectID"),
                            projectOrdinal = projectReader.GetOrdinal("Project");
                        //projectTitleOrdinal = projectReader.GetOrdinal("ProjectTitle");
                        
                        while (projectReader.Read())
                        {
                            string projectTitle = /*projectReader.IsDBNull(projectTitleOrdinal) ?*/ projectReader.GetString(projectOrdinal);// : projectReader.GetString(projectTitleOrdinal);
                            int projectID = projectReader.GetInt32(projectIDOrdinal);
                            result.Add(new Project() { ID = projectID, Title = projectTitle });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Log.ErrorFormat("Exception reading Projects: [{0}]", ex);
                }
                finally
                {                    
                    repositoryConnection.Close();
                }                
            }
            return result;
        }
        

           
    }
}
