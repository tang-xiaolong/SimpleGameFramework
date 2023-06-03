using System;

namespace DelayedTaskModule
{
    public class DelayedTaskData
    {
        public long Time;
        public string Token;
        public Action Action;
        public Action EarlyRemoveCallback;
    }
}