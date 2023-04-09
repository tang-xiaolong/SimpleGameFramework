using System;
using System.Collections;
using System.Collections.Generic;

namespace FutureEventModule
{
    public class FutureEventList : IComparable, IEnumerable<FutureEventData>, IDisposable
    {
        private bool _disposed = false;
        public long Time;
        public List<FutureEventData> FutureEventDataList;

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            return CompareTo((FutureEventList)obj);
        }

        public int CompareTo(FutureEventList obj)
        {
            return Time.CompareTo(obj.Time);
        }

        IEnumerator<FutureEventData> IEnumerable<FutureEventData>.GetEnumerator()
        {
            return FutureEventDataList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return FutureEventDataList.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                FutureEventDataList.Clear();
                FutureEventDataList = null;
            }
            _disposed = true;
        }
        
        ~FutureEventList()
        {
            Dispose(false);
        }
    }
}