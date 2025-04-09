using System;
using Nine;

namespace Garawell.Managers.Events
{
    public class RvArgs : EventArgs
    {
        public RvType RvType;
        public int Id;

        public RvArgs(RvType rvType, int id)
        {
            this.RvType = rvType;
            this.Id = id;
        }
    }
}

