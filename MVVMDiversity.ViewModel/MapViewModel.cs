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

using GalaSoft.MvvmLight;
using Microsoft.Practices.Unity;
using MVVMDiversity.Interface;
using MVVMDiversity.Messages;
using System.ComponentModel;
using MVVMDiversity.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using log4net;
using System;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;

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
    public class MapViewModel : PageViewModel, IDataErrorInfo
    {

        private const int GOOGLE_MAX_DIMENSION = 640;
        ILog _Log = LogManager.GetLogger(typeof(MapViewModel));

        IMapView _view; 
        [OptionalDependency]
        public IMapView View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != value)
                    _view = value;                
            }
        }

        [Dependency]
        public IMapService Maps { get; set; }

        #region Properties

        /// <summary>
        /// The <see cref="Latitude" /> property's name.
        /// </summary>
        public const string LatitudePropertyName = "Latitude";

        private double _latitude = 0d;

        /// <summary>
        /// Gets the Latitude property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Latitude
        {
            get
            {
                return _latitude;
            }

            set
            {
                if (_latitude == value)
                {
                    return;
                }

                var oldValue = _latitude;
                _latitude = value;               

                // Verify Property Exists
                VerifyPropertyName(LatitudePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(LatitudePropertyName);            
            }
        }

        /// <summary>
        /// The <see cref="Longitude" /> property's name.
        /// </summary>
        public const string LongitudePropertyName = "Longitude";

        private double _longitude = 0d;

        /// <summary>
        /// Gets the Longitude property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Longitude
        {
            get
            {
                return _longitude;
            }

            set
            {
                if (_longitude == value)
                {
                    return;
                }

                var oldValue = _longitude;
                _longitude = value;
                
                // Verify Property Exists
                VerifyPropertyName(LongitudePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(LongitudePropertyName);               
            }
        }

        /// <summary>
        /// The <see cref="MapName" /> property's name.
        /// </summary>
        public const string MapNamePropertyName = "MapName";      

        /// <summary>
        /// Gets the MapName property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MapName
        {
            get
            {
                return _mapInfo.Name;
            }

            set
            {
                if (_mapInfo.Name == value)
                {
                    return;
                }
                _mapInfo.Name = value;               

                // Verify Property Exists
                VerifyPropertyName(MapNamePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(MapNamePropertyName);

                updateCanSave();
            }
        }

        

        /// <summary>
        /// The <see cref="MapDescription" /> property's name.
        /// </summary>
        public const string MapDescriptionPropertyName = "MapDescription";       

        /// <summary>
        /// Gets the MapDescription property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MapDescription
        {
            get
            {
                return _mapInfo.Description;
            }

            set
            {
                if (_mapInfo.Description == value)
                {
                    return;
                }                
                _mapInfo.Description = value;
                
                // Verify Property Exists
                VerifyPropertyName(MapDescriptionPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(MapDescriptionPropertyName);

                updateCanSave();
            }
        }

        /// <summary>
        /// The <see cref="UseDeviceSize" /> property's name.
        /// </summary>
        public const string UseDeviceSizePropertyName = "UseDeviceSize";

        private bool _devSize = true;

        /// <summary>
        /// Gets the UseDeviceSize property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool UseDeviceSize
        {
            get
            {
                return _devSize;
            }

            set
            {
                if (_devSize == value)
                {
                    return;
                }                
                _devSize = value;

                // Verify Property Exists
                VerifyPropertyName(UseDeviceSizePropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(UseDeviceSizePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DeviceHeight" /> property's name.
        /// </summary>
        public const string DeviceHeightPropertyName = "DeviceHeight";

        private int _dHeight = 0;

        /// <summary>
        /// Gets the DeviceHeight property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int DeviceHeight
        {
            get
            {
                return _dHeight;
            }

            private set
            {
                if (_dHeight == value)
                {
                    return;
                }

                var oldValue = _dHeight;
                _dHeight = value;

                // Verify Property Exists
                VerifyPropertyName(DeviceHeightPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(DeviceHeightPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DeviceWidth" /> property's name.
        /// </summary>
        public const string DeviceWidthPropertyName = "DeviceWidth";

        private int _dWidth = 0;

        /// <summary>
        /// Gets the DeviceWidth property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int DeviceWidth
        {
            get
            {
                return _dWidth;
            }

            private set
            {
                if (_dWidth == value)
                {
                    return;
                }

                var oldValue = _dWidth;
                _dWidth = value;               

                // Verify Property Exists
                VerifyPropertyName(DeviceWidthPropertyName);

                // Update bindings, no broadcast
                RaisePropertyChanged(DeviceWidthPropertyName);
            }
        }
        
        public ICommand FromMap { get; private set; }
        //public ICommand ToMap { get; private set; }

        #endregion

        MapInfo _mapInfo;


        /// <summary>
        /// Initializes a new instance of the MapViewModel class.
        /// </summary>
        public MapViewModel()
            : base("Map_Next", "Map_Previous", "Map_Title", "Map_Description")
        {
            NextPage = Page.Actions;
            PreviousPage = Page.Actions;
            CanNavigateBack = true;
            CanNavigateNext = false;
            _mapInfo = new MapInfo();

            FromMap = new RelayCommand(() =>
                {
                    updateFromMap();
                });           


            MessengerInstance.Register<Settings>(this, (msg) =>
                {
                    updateFromSettings(msg.Content);
                });
            MessengerInstance.Send<SettingsRequest>(new SettingsRequest());
        }

        private void updateFromMap()
        {
            if (View != null)
            {
                var mapInfo = View.getMapInfo();
                mapInfo.Name = MapName;
                mapInfo.Description = MapDescription;

                _mapInfo = mapInfo;

                Latitude = _mapInfo.SWLat;
                Longitude = _mapInfo.SWLong;
            }
        }

        private void updateFromSettings(Model.DiversityUserOptions options)
        {
            UseDeviceSize = options.UseDeviceDimensions;
            DeviceHeight = options.ScreenHeight;
            DeviceWidth = options.ScreenWidth;
        }

        protected override bool OnNavigateNext()
        {
            saveMap();

            MapDescription = "";
            MapName = "";

            return false;
        }

        private void saveMap()
        {
            updateFromMap();
            if (View != null)
            {
                var url = View.getMapURL(
                    (DeviceHeight < GOOGLE_MAX_DIMENSION) ? DeviceHeight : GOOGLE_MAX_DIMENSION,
                    (DeviceWidth < GOOGLE_MAX_DIMENSION) ? DeviceWidth : GOOGLE_MAX_DIMENSION
                    );
                if (Maps != null)
                {
                    IsBusy = true;
                    new Action(() =>
                    {
                        Maps.saveMap(_mapInfo, url, (info) =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () =>
                                {
                                    IsBusy = false;
                                    MessengerInstance.Send<DialogMessage>(new DialogMessage("Map_Saved", null));
                                });
                        });
                    }).BeginInvoke(null, null);
                }
                else
                    _Log.Error("MapService N/A");
            }
        }

        private void updateCanSave()
        {
            CanNavigateNext = !formInError();
        }

        private bool formInError()
        {
            return Error != null;
        }



        private string _err;
        public string Error
        {
            get 
            {
                return _mapInfo.Error;
            }
        }

        public string this[string columnName]
        {
            get 
            {
                switch (columnName)
                {
                    case MapNamePropertyName:
                        return _mapInfo["Name"];
                    default:
                        return null;
                        
                }
            }
        }        
    }
}