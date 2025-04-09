using UnityEngine;

namespace Garawell.Extensions
{
    public static class RigidbodyExtension
    {
        public static void RandomDash<T>(this Rigidbody rb, float minForce = 1, float maxForce = 1)
        {
            rb.AddForce(new Vector3(GetRandomValue(minForce, maxForce), GetRandomValue(minForce, maxForce), GetRandomValue(minForce, maxForce)));
        }

        private static float GetRandomValue(float minForce, float maxForce)
        {
            return Random.Range(minForce, maxForce) * (int)Random.Range(0, 2) == 0 ? -1 : 1;
        }
    }
}

