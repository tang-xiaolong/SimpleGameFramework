using System;

namespace FutureEventModule
{
    public class FutureEventData
    {
        public long Time;
        public Action Action;
        public Action EarlyRemoveCallback;
    }
}