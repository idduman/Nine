using Garawell.Managers;
using UnityEngine;

namespace Nine
{
    public class GridLockRv : GridLock
    {
        [SerializeField] private int _id;

        public int ID => _id;
    }
}