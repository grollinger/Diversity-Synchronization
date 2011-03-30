using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using log4net;
using Microsoft.Practices.Unity;
using GalaSoft.MvvmLight.Messaging;
using MVVMDiversity.Messages;

namespace MVVMDiversity.Services
{
    public class UserOptionsService : IUserOptionsService
    {

        [Dependency]
        public IMessenger MessengerInstance { get; set; }


        private string _optionsPath;

        private XmlSerializer _serializer;

        private ILog _Log;

        public UserOptionsService()
        {
            _optionsPath = ApplicationPathManager.getFolderPath(ApplicationFolder.ApplicationData) + '\\' + ApplicationPathManager.USEROPTIONS_FILE;
            _serializer = new XmlSerializer(typeof(DiversityUserOptions));
            _Log = log4net.LogManager.GetLogger(typeof(UserOptionsService));
        }

        private DiversityUserOptions _options;               

        public DiversityUserOptions getOptions()
        {
            if (_options == null)
            {
                if (File.Exists(_optionsPath))
                {
                    try
                    {
                        using (var xmlDoc = XmlReader.Create(new FileStream(_optionsPath, FileMode.Open)))
                        {
                            if (_serializer.CanDeserialize(xmlDoc))
                                _options = _serializer.Deserialize(xmlDoc) as DiversityUserOptions;
                        }
                    }
                    catch (Exception ex)
                    {
                        _Log.ErrorFormat("Couldn't load existing UserOptions: [{0}]", ex);
                    }
                }
                if (_options == null)
                    _options = new DiversityUserOptions();
            }
            return _options;
        }

        public void setOptions(DiversityUserOptions o)
        {
            if (o == null)
                return;

            _options = o;

            if (MessengerInstance != null)
                MessengerInstance.Send<SettingsChanged>(_options);

            var openMode = File.Exists(_optionsPath) ? FileMode.Truncate : FileMode.Create;
            try
            {
                using (var fs = new FileStream(_optionsPath, openMode))
                {
                    _serializer.Serialize(fs, _options);
                }
            }
            catch (Exception ex)
            {
                _Log.ErrorFormat("Couldn't save UserOptions: [{0}]", ex);
            }
        }
    }
}
