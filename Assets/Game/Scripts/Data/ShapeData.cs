using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    [Serializable]
    public struct ShapeData
    {
        [SerializeField] public ShapeType Shape;
        [SerializeField] public int Chance;
    }
}