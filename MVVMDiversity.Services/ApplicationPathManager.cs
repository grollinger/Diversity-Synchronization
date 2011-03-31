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
using System.IO;

namespace MVVMDiversity.Services
{
    public static class ApplicationPathManager
    {
        private const string APP_FOLDER = "Diversity Mobile";
        private const string TRANSACTION_FOLDER = "Transactions";
        private const string SETTINGS_FOLDER = "Settings";
        private const string LANGUAGE_FOLDER = "Languages";
        private const string DEFAULTS_FOLDER = "Defaults";
        private const string PICTURES_FOLDER = "Pictures";
        private const string MAPS_FOLDER = "Maps";
        private const string ICONS_FOLDER = "Icons";

        public const string MOBILEDB_FILE = "MobileDB.sdf";
        public const string TAXONDB_FILE = "TaxonNames.sdf";
        public const string USEROPTIONS_FILE = "useroptions.xml";
        private const string CONNECTIONPROFILES_FILE = "ConnectionProfiles.xml";

        

        static ApplicationPathManager()
        {
            createDirectories();
        }


        private static void createDirectories()
        {            
            createIfNotExists(ApplicationFolder.ApplicationData);
            
            createIfNotExists(ApplicationFolder.Settings);
            
            createIfNotExists(ApplicationFolder.Pictures);
            
            createIfNotExists(ApplicationFolder.Maps);
        }

        private static void createIfNotExists(ApplicationFolder f)
        {
            var path = getFolderPath(f);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string getFilePath(ApplicationFile f)
        {
            switch (f)
            {
                case ApplicationFile.EmptyDB:
                    return string.Format(@"{0}\{1}", getFolderPath(ApplicationFolder.Defaults), MOBILEDB_FILE);
                case ApplicationFile.EmptyTaxonDB:
                    return string.Format(@"{0}\{1}", getFolderPath(ApplicationFolder.Defaults), TAXONDB_FILE);
                case ApplicationFile.ConnectionProfiles:
                    return string.Format(@"{0}\{1}", getFolderPath(ApplicationFolder.Defaults), CONNECTIONPROFILES_FILE);
                default:
                    throw new ArgumentException("Unkown FileType");
            }
        }

        #region Directories


        /// <summary>
        /// </summary>
        /// <param name="f"></param>
        /// <returns>The requested Application Path WITHOUT a slash at the end</returns>
        public static string getFolderPath(ApplicationFolder f)
        {
            switch (f)
            {
                case ApplicationFolder.ApplicationData:
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + APP_FOLDER;
                    
                case ApplicationFolder.Settings:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.ApplicationData), SETTINGS_FOLDER);
                    
                case ApplicationFolder.Application:
                    return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'); ;
                    
                case ApplicationFolder.Language:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.Application), LANGUAGE_FOLDER);
                    
                case ApplicationFolder.Sessions:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.ApplicationData), TRANSACTION_FOLDER);
                                    
                case ApplicationFolder.Pictures:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.Settings), PICTURES_FOLDER);
                    
                case ApplicationFolder.Defaults:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.Application), DEFAULTS_FOLDER);
                    
                case ApplicationFolder.Maps:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.Settings), MAPS_FOLDER);
                    
                case ApplicationFolder.Icons:
                    return String.Format("{0}\\{1}", getFolderPath(ApplicationFolder.Application), ICONS_FOLDER);
                    
                default:
                    throw new ArgumentException("Unknown FolderType!");
                    
            }
        }
        #endregion
    }
}
