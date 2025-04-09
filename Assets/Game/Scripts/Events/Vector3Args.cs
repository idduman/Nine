using System;
using UnityEngine;

namespace Garawell.Managers.Events
{
    public class Vector3Args : EventArgs
    {
        public float x;
        public float y;
        public float z;

        private Vector3 toVector3;

        public Vector3Args()
        {

        }

        public Vector3Args(Vector3 vector3)
        {
            FromVector(vector3);
        }

        public Vector3Args(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void FromVector(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public void Reset()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vector3 ToVector()
        {
            toVector3.x = x;
            toVector3.y = y;
            toVector3.z = z;
            return toVector3;
        }
    }
}

