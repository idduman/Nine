using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    [CreateAssetMenu(fileName = "LevelDataManager", menuName = "Scriptlable Objects/Level Data Manager")]
    public class LevelDataManager : ScriptableObject
    {
        public static LevelDataManager CreateFromString(string strData)
        {
            var levelDataManager = ScriptableObject.CreateInstance<LevelDataManager>();
           var rows = strData.Split("\n");
           for (int i = 1; i < rows.Length; i++)
           {
               var validData = true;
               
               var levelData = new LevelData { ShapeData = new List<ShapeData>()};
               
               var columns = rows[i].Split(",");
               if (columns.Length < 17)
               {
                   validData = false;
                   Debug.LogWarning($"Invalid row size {columns.Length} in row {i}");
                   continue;
               }
               
               levelData.HardLevel = columns[0].Contains("Hard", StringComparison.InvariantCultureIgnoreCase);
               if(!int.TryParse(columns[1].Trim().Substring(0,1), out var size))
               {
                   validData = false;
                   Debug.LogWarning($"Invalid grid size data: \"{columns[1]}\" in row {i}");
                   continue;
               }
               levelData.GridSize = size;
               if (!int.TryParse(columns[2].Trim(), out var scoreToWin))
               {
                   Debug.LogWarning($"Invalid win score data: \"{columns[2]}\" in row {i}");
                   continue;
               }
               levelData.ScoreToWin = scoreToWin;
               if (!int.TryParse(columns[3].Trim(), out var goal))
               {
                   Debug.LogWarning($"Invalid goal data: \"{columns[3]}\" in row {i}");
                   continue;
               }
               levelData.NumberToWin = goal;
               if (!int.TryParse(columns[4].Trim(), out var suggestChance))
               {
                   Debug.LogWarning($"Invalid suggest chance data: \"{columns[3]}\" in row {i}");
                   continue;
               }
               levelData.SuggestChance = suggestChance;
               
               for (int c = 5; c <= 12; c++)
               {
                   if (!int.TryParse(columns[c].Trim(), out var colorChance))
                   {
                       Debug.LogWarning($"Invalid number chance data: \"{columns[c]}\" in row {i}");
                       validData = false;
                       break;
                   }

                   levelData.NumberData.Add(new NumberData
                   {
                       Number = c-3,
                       Chance = colorChance
                   });
               }

               for (int s = 13; s <= 20; s++)
               {
                   if(!int.TryParse(columns[s].Trim(), out var shapeChance))
                   {
                       Debug.LogWarning($"Invalid shape chance data: \"{columns[s]}\" in row {i}");
                       validData = false;
                       break;
                   }
                   
                   levelData.ShapeData.Add(new ShapeData
                   {
                       Shape = (ShapeType)(s-13),
                       Chance = shapeChance
                   });
               }
               
               if(validData)
                   levelDataManager.LevelDatas.Add(levelData);
           }

           return levelDataManager;
        }
        
        [SerializeField] public List<LevelData> LevelDatas = new List<LevelData>();
    }
}

