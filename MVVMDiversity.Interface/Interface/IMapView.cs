using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IMapView
    {
        void showOverlay(int height, int width);
        MapInfo getMapInfo();
        string getMapURL(int height, int width);        
    }
}
