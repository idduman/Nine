using System.Collections.Generic;
using UnityEngine;

namespace Garawell.Utility.Triangulator
{
    public class PointGizmoHandler : MonoBehaviour
    {
        public List<Vector2> vertices = new List<Vector2>();
        public Material textureMaterial;

        public void SetMaterial(Material material)
        {
            textureMaterial = material;
        }

        public Material GetMaterial()
        {
            return textureMaterial;
        }

        public Vector2[] GetPoints()
        {
            return vertices.ToArray();
        }

        private void OnDrawGizmos()
        {
            if (vertices.Count <= 0) { return; }

            Gizmos.color = Color.red;

            for (int i = 0; i < vertices.Count; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }
}

