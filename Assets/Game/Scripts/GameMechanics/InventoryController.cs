using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Garawell.Managers;
using Garawell.Managers.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nine
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private Camera _gameCamera;
        [SerializeField] private float _pickupOffset = 1f;
        [SerializeField] private float _hoverOffset = 0.1f;
        [SerializeField] private float _cameraHoverOffset = 2f;
        [SerializeField] private float _pickupMinDelta = 1f;
        [SerializeField] private float _inventoryScale = 0.85f;
        [SerializeField] private float _itemExpandDuration = 0.15f;
        [SerializeField] private List<ShapeItem> _itemPrefabs;
        [SerializeField] private List<Transform> _benchSlots;
        [SerializeField] private List<Transform> _queueSlots;
        [SerializeField] private bool _debugMode;
        [SerializeField] private Transform _itemPoolParent;
        [SerializeField] private Transform _itemHighlightParent;
        [SerializeField] private Transform _powerUpParent;

        public bool InputEnabled;
        [HideInInspector]
        public bool Finished;
        
        public List<ShapeItem> BenchItems => _benchItems;
        
        private List<ShapeItem> _itemPool;
        private List<ShapeItem> _availableItems;
        public List<ShapeItem> _benchItems;
        private List<PowerUpItem> _powerUpItems;
        
        private Transform _cameraPivot;
        private LevelData _levelData;
        private Dictionary<int, int> _colorWeights;
        private Dictionary<ShapeType, int> _shapeWeights;
        private int _totalNumberChance;
        private int _totalShapeChance;
        private int _spawnCount;
        
        private ShapeItem _selectedItem;
        private ShapeItem _highLightItem;
        private PowerUpItem _powerUpItem;
        private Vector3 _initialPos;
        private Vector3 _hitOffset;
        private Vector3 _pivotOffset;
        private Vector3 _prevPosition;
        private Vector3 _currentPosition;
        
        private LayerMask _inventoryMask;
        private LayerMask _gridMask;
        private LayerMask _bgMask;
        private LayerMask _lockMask;
        private LayerMask _powerUpMask;
        private LayerMask _shapeMask;

        private bool _checkedForSpace;
        private bool _draggable;
        private bool _moved;
        private bool _droppable;
        
        private GridController _gridController;
        
        private Coroutine _availabilityRoutine;
        private Coroutine _checkItemRoutine;
        private Tweener _offsetTween;


        #region Public Methods

        public void Initialize(LevelData levelData)
        {
            var level = PlayerPrefs.GetInt("Level", 0);
            _levelData = levelData;
            _colorWeights = new Dictionary<int, int>();
            _totalNumberChance = 0;
            for (int n = 0; n < _levelData.NumberData.Count; n++)
            {
                if (_levelData.NumberData[n].Chance <= 0)
                    continue;
                
                _colorWeights.Add(_levelData.NumberData[n].Number, _levelData.NumberData[n].Chance);
                _totalNumberChance += _levelData.NumberData[n].Chance;
            }
            
            _shapeWeights = new Dictionary<ShapeType, int>();
            _totalShapeChance = 0;
            for (int s = 0; s < _levelData.ShapeData.Count; s++)
            {
                if (_levelData.ShapeData[s].Chance <= 0) 
                    continue;
                
                _shapeWeights.Add(_levelData.ShapeData[s].Shape, _levelData.ShapeData[s].Chance);
                _totalShapeChance += _levelData.ShapeData[s].Chance;
            }
            

            _gridController = NineManager.Instance.GridController;
            _benchItems = new List<ShapeItem>();
            
            _inventoryMask = LayerMask.GetMask("Inventory");
            _lockMask = LayerMask.GetMask("GridLock");
            _gridMask = LayerMask.GetMask("Grid");
            _bgMask = LayerMask.GetMask("Background");
            _powerUpMask = LayerMask.GetMask("PowerUp");
            _shapeMask = LayerMask.GetMask("Shape");
            
            _checkedForSpace = false;
            _draggable = true;
            _droppable = false;
            _moved = false;
            Finished = false;
            
            _benchItems = new List<ShapeItem>{null, null, null};

            _cameraPivot = _gameCamera.transform.parent.parent;

            if (_itemPool == null)
            {
                int id = 0;
                _itemPool = new List<ShapeItem>();
                var scale = NineManager.Instance.GridController.transform.localScale * 1.05f;
                for (int i = 0; i < _itemPrefabs.Count; i++)
                {
                    for (int r = 0; r < 4; r++)
                    {
                        var itemToSpawn = _itemPrefabs[i];
                        if (r > 0 && itemToSpawn.Symmetry is ShapeItem.SymmetryMode.Full
                            || r > 1 && itemToSpawn.Symmetry is ShapeItem.SymmetryMode.Half)
                            break;
                        
                        var item = Instantiate(itemToSpawn, _itemPoolParent);
                        item.gameObject.name = $"{itemToSpawn.name}_r{r}_c({item.Complexity})_{item.ID}";
                        item.transform.localPosition = new Vector3(4f * r, -4f * i, 0f);
                        item.transform.localScale = scale;
                        item.Flip.rotation = Quaternion.Euler(0f, 0f, r*90f);
                        item.ResetNumberRotations();
                        
                        item.SetInfo(id++, itemToSpawn.Shape, r);
                        _itemPool.Add(item);
                    }
                }
            }
            _availableItems = new List<ShapeItem>(_itemPool.Count);
            
            _powerUpItems = _powerUpParent.GetComponentsInChildren<PowerUpItem>().ToList();
            foreach (var pItem in _powerUpItems)
            {
                if(level + 1 < pItem.UnlockLevel)
                    pItem.gameObject.SetActive(false);
            }

            if (NineManager.Instance.TutorialMode)
            {
                var item = _itemPool.First(it => it.Shape == ShapeType.One);
                var newItem = Instantiate(item, item.transform.position, item.transform.rotation, transform);
                newItem.SetInfo(item.ID, item.Shape, item.RotationMode);
                newItem.transform.localScale = Vector3.one;
                foreach (var t in newItem.Tiles)
                {
                    t.SetColor(ColorCode.Two);
                }
                
                _gridController.AddShape(newItem,
                    _gridController.Elements[_gridController.GetIndex(1, 1)].transform.position);
                
                _gridController.LockAll();
                _powerUpParent.gameObject.SetActive(false);
            }
            
            MainManager.Instance.EventManager.Register(EventTypes.OnHoldStart, OnHoldStart, true);
            MainManager.Instance.EventManager.Register(EventTypes.OnHoldFinish, OnHoldFinish, true);
            MainManager.Instance.EventManager.Register(EventTypes.OnShapeAdded, OnShapeAdded, true);
        }

        private void OnShapeAdded(EventArgs args)
        {
            UpdateItems();
        }

        public void GenerateItems()
        {
            if (NineManager.Instance.TutorialMode)
            {
                GenerateTutorialItem();
                return;
            }
            
            for (int i = 0; i < _benchSlots.Count; i++)
            {
                if(NineManager.Instance.TutorialMode && i != 1)
                    continue;
                
                GenerateItem(i);
            }
        }

        public bool GenerateTutorialItem()
        {
            var shapes = NineManager.Instance.TutorialShapes;
            if (_spawnCount >= shapes.Count)
                return false;
            
            var s = shapes[_spawnCount].Item1;
            var n = shapes[_spawnCount].Item2;
            var r = shapes[_spawnCount].Item3;

            switch (_spawnCount)
            {
                case 0:
                    _gridController.Unlock(2,1);
                    break;
                case 1:
                    _gridController.Unlock(2,2);
                    break;
                case 2:
                    _gridController.Unlock(1,2);
                    break;
            }
            
            if (_benchItems[1])
            {
                Destroy(_benchItems[1].gameObject);
            }

            var itemPrefab = _itemPool.FirstOrDefault(it => it.Shape == s && it.RotationMode == r);
            if (!itemPrefab)
                return false;
            
            var item = Instantiate(itemPrefab, _benchSlots[1]);
            
            item.InventoryOffset = item.GetBottomOffset() * Vector3.up;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.zero;
            item.Pivot.localScale = _inventoryScale * Vector3.one;
            item.SetInfo(itemPrefab.ID, itemPrefab.Shape, itemPrefab.RotationMode);
            item.gameObject.name = $"{item.Shape}_r{item.RotationMode}_({item.Color})_{item.ID}";
            item.gameObject.layer = LayerMask.NameToLayer("Inventory");
                
            for (int t = 0; t < item.Tiles.Length; t++)
            {
                item.Tiles[t].SetColor((ColorCode)n);
            }
            
            item.transform.DOScale(Vector3.one, _itemExpandDuration).SetEase(Ease.Linear);
                
            _benchItems[1] = item;
            return true;
        }

        public bool GenerateItem(int index)
        {
            var shape = GetRandomShapeFromData();
            var itemList = _itemPool.Where(it => it.Shape == shape).ToList();
            if (itemList.Count == 0)
            {
                Debug.LogError($"No Items available of shape {shape}");
                return false;
            }
            
            if (_benchItems[index])
            {
                Destroy(_benchItems[index].gameObject);
            }

            var itemPrefab = itemList[Random.Range(0, itemList.Count)];
            var item = Instantiate(itemPrefab, _benchSlots[index]);
            
            item.InventoryOffset = item.GetBottomOffset() * Vector3.up;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.zero;
            item.Pivot.localScale = _inventoryScale * Vector3.one;
            item.SetInfo(itemPrefab.ID, itemPrefab.Shape, itemPrefab.RotationMode);
            item.gameObject.name = $"{item.Shape}_r{item.RotationMode}_({item.Color})_{item.ID}";
            item.gameObject.layer = LayerMask.NameToLayer("Inventory");
                
            for (int t = 0; t < item.Tiles.Length; t++)
            {
                item.Tiles[t].SetColor(GetRandomColorFromData(item, t));
            }
            
            item.transform.DOScale(Vector3.one, _itemExpandDuration).SetEase(Ease.Linear);
                
            _benchItems[index] = item;
            return true;
        }

        public bool GenerateAvailableItem(int index)
        {
            if (_availableItems.Count == 0)
                return false;
            
            if (!GetRandomAvailableShape(out var shape))
                return false;
            
            var itemList = _availableItems.Where(it => it.Shape == shape).ToList();
            if (itemList.Count == 0)
            {
                Debug.LogError($"No available items found of shape {shape}");
                return false;
            }
            
            if (_benchItems[index])
            {
                Destroy(_benchItems[index].gameObject);
            }

            var itemPrefab = itemList[Random.Range(0, itemList.Count)];
            var item = Instantiate(itemPrefab, _benchSlots[index]);
            
            item.InventoryOffset = item.GetBottomOffset() * Vector3.up;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.zero;
            item.Pivot.localScale = _inventoryScale * Vector3.one;
            item.SetInfo(itemPrefab.ID, itemPrefab.Shape, itemPrefab.RotationMode);
            item.gameObject.name = $"{item.Shape}_r{item.RotationMode}_({item.Color})_{item.ID}";
            item.gameObject.layer = LayerMask.NameToLayer("Inventory");
            
            for (int t = 0; t < item.Tiles.Length; t++)
            {
                item.Tiles[t].SetColor(GetRandomColorFromData(item, t));
            }

            item.transform.DOScale(Vector3.one, _itemExpandDuration).SetEase(Ease.Linear);
            
            _benchItems[index] = item;
            
            return true;
        }
        
        public void ResetItems()
        {
            for(int i = 0; i < _benchItems.Count; i++)
            {
                var item = _benchItems[i];
                if(!item)
                    continue;
                
                item.ToggleShadow(true);
                _offsetTween.Kill();
                item.Pivot.DOKill();
                item.Pivot.DOScale(_inventoryScale, 0.05f);
                item.transform.localScale = Vector3.one;
                item.ShapeTransform.localPosition = Vector3.zero;
                item.transform.DOKill();
                item.transform.DOMove(_benchSlots[i].position + item.GetBottomOffset() * Vector3.forward, 0.05f);

                _selectedItem = null;
            }
        }
        
        public void OnFinish(bool success)
        {
            InputEnabled = false;
            _powerUpParent.gameObject.SetActive(false);
            
            if (!success)
                return;
            
            gameObject.SetActive(false);
            _cameraPivot.DORotate(new Vector3(0f, 0f, 45f), 1f, RotateMode.Fast)
                .SetEase(Ease.Linear);
            DOTween.To(() => _gameCamera.orthographicSize, x => _gameCamera.orthographicSize = x, 1f, 1f)
                .SetRelative().SetEase(Ease.Linear);
        }

        #endregion


        #region Unity Methods

        private void Update()
        {
            OnDragged(Input.mousePosition);
            
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha2))
                AddTile(2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                AddTile(3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                AddTile(4);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                AddTile(5);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                AddTile(6);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                AddTile(7);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                AddTile(8);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                AddTile(9);
#endif
        }

        #endregion
        
        #region Input Listeners
        private void OnHoldStart(EventArgs args)
        {
            if (!InputEnabled || !_draggable)
                return;

            _moved = false;
            _droppable = false;
            
            var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

            if (_powerUpItem)
            {
                if(Physics.Raycast(ray, out var sHit, 100f, _shapeMask) &&
                    sHit.collider.TryGetComponent<ShapeTile>(out var tile))
                {
                    if (_gridController.PowerUpItem(tile, _powerUpItem.PowerUpType))
                    {
                        EventRunner.SpendCurrency(0, _powerUpItem.Price, false);
                        Taptic.Light();
                    }
                    else
                    {
                        Taptic.Heavy();
                    }


                    _powerUpItem.Selected = false;
                    _powerUpItem = null;
                }
                else
                {
                    _powerUpItem.Selected = false;
                    _powerUpItem = null;
                }
            }


            if (Physics.Raycast(ray, out var pHit, 100f, _powerUpMask)
                && pHit.collider.TryGetComponent<PowerUpItem>(out var powerUp))
            {
                if (powerUp.Available)
                {
                    _powerUpItem = powerUp;
                    _powerUpItem.Selected = true;
                    Taptic.Light();
                }
                else
                {
                    Taptic.Heavy();
                }

                return;
            }
            
            if (Physics.Raycast(ray, out var hit, 100f, _inventoryMask))
            {
                _powerUpItem = null;
                var item = hit.collider.attachedRigidbody.GetComponent<ShapeItem>();
                if (!item || !_benchItems.Contains(item))
                    return;
                
                _prevPosition = Input.mousePosition;

                if (_highLightItem != null && _highLightItem.ID != item.ID)
                {
                    Destroy(_highLightItem.gameObject);
                    _highLightItem = Instantiate(item, _itemHighlightParent);
                    _highLightItem.SetInfo(item.ID, item.Shape, item.RotationMode);
                }
                
                _checkedForSpace = false;
                _selectedItem = item;
                var itemTransform = _selectedItem.transform;
                var itemPos = itemTransform.position;
                
                _initialPos = itemPos;
                
                _gridController.SetHighlightedItem(_selectedItem);
                
                _hitOffset = itemPos - (Vector3)hit.point;
                MainManager.Instance.AudioManager.PlayAudio(AudioID.ShapeSelect);
                
                _selectedItem.ToggleShadow(false);
                _selectedItem.Pivot.DOKill();
                _selectedItem.Pivot.DOScale(1.05f * _gridController.transform.localScale, 0.05f);
                _offsetTween.Kill();
                _pivotOffset = Vector3.zero;
                var offset = (1f + item.GetBottomOffset()) * Vector3.up;
                item.ShapeTransform.localPosition = -_cameraHoverOffset * item.ShapeTransform.InverseTransformDirection(_gameCamera.transform.forward);
                
                MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnShapePickedUp);
                _offsetTween = DOTween.To(() => _pivotOffset, x => _pivotOffset = x,
                        offset, 0.06f)
                    .OnComplete(() =>
                    {
                        _droppable = true;
                        Taptic.Medium();
                    });
                
                //Debug.LogWarning($"Item Selected: {_selectedItem}");
            }
        }
        
        private void OnDragged(Vector3 position)
        {
            if (!InputEnabled || !_selectedItem)
                return;
            
            var newPos = _prevPosition + 1.2f * (position - _prevPosition);

            _moved = true;
            var bgRay = _gameCamera.ScreenPointToRay(newPos);
            if (!Physics.Raycast(bgRay, out var hit, 100f, _bgMask))
                return;

            var worldPos = hit.point;
            worldPos.z = transform.position.z + _hoverOffset;
            var itemPos = worldPos + _hitOffset + _pivotOffset;
            _selectedItem.transform.position = itemPos;
            
            var rayPos = _gameCamera.WorldToScreenPoint(_selectedItem.Pivot.position);
            var ray = _gameCamera.ScreenPointToRay(rayPos);
            if(_droppable && !_gridController.Updating && 
               Physics.Raycast(ray, out var gridHit, 100f, _gridMask))
            {
                _gridController.HighlightShape(_selectedItem, gridHit.point);
            }
            else
            {
                _gridController.ResetHighlight();
            }
        }
        
        private void OnHoldFinish(EventArgs args)
        {
            if (!InputEnabled || !_selectedItem)
                return;
            
            _draggable = false;
            //Debug.Log("Draggable = false");
            
            var itemTransform = _selectedItem.transform;
            var dist = Vector3.Distance(_initialPos, itemTransform.position);

            var rayPos = _gameCamera.WorldToScreenPoint(_selectedItem.Pivot.position);

            var ray = _gameCamera.ScreenPointToRay(rayPos);
            
            if(_droppable && _moved && dist > 0.1f && 
               Physics.Raycast(ray, out var gridHit, 100f, _gridMask)
               && _gridController.AddShape(_selectedItem, gridHit.point))
            {
                var index = _benchItems.FindIndex(b => b == _selectedItem);
                _benchItems[index] = null;
                MainManager.Instance.AudioManager.PlayAudio(AudioID.ShapeAdd);
                Taptic.Light();
                Destroy(_selectedItem.gameObject);
                _spawnCount++;
            }
            else
            {
                _selectedItem.ToggleShadow(true);
                _offsetTween.Kill();
                _selectedItem.Pivot.DOKill();
                _selectedItem.Pivot.DOScale(_inventoryScale, 0.05f);
                _selectedItem.ShapeTransform.localPosition = Vector3.zero;
                itemTransform.DOKill();
                itemTransform.DOMove(_initialPos, 0.05f)
                    .OnComplete(() =>
                    {
                        MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnShapeReturned);
                        _draggable = true;
                    });
            }
            
            _droppable = false;
            _checkedForSpace = false;
            _selectedItem = null;
        }

        #endregion

        private ColorCode GetRandomColorFromData(ShapeItem item, int slotIndex)
        {
            var colorWeights = new Dictionary<int, int>(_colorWeights);
            for (var i = 2; i <= item.Slots.Length; i++)
            {
                if (item.GetNumberCountAdjacent(slotIndex, i) >= (i-1))
                    colorWeights.Remove(i);
            }

            var total = colorWeights.Values.Sum();
                
            var roll = Random.Range(0, total);
            var sum = 0;
            foreach (var c in colorWeights)
            {
                sum += c.Value;
                
                if (roll < sum)
                    return (ColorCode)c.Key;
            }
            return (ColorCode)_colorWeights.Keys.ToList()[Random.Range(0, _colorWeights.Count)];
        }

        private ShapeType GetRandomShapeFromData()
        {
            var roll = Random.Range(0, _totalShapeChance);
            var sum = 0;
            foreach (var s in _shapeWeights)
            {
                sum += s.Value;
                if (roll < sum)
                    return s.Key;
            }
            throw new InvalidOperationException("Shape weight not found");
        }

        private bool GetRandomAvailableShape(out ShapeType shape)
        {
            shape = ShapeType.One;
            var availableWeights = new Dictionary<ShapeType, int>();
            foreach (var w in _shapeWeights)
            {
                if(_availableItems.Any(i => i.Shape == w.Key))
                    availableWeights.Add(w.Key, w.Value);
            }

            if (availableWeights.Count == 0)
                return false;

            var totalChance = availableWeights.Sum(w => w.Value);
            var roll = Random.Range(0, totalChance);
            var sum = 0;
            foreach (var a in availableWeights)
            {
                sum += a.Value;
                if (roll < sum)
                {
                    shape = a.Key;
                    return true;
                }
            }
            throw new InvalidOperationException("Shape weight not found");
        }

        private void CheckAvailableItems()
        {
            _availableItems.Clear();
            foreach (var item in _itemPool)
            {
                if(_gridController.CheckForSpace(item))
                    _availableItems.Add(item);
            }
        }
        
        private void AddTile(int i)
        {
            if (i is < 2 or > 9 || _gridController.Updating)
                return;
            
            var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);
            if(Physics.Raycast(ray, out var gridHit, 100f, _gridMask))
            {
                _gridController.AddTile(i, gridHit.point);
            }
        }

        private void UpdateItems()
        {
            if (Finished)
                return;

            if (NineManager.Instance.TutorialMode)
            {
                GenerateTutorialItem();
                _draggable = true;
                return;
            }
            
            if (!_benchItems.Any(b => b))
            {
                CheckAvailableItems();
                for (int b = 0; b < _benchItems.Count; b++)
                {
                    var roll = Random.Range(0f, 1f);
                    if (roll < NineManager.Instance.SuggestChance)
                    {
                        if (!GenerateAvailableItem(b))
                            GenerateItem(b);
                    }
                    else
                        GenerateItem(b);
                }
            }
            
            _draggable = true;
        }

        public void CheckItems()
        {
            if (NineManager.Instance.TutorialMode)
                return;
            
            if (_benchItems.Count(b => b) > 0)
            {
                var availableAny = false;
                var log = "Item Check Status: ";
                foreach(var item in _benchItems)
                {
                    if (!item)
                        continue;
                    
                    var itemToCheck = _itemPool.FirstOrDefault(i => i.ID == item.ID);
                    var available = _gridController.CheckForSpace(itemToCheck);
                    availableAny |= available;
                    log += $"{item.name}-{available}, ";
                }
                
                Debug.LogWarning(log);

                if (!availableAny)
                {
                    _selectedItem = null;
                    Debug.LogWarning("No Space Available");
                    NineManager.Instance.FinishGame(false);
                    return;
                }
                return;
            }
            return;
        }

        /*private void GenerateRearItem(int index)
        {
            var shape = GetRandomShapeFromData();
            var itemList = _itemPool.Where(it => it.Shape == shape).ToList();
            if (itemList.Count == 0)
            {
                Debug.LogError($"No Items available of shape {shape}");
                return;
            }

            var itemPrefab = itemList[Random.Range(0, itemList.Count)];
            
            //Debug.LogWarning($"Rear item index = {index}");
                    
            var item = Instantiate(itemPrefab, _queueSlots[index]);
            item.InventoryOffset = item.GetBottomOffset() * Vector3.up;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.Pivot.localScale = _inventoryScale * Vector3.one;
            item.SetInfo(itemPrefab.ID, itemPrefab.Shape, itemPrefab.RotationMode);
            item.gameObject.name = $"{item.Shape}_r{item.RotationMode}_({item.Color})_{item.ID}";
            item.gameObject.layer = LayerMask.NameToLayer("Inventory");
            
            for (int t = 0; t < item.Tiles.Length; t++)
            {
                item.Tiles[t].SetColor(GetRandomColorFromData(item));
            }
            
            _rearItems[index] = item;
        }*/
    }
}

