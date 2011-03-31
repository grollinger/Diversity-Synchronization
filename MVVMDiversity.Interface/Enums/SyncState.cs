using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMDiversity.Model
{
    [Flags]
    public enum SyncState
    {
        None = 0x00,
        TaxaDownloaded = 0x01,
        PropertyNamesDownloaded = 0x02,
        FieldDataDownloaded = 0x04,
        FieldDataUploaded = 0x08
    }
}
