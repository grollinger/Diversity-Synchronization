//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
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
using System.Runtime.InteropServices;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer
{
    public abstract class Cleaner : IDisposable
    {
        public const String DEFAULT = "DEFAULT"; 

        private ICleanable _cleanable;

        public ICleanable Cleanable
        {
            get { return _cleanable; }
            set { _cleanable = value; }
        }

        abstract internal void Cleanup();
        

        protected void StartClean()
        {
            if (_cleanable == null)
            {
                throw new NullReferenceException("cleanable not set");
            }
            _cleanable.Cleanup();
        }

        public virtual void Dispose()
        {
            _cleanable = null;
        }

        internal static Cleaner CreateCleaner(String id)
        {
            return new DefaultCleaner();
        }
    }

    internal class DefaultCleaner : Cleaner
    {
        internal override void Cleanup()
        {
            base.StartClean();
        }
    }

    //internal class ThreadedCleaner : Cleaner
    //{
    //    private System.Threading.Thread _thread;


    //    internal sealed override void Cleanup()
    //    {
    //        if (_thread == null)
    //        {
    //            _thread = new System.Threading.Thread(new System.Threading.ThreadStart(CleanupThread));
    //            _thread.Start();
    //        }
    //    }

    //    private void CleanupThread()
    //    {
    //        while (true)
    //        {
    //            base.StartClean();
    //            System.Threading.Thread.Sleep(10000);
    //        }
    //    }

    //    public override void Dispose()
    //    {
    //        base.Dispose();
    //        _thread.Abort();
    //    }
    //}

    //internal class DOT_NET_CF_DiscSpaceThreadCleaner : Cleaner
    //{
    //    private System.Threading.Thread _thread;


    //    internal sealed override void Cleanup() 
    //    {
    //        if (_thread == null)
    //        {
    //            _thread = new System.Threading.Thread(new System.Threading.ThreadStart(CleanupThread));
    //            _thread.Start();
    //        }
    //    }

    //    [DllImport("coredll", SetLastError = true)]
    //    private static extern bool GetDiskFreeSpaceEx(string directoryName, ref long longfreeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);

    //    private void CleanupThread()
    //    {
    //        while (true)
    //        {
    //            long availFree = 0, total = 0, totalFree = 0;
    //            if (!GetDiskFreeSpaceEx(@"\Windows", ref availFree, ref total, ref totalFree))
    //            {
    //                base.StartClean();
    //            }

    //            if (availFree < 1024 * 1024 * 8)
    //            {
    //                base.StartClean();
    //            }
    //            System.Threading.Thread.Sleep(10000);
    //        }
    //    }

    //    public override void Dispose()
    //    {
    //        base.Dispose();
    //        _thread.Abort();
    //    }
    //}
}
