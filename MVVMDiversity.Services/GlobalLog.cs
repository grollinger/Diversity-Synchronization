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
using log4net;
using log4net.Appender;
using System.IO;

namespace MVVMDiversity.Services
{
    static class GlobalLog
    {
        public static ILog Global { get; private set; }
        static GlobalLog ()
	    {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ApplicationPathManager.getFolderPath(ApplicationFolder.Application) + "\\log4net.config"));

            var globalAppender = getFileAppender("GlobalLogFileAppender");
            if (globalAppender != null)
            {
                globalAppender.File = ApplicationPathManager.getFolderPath(ApplicationFolder.ApplicationData) + @"\log.txt";
                globalAppender.ActivateOptions();
            }

            Global = log4net.LogManager.GetLogger("GlobalLog");
            Global.Info("Logging initialized");
	    }

        public static FileAppender getFileAppender(string name)
        {
            var appenders = (from appender in log4net.LogManager.GetRepository().GetAppenders()
                             where appender.Name == name && appender is log4net.Appender.FileAppender
                             select appender as log4net.Appender.FileAppender);
            return (appenders.Count() > 0) ? appenders.First() : null;
        }
    }
}
