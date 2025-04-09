using UnityEngine;

namespace Garawell.Utility.Triangulator
{
	public class PolygonTester : MonoBehaviour
	{

		[SerializeField] private PointGizmoHandler handler;
		private Vector2[] vertices2D;
		private Material meshMaterial;

		public void DoTriangulate()
		{
			vertices2D = handler.GetPoints();

			meshMaterial = handler.GetMaterial();


			// Use the triangulator to get indices for creating triangles
			Triangulator tr = new Triangulator(vertices2D);
			int[] indices = tr.Triangulate();

			// Create the Vector3 vertices
			Vector3[] vertices = new Vector3[vertices2D.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
			}

			// Create the mesh
			Mesh msh = new Mesh();
			msh.vertices = vertices;
			msh.triangles = indices;
			msh.RecalculateNormals();
			msh.RecalculateBounds();

			// Set up game object with mesh;
			gameObject.AddComponent(typeof(MeshRenderer));
			MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			filter.mesh = msh;

			gameObject.GetComponent<MeshRenderer>().material = meshMaterial;

		}
	}
}
