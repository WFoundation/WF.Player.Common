using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WF.Player.Providers
{
    public class SyncEventArgs : EventArgs
    {
        public Uri ProviderFilePath { get; private set; }

        public float FileSyncProgress { get; private set; }

        public float TotalSyncProgress { get; private set; }
    }
}
