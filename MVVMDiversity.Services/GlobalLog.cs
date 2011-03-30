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
