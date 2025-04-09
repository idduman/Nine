using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    public class ShapeSlot : MonoBehaviour
    {
        [SerializeField] private ShapeTile _tile;
        
        public ShapeTile Tile => _tile;
    }
}