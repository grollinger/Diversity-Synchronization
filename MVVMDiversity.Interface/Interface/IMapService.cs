using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;

namespace MVVMDiversity.Interface
{
    public interface IMapService
    {
        void saveMap(MapInfo metadata, string url, Action<MapInfo> finishedCallback);
        //IList<MapInfo> availableMaps();
    }
}
