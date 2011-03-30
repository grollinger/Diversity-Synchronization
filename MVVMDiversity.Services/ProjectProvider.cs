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
