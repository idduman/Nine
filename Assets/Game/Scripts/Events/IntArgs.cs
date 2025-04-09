using System;

namespace Garawell.Managers.Events
{
    public class IntArgs : EventArgs
    {
        public int value;

        public IntArgs(int v)
        {
            this.value = v;
        }
    }
}

