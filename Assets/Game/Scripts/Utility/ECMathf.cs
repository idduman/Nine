using System.Collections.Generic;
using UnityEngine;

namespace Garawell.Utility
{
    public struct ECMathf
    {
        public static float Remap(float currentValue, float x1, float y1, float x2, float y2)
        {
            float t = Mathf.InverseLerp(x1, y1, currentValue);
            return Mathf.Lerp(x2, y2, t);
        }

        public static T FindClosest<T>(Vector3 position, List<T> objects) where T : Component
        {
            float distance = Vector3.Distance(position, objects[0].transform.position);
            int index = 0;
            for (int i = 1; i < objects.Count; i++)
            {
                float tempDis = Vector3.Distance(position, objects[i].transform.position);
                if (tempDis < distance)
                {
                    distance = tempDis;
                    index = i;
                }
            }

            return objects[index];
        }
    }
}