using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nine
{
    public class ShapeItem : MonoBehaviour
    {
        [SerializeField] private ShapeType _shape;
        [SerializeField] private ColorCode _color;
        [SerializeField] private SymmetryMode _symmetry;
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _flip;
        [FormerlySerializedAs("_shape")] 
        [SerializeField] private Transform _shapeTransform;
        [SerializeField] private int _complexity = 1;
        [SerializeField] private ShapeSlot[] _slots;
        public int ID { get; private set; }

        public Vector3 InventoryOffset;
        public ColorCode Color => _color;
        public ShapeType Shape => _shape;
        public ShapeSlot[] Slots => _slots;
        public ShapeTile[] Tiles => _tiles;
        public int RotationMode => _rotationMode;
        public SymmetryMode Symmetry => _symmetry;
        public Transform Pivot => _pivot;
        public Transform Flip => _flip;
        public Transform ShapeTransform => _shapeTransform;
        public int Complexity => _complexity;

        private int _rotationMode;
        private List<GridElement> _filledSlots;
        private ShapeTile[] _tiles;
        
        private Sequence _jiggleSequence;
        
        private Material _highlightMaterial;

        private void Awake()
        {
            _tiles = GetComponentsInChildren<ShapeTile>();
        }

        public void SetInfo(int id, ShapeType shape, int rotationMode)
        {
            ID = id;
            _shape = shape;
            _rotationMode = rotationMode;
        }

        public void SetTiles(ShapeTile[] tiles)
        {
            for (int t = 0; t < _tiles.Length; t++)
            {
                _tiles[t].SetColor(tiles[t].Color);
            }
        }

        public int GetNumberCount(int number)
        {
            var count = 0;
            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i].Number == number)
                    count++;
            }

            return count;
        }

        public int GetNumberCountAdjacent(int slotIndex, int number)
        {
            var neighbors = new List<ShapeSlot>();
            var slotsToCheck = new List<ShapeSlot>();
            var slot = _slots[slotIndex];
            slotsToCheck.Add(slot);
            while (slotsToCheck.Count > 0)
            {
                var stc = slotsToCheck[0];
                foreach (var s in _slots)
                {
                    if(stc == s)
                        continue;
                    
                    var dist = Vector3.Distance(stc.transform.localPosition, s.transform.localPosition);
                    if (Mathf.Approximately(dist, 1f) && s.Tile.Number == number
                        && s != slot && !neighbors.Contains(s))
                    {
                        neighbors.Add(s);
                        slotsToCheck.Add(s);
                    }
                }
                slotsToCheck.RemoveAt(0);
            }

            return neighbors.Count;
        }

        public void ToggleHighlight(bool highlight)
        {
            foreach (var r in _tiles)
            {
                r.ToggleHighlight(highlight);
            }
        }

        public float GetBottomOffset()
        {
            var minOffset = 0f;
            foreach (var s in _slots)
            {
                var yPos = transform.InverseTransformPoint(s.transform.position).y;
                if(yPos < minOffset)
                    minOffset = yPos;
            }
            return 0.5f - minOffset;
        }

        public void ResetNumberRotations()
        {
            foreach (var t in _tiles)
            {
                t.ResetNumberRotation();
            }
        }

        public void ToggleShadow(bool shadow)
        {
            foreach (var r in _tiles)
            {
                r.ToggleShadow(shadow);
            }
        }

        public enum SymmetryMode
        {
            None,
            Half,
            Full
        }
    }
}

