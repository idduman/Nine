using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] public bool HardLevel;
        [SerializeField] public int GridSize;
        [SerializeField] public int ScoreToWin = 250;
        [SerializeField] public int NumberToWin = 7;
        [SerializeField] public int SuggestChance = 9;
        [SerializeField] public List<NumberData> NumberData = new List<NumberData>();
        [SerializeField] public List<ShapeData> ShapeData = new List<ShapeData>();
    }
}

