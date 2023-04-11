using System;

namespace DelayedTaskModule
{
    public class DelayedTaskData
    {
        public long Time;
        public Action Action;
        public Action EarlyRemoveCallback;
    }
}