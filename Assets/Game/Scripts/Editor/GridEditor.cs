using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Nine
{
   public class GridEditor : EditorWindow
   {
       //public GameObject dotPrefab;
       //public GameObject hLinePrefab;
       //public GameObject vLinePrefab;
       //public GameObject squarePrefab;
       public GridController _grid;
       public Material _squareMaterial1;
       public Material _squareMaterial2;
   
       [MenuItem("Tools/Grid Editor")]
       public static void ShowWindow()
       {
           GetWindow<GridEditor>("Grid Editor");
       }
   
       private void OnGUI()
       {
           GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
           
           _grid = (GridController)EditorGUILayout.ObjectField("Grid", _grid, typeof(GridController), true);
           _squareMaterial1 = (Material)EditorGUILayout.ObjectField("Square Material 1", _squareMaterial1, typeof(Material), true);
           _squareMaterial2 = (Material)EditorGUILayout.ObjectField("Square Material 2", _squareMaterial2, typeof(Material), true);
   
           if (GUILayout.Button("Sort Grid"))
           {
               SortGrid();
           }
       }

       private void SortGrid()
       {
           var gridList = _grid.GetComponentsInChildren<GridElement>()
               .OrderBy(ge => ge.transform.localPosition.z)
               .ThenBy(ge => ge.transform.localPosition.x)
               .ToList();
           
           for (int i = 0; i < gridList.Count; i++)
           {
               gridList[i].gameObject.name = $"GridElement_({i / _grid.Width + 1},{i % _grid.Width + 1})";
               gridList[i].transform.SetSiblingIndex(i);
               if (_squareMaterial1 && _squareMaterial2)
               {
                   var material = i%2 == 0 ? _squareMaterial1 : _squareMaterial2;
                   gridList[i].GetComponent<Renderer>().material = material;
               }
           }
       }
   }               
}