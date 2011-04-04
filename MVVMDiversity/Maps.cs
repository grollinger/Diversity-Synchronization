using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;

namespace MVVMDiversity.DesignServices
{
    public class Maps : IMapService
    {
        public void saveMap(Model.MapInfo metadata, string url, Action<Model.MapInfo> finishedCallback)
        {
            if (finishedCallback != null)
                finishedCallback(metadata);
        }
    }
}
