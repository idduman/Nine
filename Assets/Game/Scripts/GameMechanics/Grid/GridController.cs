using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameAnalyticsSDK;
using Garawell.Managers;
using Garawell.Managers.Events;
using UnityEngine;

namespace Nine
{
    public class GridController : MonoBehaviour
    {
        public int Width;
        public int Height;

        [SerializeField] private Transform _elementsParent;
        [SerializeField] private Transform _highlightedItemParent;
        [SerializeField] private float _highlightYOffset = 0.5f;
        [SerializeField] private GridLock[] _gridLocks;
        
        public Transform ElementsParent => _elementsParent;
        //public int ItemCount => Items.Count;

        //public List<ShapeItem> Items { get; private set; }
        public GridElement[] Elements => _gridElements;
        
        public bool Updating {get; private set;}

        private int _level;

        private BoxCollider _collider;

        private ShapeItem _highlightedItem;

        private bool _validMove;
        private bool _highlighted;

        private Sequence _traySequence;
        private GridElement[] _gridElements;

        private List<GridElement> _highlightedElements;

        private int _lastAddedSegmentCount;
        private int _unlockedWidth;
        private int _unlockedHeight;

        private Vector2 _highlightedShapeBoundsX;
        private Vector2 _highlightedShapeBoundsY;
        private bool _moveCompleted;

        private GameObject _popParticle;

        public int GetIndex(int x, int y)
        {
            return y * Width + x;
        }

        public void Initialize()
        {
            _popParticle = MainManager.Instance.AssetManager.GetPrefab("PopParticle");
            
            _level = PlayerPrefs.GetInt("Level", 0) + 1;
            _collider = GetComponent<BoxCollider>();
            _gridElements = GetComponentsInChildren<GridElement>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _gridElements[GetIndex(x, y)].SetIndex(x, y);
                }
            }

            _highlightedElements = new List<GridElement>();
            ResetGrid();
            _unlockedWidth = Mathf.RoundToInt(_gridElements.Where(ge => !ge.IsLocked && ge.IsActive)
                .Max(ge => ge.transform.localPosition.x));
            _unlockedHeight = Mathf.RoundToInt(_gridElements.Where(ge => !ge.IsLocked && ge.IsActive)
                .Max(ge => ge.transform.localPosition.z));

            _moveCompleted = true;
        }

        public void ResetGrid()
        {
            foreach (var e in _gridElements)
            {
                if (e.IsFilled)
                    e.Empty();
            }
            /*if (Items != null)
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    Destroy(Items[i].gameObject);
                }
                Items.Clear();
            }
            else
                Items = new List<ShapeItem>();*/
        }

        public void LockAll()
        {
            foreach (var g in _gridElements)
            {
                if(!g.IsFilled)
                    g.Lock();
            }
        }

        public void Unlock(int x, int y)
        {
            _gridElements[GetIndex(x, y)].Unlock();
        }
        
        private void InitializeLocks()
        {
            foreach (var l in _gridLocks)
            {
                l.Initialize(_gridElements);

                if (l is GridLockLevel g)
                {
                    if (_level < g.LevelToUnlock)
                    {
                        g.Lock();
                    }
                    else if (_level == g.LevelToUnlock && PlayerPrefs.GetInt($"Unlock{_level}", 0) == 0)
                    {
                        PlayerPrefs.SetInt($"Unlock{_level}", 1);
                        g.Lock();
                        g.UnlockWithAnimation();
                    }
                    else
                        g.Unlock();

                    if (_level < 5 && g.LevelToUnlock > 5 ||
                        _level < 10 && g.LevelToUnlock > 10 ||
                        _level < 20 && g.LevelToUnlock > 20 ||
                        _level < 30 && g.LevelToUnlock > 30 ||
                        _level < 40 && g.LevelToUnlock > 40 ||
                        _level < 50 && g.LevelToUnlock > 50 ||
                        _level < 75 && g.LevelToUnlock > 75 ||
                        _level < 100 && g.LevelToUnlock > 100)
                        g.Deactivate();
                    else
                        g.Activate();
                }
                else
                    l.Lock();
            }
        }

        private Vector2Int GetDimensions(ShapeItem item)
        {
            var dimXMin = 0;
            var dimXMax = 0;
            var dimZMin = 0;
            var dimZMax = 0;

            foreach (var slot in item.Slots)
            {
                var slotOffset = Quaternion.AngleAxis(item.RotationMode * 90f, Vector3.up) *
                                 (slot.transform.localPosition - item.Slots[0].transform.localPosition);

                var offsetX = Mathf.RoundToInt(slotOffset.x);
                var offsetZ = Mathf.RoundToInt(slotOffset.z);

                if (offsetX > dimXMax)
                    dimXMax = offsetX;

                if (offsetX < dimXMin)
                    dimXMin = offsetX;

                if (offsetZ > dimZMax)
                    dimZMax = offsetZ;

                if (offsetZ < dimZMin)
                    dimZMin = offsetZ;
            }

            return new Vector2Int(dimXMax - dimXMin + 1, dimZMax - dimZMin + 1);
        }

        public int GetLockedRvCount()
        {
            var counter = 0;
            for (int i = 0; i < _gridLocks.Length; i++)
            {
                if (_gridLocks[i] is GridLockRv { Locked: true })
                    counter++;
            }

            return counter;
        }

        public bool CheckForSpace(ShapeItem item)
        {
            var offset = item.transform.position - item.Slots[0].transform.position;
            for (float y = -0.5f; y <= Height + 0.5f; y += 0.5f)
            {
                for (float x = -0.5f; x <= Width + 0.5f; x += 0.5f)
                {
                    var globalPos = transform.TransformPoint(new Vector3(x, y, 0f)) + offset;
                    if (ValidateMove(item, globalPos))
                        return true;
                }
            }

            return false;
        }

        private bool ValidateMove(ShapeItem item, Vector3 position, bool debug = false)
        {
            _highlightedElements.Clear();
            var slotPos = item.Slots[0].transform.position;
            var offset = position - item.transform.position;
            var localPos = transform.InverseTransformPoint(slotPos + offset);
            foreach (var slot in item.Slots)
            {
                var center = slot.transform.position + offset;
                if (center.x < _collider.bounds.min.x
                    || center.x > _collider.bounds.max.x
                    || center.y < _collider.bounds.min.y
                    || center.y > _collider.bounds.max.y)
                {
                    return false;
                }

                var slotOffset = Quaternion.AngleAxis(item.RotationMode * 90f, Vector3.forward) *
                                 (slot.transform.localPosition - item.Slots[0].transform.localPosition);

                var offsetX = Mathf.RoundToInt(slotOffset.x);
                var offsetY = Mathf.RoundToInt(slotOffset.y);

                var fx = Mathf.FloorToInt(localPos.x);
                var x = localPos.x - fx >= 0.5f ? fx + 1 : fx;
                x += offsetX;

                var fy = Mathf.FloorToInt(localPos.y);
                var y = localPos.y - fy >= 0.5f ? fy + 1 : fy;
                y += offsetY;

                if (x < 0 || x > Width - 1)
                    return false;
                if (y < 0 || y > Height - 1)
                    return false;

                var index = y * Width + x;
                if (index > _gridElements.Length || !_gridElements[index] ||
                    _gridElements[index].IsFilled || _gridElements[index].IsLocked)
                    return false;

                _highlightedElements.Add(_gridElements[index]);
            }

            return true;
        }

        public void ResetHighlight()
        {
            if (_highlightedItem)
                _highlightedItem.transform.position = _highlightedItemParent.position;
        }

        public bool HighlightShape(ShapeItem item, Vector3 position)
        {
            if (!_highlightedItem || _highlightedItem.ID != item.ID || _highlightedItem.Color != item.Color)
                SetHighlightedItem(item);

            var slotPos = item.Slots[0].transform.position;
            var slotOffset = slotPos - item.transform.position;
            slotOffset.z = 0;
            
            var localPos = transform.InverseTransformPoint(position) + slotOffset;

            var x = Math.Clamp(Mathf.RoundToInt(localPos.x), -_highlightedShapeBoundsX.x,
                Width - 1 - _highlightedShapeBoundsX.y);
            var y = Math.Clamp(Mathf.RoundToInt(localPos.y), -_highlightedShapeBoundsY.x,
                Height - 1 - _highlightedShapeBoundsY.y);

            _highlightedItem.transform.localPosition = new Vector3(x, y, _highlightedItemParent.localPosition.z) - slotOffset;
            _highlightedItem.ShapeTransform.localPosition = Vector3.zero;

            if (_moveCompleted && ValidateMove(_highlightedItem, _highlightedItem.transform.position))
            {
                _highlightedItem.gameObject.SetActive(true);
                _highlighted = true;
                return true;
            }
            else
                _highlightedElements.Clear();

            if (_highlightedItem)
                _highlightedItem.gameObject.SetActive(false);

            _highlighted = false;
            return false;
        }

        public void SetHighlightedItem(ShapeItem item)
        {
            if (_highlightedItem)
                Destroy(_highlightedItem.gameObject);

            _highlightedItem = Instantiate(item, transform);
            _highlightedItem.Pivot.localPosition = Vector3.zero;
            _highlightedItem.Pivot.localScale = Vector3.one;
            _highlightedItem.SetInfo(item.ID, item.Shape, item.RotationMode);
            _highlightedItem.SetTiles(item.Tiles);
            _highlightedItem.ToggleHighlight(true);
            _highlightedItem.ToggleShadow(false);

            _highlightedShapeBoundsX = new Vector2(0f, 0f);
            _highlightedShapeBoundsY = new Vector2(0f, 0f);
            foreach (var slot in _highlightedItem.Slots)
            {
                var slotOffset = Quaternion.AngleAxis(item.RotationMode * 90f, Vector3.forward) *
                                 (slot.transform.localPosition - item.Slots[0].transform.localPosition);

                if (slotOffset.x > 0 && slotOffset.x > _highlightedShapeBoundsX.y)
                    _highlightedShapeBoundsX.y = Mathf.RoundToInt(slotOffset.x);
                if (slotOffset.x < 0 && slotOffset.x < _highlightedShapeBoundsX.x)
                    _highlightedShapeBoundsX.x = Mathf.RoundToInt(slotOffset.x);
                if (slotOffset.y > 0 && slotOffset.y > _highlightedShapeBoundsY.y)
                    _highlightedShapeBoundsY.y = Mathf.RoundToInt(slotOffset.y);
                if (slotOffset.y < 0 && slotOffset.y < _highlightedShapeBoundsY.x)
                    _highlightedShapeBoundsY.x = Mathf.RoundToInt(slotOffset.y);
            }

            foreach (var tile in _highlightedItem.Tiles)
            {
                tile.transform.localScale = new Vector3(0.915f, 0.915f, 0.95f);
            }
        }

        public bool AddShape(ShapeItem item, Vector3 position)
        {
            if (Updating || !HighlightShape(item, position))
            {
                return false;
            }

            _moveCompleted = false;
            
            _highlightedItem.ToggleHighlight(false);
            _highlightedItem.ToggleShadow(true);

            for (int i = 0; i < _highlightedElements.Count; i++)
            {
                _highlightedElements[i].Fill(_highlightedItem.Slots[i].Tile);
                _highlightedItem.Slots[i].Tile.SetIndex(_highlightedElements[i].Index);
            }

            Destroy(_highlightedItem.gameObject);
            _highlightedItem = null;

            UpdateGrid();

            StartCoroutine(ShapeAddedRoutine());

            return true;
        }
        
        public void AddTile(int number, Vector3 position)
        {
            if (Updating)
                return;
            
            var localPos = transform.InverseTransformPoint(position);
            var x = Math.Clamp(Mathf.RoundToInt(localPos.x), 0, Width-1);
            var y = Math.Clamp(Mathf.RoundToInt(localPos.y), 0, Height-1);
            
            var index = GetIndex(x, y);
            var element = _gridElements[index];

            if (!element.IsActive || element.IsLocked || element.IsFilled)
                return;

            var tilePrefab = MainManager.Instance.AssetManager.GetPrefab("ShapeTile");
            var tile = Instantiate(tilePrefab, transform).GetComponent<ShapeTile>();
            tile.transform.localScale = new Vector3(0.915f, 0.915f, 0.95f);
            element.Fill(tile);
            tile.transform.localPosition = Vector3.zero;
            tile.SetColor((ColorCode)number);
            tile.SetIndex(element.Index);
            
            _highlightedElements.Clear();
            _highlightedElements.Add(element);
            UpdateGrid();
        }

        private void UpdateGrid(float delay = 0f)
        {
            if (Updating)
                return;
            
            StartCoroutine(UpdateGridRoutine(delay));
        }

        private bool MoveTile(int x1, int y1, int x2, int y2)
        {
            if (!IsValidAndFilled(x1, y1) || !IsValidIndex(x2, y2))
                return false;

            var src = _gridElements[GetIndex(x1, y1)];
            var dest = _gridElements[GetIndex(x2, y2)];
            if (!dest.IsActive || dest.IsLocked || dest.IsFilled)
                return false;
            
            dest.Fill(src.Tile);
            src.Detach();
            dest.Tile.SetIndex(new Vector2Int(x2, y2));
            dest.Tile.transform.DOLocalMove(Vector3.zero, 0.25f)
                .SetEase(Ease.Linear);
            return true;
        }

        public bool PowerUpItem(ShapeTile tile, PowerUpType powerUpType)
        {
            if (Updating)
                return false;
            
            var index = tile.Index;
            var e = _gridElements[GetIndex(index.x, index.y)];
            _highlightedElements.Clear();
            switch (powerUpType)
            {
                case PowerUpType.PlusOne:
                    if (tile.Number < 9)
                    {
                        tile.Increment();
                        _highlightedElements.Add(e);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case PowerUpType.MinusOne:
                    if (tile.Number > 2)
                    {
                        tile.Decrement();
                        _highlightedElements.Add(e);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case PowerUpType.MoveUp:
                    if (MoveTile(index.x, index.y, index.x, index.y + 1))
                    {
                        _highlightedElements.Add(_gridElements[GetIndex(index.x, index.y+1)]);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case PowerUpType.MoveDown:
                    if (MoveTile(index.x, index.y, index.x, index.y - 1))
                    {
                        _highlightedElements.Add(_gridElements[GetIndex(index.x, index.y-1)]);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case PowerUpType.MoveLeft:
                    if (MoveTile(index.x, index.y, index.x - 1, index.y))
                    {
                        _highlightedElements.Add(_gridElements[GetIndex(index.x-1, index.y)]);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case PowerUpType.MoveRight:
                    if (MoveTile(index.x, index.y, index.x + 1, index.y))
                    {
                        _highlightedElements.Add(_gridElements[GetIndex(index.x+1, index.y)]);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerUpType), powerUpType, null);
            }
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"PowerUpUsed:{powerUpType.ToString()}:Level", PlayerPrefs.GetInt("Level", 0) + 1);
#endif
            UpdateGrid(0.5f);
            
            return true;
        }

        public void UnlockRv(int rvArgsId, bool animation = false)
        {
            var rvLock = _gridLocks.FirstOrDefault(gl => gl is GridLockRv glrv && glrv.ID == rvArgsId);
            if (!rvLock)
            {
                Debug.LogError($"Grid RV Lock not found with id {rvArgsId}");
                return;
            }

            if (animation)
                rvLock.UnlockWithAnimation();
            else
                rvLock.Unlock();
        }

        public void UnlockRvAll()
        {
            foreach (var g in _gridLocks)
            {
                if (g is GridLockRv r)
                    r.Unlock();
            }
        }

        public void UnlockAll()
        {
            foreach (var gl in _gridLocks)
                gl.Unlock();
        }

        public void OnFinish(bool success)
        {
            if (_highlightedItem)
                Destroy(_highlightedItem.gameObject);

            _highlightedItem = null;

            if (!success)
                return;
            

            for(int i = 0; i < _gridElements.Length; i++)
            {
                if(!_gridElements[i].IsFilled)
                    continue;
                
                var tr = _gridElements[i].transform;
                tr.DOScale(0f, 0.2f)
                    .SetDelay(1f + GetGridDistance(_gridElements[i].Index, new Vector2Int(0,0)) * 0.1f)
                    .SetEase(Ease.Linear);
            }

        }

        private bool IsValidIndex(int x, int y)
        {
            var index = GetIndex(x, y);
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private bool IsValidAndFilled(int x, int y)
        {
            var index = GetIndex(x, y);
            return IsValidIndex(x, y) &&
                   _gridElements[index] &&
                   _gridElements[index].IsActive &&
                   !_gridElements[index].IsLocked &&
                   _gridElements[index].IsFilled;
        }

        private bool IsNeighbor(GridElement e1, GridElement e2)
        {
            return GetGridDistance(e1.Index, e2.Index) == 1;
        }

        private List<GridElement> GetMatchingNeighbors(GridElement e)
        {
            List<GridElement> elementsToCheck = new List<GridElement>();
            List<GridElement> matchingNeighbours = new List<GridElement>();
            elementsToCheck.Add(e);

            bool keepChecking = true;

            while (keepChecking)
            {
                List<GridElement> neighbors = new List<GridElement>();
                keepChecking = false;
                foreach (var etc in elementsToCheck)
                {
                    var index = etc.Index;
                    if (IsValidAndFilled(index.x - 1, index.y))
                    {
                        var n1 = _gridElements[GetIndex(index.x - 1, index.y)];
                        if (n1 && n1.Tile.Number == e.Tile.Number && !matchingNeighbours.Contains(n1) &&
                            !neighbors.Contains(n1))
                        {
                            neighbors.Add(n1);
                            keepChecking = true;
                        }
                    }

                    if (IsValidAndFilled(index.x + 1, index.y))
                    {
                        var n2 = _gridElements[GetIndex(index.x + 1, index.y)];
                        if (n2 && n2.Tile.Number == e.Tile.Number && !matchingNeighbours.Contains(n2) &&
                            !neighbors.Contains(n2))
                        {
                            neighbors.Add(n2);
                            keepChecking = true;
                        }
                    }

                    if (IsValidAndFilled(index.x, index.y - 1))
                    {
                        var n3 = _gridElements[GetIndex(index.x, index.y - 1)];
                        if (n3 && n3.Tile.Number == e.Tile.Number && !matchingNeighbours.Contains(n3) &&
                            !neighbors.Contains(n3))
                        {
                            neighbors.Add(n3);
                            keepChecking = true;
                        }
                    }

                    if (IsValidAndFilled(index.x, index.y + 1))
                    {
                        var n4 = _gridElements[GetIndex(index.x, index.y + 1)];
                        if (n4 && n4.Tile.Number == e.Tile.Number && !matchingNeighbours.Contains(n4) &&
                            !neighbors.Contains(n4))
                        {
                            neighbors.Add(n4);
                            keepChecking = true;
                        }
                    }
                }

                matchingNeighbours.AddRange(elementsToCheck);
                elementsToCheck.Clear();
                elementsToCheck.AddRange(neighbors);
            }

            return matchingNeighbours;
        }

        private int GetNeighborCount(GridElement e)
        {
            var count = 0;
            if (IsValidAndFilled(e.Index.x - 1, e.Index.y))
                count++;
            if (IsValidAndFilled(e.Index.x + 1, e.Index.y))
                count++;
            if (IsValidAndFilled(e.Index.x, e.Index.y - 1))
                count++;
            if (IsValidAndFilled(e.Index.x, e.Index.y + 1))
                count++;

            return count;
        }

        private int GetPlusOneNeighborCount(GridElement e)
        {
            var count = 0;
            if (IsValidAndFilled(e.Index.x - 1, e.Index.y))
            {
                var n1 = _gridElements[GetIndex(e.Index.x - 1, e.Index.y)];
                if (n1.Tile.Number == e.Tile.Number + 1)
                    count++;
            }

            if (IsValidAndFilled(e.Index.x + 1, e.Index.y))
            {
                var n2 = _gridElements[GetIndex(e.Index.x + 1, e.Index.y)];
                if (n2.Tile.Number == e.Tile.Number + 1)
                    count++;
            }

            if (IsValidAndFilled(e.Index.x, e.Index.y - 1))
            {
                var n3 = _gridElements[GetIndex(e.Index.x, e.Index.y - 1)];
                if (n3.Tile.Number == e.Tile.Number + 1)
                    count++;
            }

            if (IsValidAndFilled(e.Index.x, e.Index.y + 1))
            {
                var n4 = _gridElements[GetIndex(e.Index.x, e.Index.y + 1)];
                if (n4.Tile.Number == e.Tile.Number + 1)
                    count++;
            }

            return count;
        }

        private List<GridElement> GetNeighbors(GridElement e)
        {
            var neighbors = new List<GridElement>();
            if (IsValidAndFilled(e.Index.x - 1, e.Index.y))
                neighbors.Add(_gridElements[GetIndex(e.Index.x - 1, e.Index.y)]);

            if (IsValidAndFilled(e.Index.x + 1, e.Index.y))
                neighbors.Add(_gridElements[GetIndex(e.Index.x + 1, e.Index.y)]);

            if (IsValidAndFilled(e.Index.x, e.Index.y - 1))
                neighbors.Add(_gridElements[GetIndex(e.Index.x, e.Index.y - 1)]);

            if (IsValidAndFilled(e.Index.x, e.Index.y + 1))
                neighbors.Add(_gridElements[GetIndex(e.Index.x, e.Index.y + 1)]);

            return neighbors;
        }

        private int GetRank(GridElement e, GridElement n)
        {
            var diff = n.Tile.Number - e.Tile.Number;
            if (diff == 0)
                return 0;
            if (diff < 0)
                return 1;
            if (diff == 1)
                return 1000000;

            return Mathf.FloorToInt(Mathf.Pow(4, 7 - diff));
        }
        
        private int GetNeighborRank(GridElement e, GridElement n)
        {
            var diff = n.Tile.Number - e.Tile.Number;
            if (diff <= 0)
                return 0;
            if (diff == 1)
                return 16;
            if (diff == 2)
                return 4;

            return 1;
        }
        
        private int GetNeighborRanking(GridElement e)
        {
            var rank = 0;
            var neighbors = GetNeighbors(e);
            foreach (var n in neighbors)
            {
                var r = GetRank(e, n);
                
                var nRank = 0;
                var nNeighbours = GetNeighbors(n);
                foreach (var nNeighbour in nNeighbours)
                {
                    if(neighbors.Contains(nNeighbour) || nNeighbour == e)
                        continue;
                    
                    nRank += r * GetNeighborRank(e, nNeighbour);
                }

                rank += r;
                rank += nRank;
            }

            return rank;
        }

        private List<GridElement> SortByPathDistance(List<GridElement> elements)
        {
            if (elements.Count == 0)
                return elements;
            
            var sorted = new List<GridElement>();
            var toCheck = new List<GridElement>();
            toCheck.Add(elements[0]);
            while (toCheck.Count > 0)
            {
                sorted.AddRange(toCheck);
                var checkList = new List<GridElement>(toCheck);
                toCheck.Clear();
                for (int i = 0; i < checkList.Count; i++)
                {
                    var check = checkList[i];
                    foreach (var element in elements)
                    {
                        if(IsNeighbor(check, element) && !sorted.Contains(element))
                            toCheck.Add(element);
                    }
                }
            }

            return sorted;
        }

        private int GetGridDistance(Vector2Int p1, Vector2Int p2)
        {
            return Math.Abs(p2.x - p1.x) + Math.Abs(p2.y - p1.y);
        }

        private IEnumerator ShapeAddedRoutine()
        {
            yield return new WaitForEndOfFrame();
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnShapeAdded);
        }

        private IEnumerator UpdateGridRoutine(float delay = 0f)
        {
            Updating = true;
            bool win = false;
            var elementsToCheck = new List<GridElement>(_highlightedElements);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(delay);
            while (elementsToCheck.Count > 0)
            {
                var elements = elementsToCheck.Where(e => e.Tile)
                    .OrderBy(e => e.Tile.Number).ToList();
                foreach (var e in elements)
                {
                    if (!e || !e.IsFilled)
                    {
                        elementsToCheck.Remove(e);
                        continue;
                    }
                    
                    var popList = new List<ShapeTile>();
                    var matching = GetMatchingNeighbors(e).OrderByDescending(GetNeighborRanking).
                        ThenBy(el => el.Index.y).ThenBy(el => el.Index.x).ToList();
                    var e2 = matching.First();
                    if (e2 != e)
                    {
                        elementsToCheck.Remove(e);
                        elementsToCheck.Add(e2);
                    }
                    
                    var matchCount = NineManager.Instance.MergeMode == MergeMode.Number ? e2.Tile.Number : 2;
                    if (e2 && e2.IsFilled && matching.Count >= matchCount)
                    {
                        var matchingTiles = SortByPathDistance(matching);
                        for (int i = matchingTiles.Count-1; i > 0; i--)
                        {
                            var m = matchingTiles[i];
                            if (!m.IsFilled)
                            {
                                matchingTiles.Remove(m);
                                if (elementsToCheck.Contains(m))
                                    elementsToCheck.Remove(m);
                                continue;
                            }
                            
                            //var tiles = matchingTiles.OrderBy(t => GetGridDistance(m.Index, t.Index));
                            var n = matchingTiles.LastOrDefault(t => t != m && t.IsFilled && IsNeighbor(t,m));
                            if (n)
                            {
                                var tile = m.Tile;
                                m.Detach();
                                popList.Add(tile);
                                matchingTiles.Remove(m);

                                tile.transform.SetParent(n.Tile.Pivot);
                                
                                var offset = 0.25f * (n.Tile.StackSize + 1);
                                var dist = n.Index - m.Index;
                                var rotateAxis = Math.Abs(dist.x) > 0
                                    ? Math.Sign(dist.x) * Vector3.down
                                    : Math.Sign(dist.y) * Vector3.right;

                                yield return new WaitForSeconds(0.02f);
                                
                                tile.Pivot.DORotate(180f * rotateAxis, 0.18f, RotateMode.WorldAxisAdd);
                                offset += 0.25f * tile.StackSize;
                            

                                tile.transform.DOVectorJump(n.Tile.transform.position - offset * Vector3.forward,
                                    Vector3.back, 1f + n.Tile.StackSize * 0.25f, 0.18f).SetEase(Ease.Linear)
                                    .OnComplete(() =>
                                    {
                                        tile.HideNumber();
                                        MainManager.Instance.AudioManager.PlayAudio(AudioID.Flip);
                                        NineManager.Instance.AddScore(10);
                                    });
                                n.Tile.StackSize += tile.StackSize + 1;
                            }

                            if (elementsToCheck.Contains(m))
                                elementsToCheck.Remove(m);

                            yield return new WaitForSeconds(0.2f);
                        }

                        yield return new WaitForSeconds(0.15f);
                        
                        if (e2.Tile.Number < 9)
                        {
                            e2.Tile.SetColor(e2.Tile.Color + 1);
                            e2.Tile.StackSize = 0;
                            if (e2.Tile.Number >= NineManager.Instance.NumberToWin)
                                win = true;
                        }
                        foreach (var t in popList.OrderBy(t => t.transform.position.z))
                        {
                            t.NumberParent.DOScale(0f, 0.15f);
                            yield return new WaitForSeconds(0.05f);
                            Instantiate(_popParticle, t.Pivot.position, Quaternion.identity, transform);
                            yield return new WaitForSeconds(0.05f);
                            MainManager.Instance.AudioManager.PlayAudio(AudioID.Blast);
                            //NineManager.Instance.AddScore(10);
                        }
                        /*else
                        {
                            _highlightedElements.Remove(e2);
                            popList.Add(e2.Tile);
                            e2.Detach();
                        }*/

                        yield return new WaitForEndOfFrame();
                        
                        for (int i = popList.Count-1; i >= 0; i--)
                        {
                            Destroy(popList[i].gameObject);
                        }
                        
                        NineManager.Instance.AddScore(e2.Tile.Number * 10);

                        yield return new WaitForSeconds(0.2f);
                    }
                    else
                        elementsToCheck.Remove(e2);
                }

                yield return new WaitForSeconds(0.2f);
            }
            
            if (win)
                NineManager.Instance.FinishGame(true);

            _highlightedElements.Clear();
            _moveCompleted = true;
            EventRunner.MoveCompleted();
            Updating = false;
        }
    }
}