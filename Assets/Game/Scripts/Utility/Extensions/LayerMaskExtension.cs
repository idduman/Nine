using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell.Extensions
{
    public static class LayerMaskExtension
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((mask & (1 << layer)) != 0);
        }
    }
}

