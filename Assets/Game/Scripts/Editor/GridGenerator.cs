using UnityEngine;
using UnityEditor;

namespace Nine
{
   public class GridGenerator : EditorWindow
   {
       //public GameObject dotPrefab;
       //public GameObject hLinePrefab;
       //public GameObject vLinePrefab;
       public GameObject tilePrefab;
       public int horizontalSize = 7;
       public int verticalSize = 7;
       public Sprite _bgSprite;
       //public Material _lineMaterial;
   
       [MenuItem("Tools/Grid Generator")]
       public static void ShowWindow()
       {
           GetWindow<GridGenerator>("Grid Generator");
       }
   
       private void OnGUI()
       {
           GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
   
           /*dotPrefab = (GameObject)EditorGUILayout.ObjectField("Dot Prefab", dotPrefab, typeof(GameObject), false);
           hLinePrefab = (GameObject)EditorGUILayout.ObjectField("Horizontal Line Prefab", hLinePrefab, typeof(GameObject), false);
           vLinePrefab = (GameObject)EditorGUILayout.ObjectField("Vertical Line Prefab", vLinePrefab, typeof(GameObject), false);*/
           tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
           
           _bgSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", _bgSprite, typeof(Sprite), false);
           //_lineMaterial = (Material)EditorGUILayout.ObjectField("Line Material", _lineMaterial, typeof(Material), false);
   
           horizontalSize = EditorGUILayout.IntField("Horizontal Size", horizontalSize);
           verticalSize = EditorGUILayout.IntField("Vertical Size", verticalSize);
   
           if (GUILayout.Button("Generate Grid"))
           {
               GenerateGrid();
           }
       }
   
       private void GenerateGrid()
       {
           GameObject gridObject = new GameObject($"Grid_{horizontalSize}x{verticalSize}");
           var gridComponent = gridObject.AddComponent<GridController>();
           gridComponent.Width = horizontalSize;
           gridComponent.Height = verticalSize;
           
           var bg = new GameObject
           {
               name = "GridBG",
               transform =
               {
                   parent = gridObject.transform,
                   localScale = new Vector3(horizontalSize / 4f, verticalSize / 4f, 1f),
                   localPosition = new Vector3(horizontalSize / 2f - 0.5f, verticalSize/2f - 0.5f, 0f),
                   localRotation = Quaternion.Euler(0f, 0f, 0f)
               }
           };
           var bgSprite = bg.AddComponent<SpriteRenderer>();
           bgSprite.sprite = _bgSprite;
           bgSprite.sortingOrder = -5;
           
           gridObject.layer = LayerMask.NameToLayer("Grid");
           
           GameObject highlightedItem = new GameObject("HighlightedItem");
           highlightedItem.transform.parent = gridObject.transform;
           
           GameObject tileGroup = new GameObject("Tiles");
           tileGroup.transform.parent = gridObject.transform;
   
           for (int y = 0; y < verticalSize; y++)
           {
               for (int x = 0; x < horizontalSize; x++)
               {
                   var square = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, tileGroup.transform);
                   square.name = $"Tile_({y+1},{x+1})";
                   square.transform.position = new Vector3(x, y, 0);
               }
           }
           
           var collider = gridObject.AddComponent<BoxCollider>();
           collider.center = new Vector3(horizontalSize / 2f - 0.5f, verticalSize / 2f - 0.5f, 0f);
           collider.size = new Vector3(horizontalSize + 0.75f, verticalSize + 0.75f, 0f);
       }
   }               
}