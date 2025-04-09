using UnityEngine;

namespace Garawell.Utility
{
    [System.Serializable]
    public struct ECVector2Int
    {
        public int X;
        public int Y;

        public ECVector2Int(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public ECVector2Int(Vector2 vector2)
        {
            X = (int)vector2.x;
            Y = (int)vector2.y;
        }
        public Vector2 GetVector2()
        {
            return new Vector2(X, Y);
        }

        public void SetFromVector2(Vector2 vector2)
        {
            X = (int)vector2.x;
            Y = (int)vector2.y;
        }

        public void Add(ECVector2Int addedVector)
        {
            X += addedVector.X;
            Y += addedVector.Y;
        }
    }
}
  


