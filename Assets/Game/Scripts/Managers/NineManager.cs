using System;
using System.Collections.Generic;
using Garawell.Managers;
using Garawell.Managers.Events;
using Garawell.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Voodoo.Tiny.Sauce.Internal.Ads;

namespace Nine
{
    public class NineManager : MonoSingleton<NineManager>
    {
        [SerializeField] private LevelDataManager _levelDataManager;
        [SerializeField] private RemoteLevelDataHandler _remoteLevelDataHandler;
        [SerializeField] private GameObject _remoteLoadingCanvas;
        [SerializeField] private GameObject _shapeRenderCamera;
        [SerializeField] private ShapeTile _goalTile;
        public InventoryController InventoryController { get; private set; }
        public GridPanel GridPanel { get; private set; }
        public GridController GridController { get; private set; }
        public MergeMode MergeMode { get; private set; }
        public int ScoreToWin { get; private set; }
        public int NumberToWin { get; private set; }
        public float SuggestChance { get; private set; }
        public bool TutorialMode { get; private set; }
        
        public List<Tuple<ShapeType, int, int>> TutorialShapes {get; private set;}
        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                MainManager.Instance.MenuManager.GamePanel.SetScore(_score, ScoreToWin);
            }
        }
        
        private Coroutine _checkRoutine;

        private List<LevelData> _randomizedLevelData;


        private bool _finished;
        private bool _checkInventory;
        private bool _itemAvailable;
        
        private float _checkInventoryTimer;
        private int _score;

        private static readonly float MoveCheckTime = 0.1f;
        
        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                FinishGame(true);
            }
	        
            if (Input.GetKeyDown(KeyCode.L))
            {
                FinishGame(false);
            }
            
            if(Input.GetKeyDown(KeyCode.U))
            {
                GridController.UnlockAll();
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                EventRunner.LevelSuccess();
            }
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                EventRunner.EarnCurrency(0,200,false);
            }
        }
        #endif

        private void LateUpdate()
        {
            if (!GridController || !_checkInventory)
                return;
            
            if(_checkInventoryTimer > 0)
                _checkInventoryTimer -= Time.deltaTime;
            else
            {
                _checkInventoryTimer = MoveCheckTime;
                CheckInventory();
            }
            /*else if (_checkInventory && !_itemAvailable && GridController.Items.All(i => !i.AnimationPlaying && !i.CanAnimationPlaying && i.HasSpace))
                CheckInventory();*/
        }

        private void OnDestroy()
        {
            RemoteLevelDataHandler.DataLoaded -= OnRemoteLevelDataLoaded;
            
            if(_checkRoutine != null)
                StopCoroutine(_checkRoutine);
        }
        
        public void Initialize()
        {
            MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, OnLevelLoaded);
            MainManager.Instance.EventManager.Register(EventTypes.LevelStart, OnLevelStart);
            MainManager.Instance.EventManager.Register(EventTypes.LevelFinish, OnLevelFinished);
            MainManager.Instance.EventManager.Register(EventTypes.OnRvStart, OnRvStarted);
            MainManager.Instance.EventManager.Register(EventTypes.OnRvGranted, OnRvGranted);
            MainManager.Instance.EventManager.Register(EventTypes.OnRevive, OnRevive);
            MainManager.Instance.EventManager.Register(EventTypes.OnMoveCompleted, OnMoveCompleted);

            RemoteLevelDataHandler.DataLoaded += OnRemoteLevelDataLoaded;
            _remoteLoadingCanvas.SetActive(true);
            _remoteLevelDataHandler.Initialize();
        }
        
        public void PrevLevel()
        {
            var level = PlayerPrefs.GetInt("Level", 0);
            PlayerPrefs.SetInt("Level", Mathf.Max(level-1, 0));
            EventRunner.LevelRestart();
        }

        public void NextLevel()
        {
            var level = PlayerPrefs.GetInt("Level", 0);
            PlayerPrefs.SetInt("Level", level+1);
            EventRunner.LevelRestart();
        }
        
        public void AddScore(int score)
        {
            Score += score;
        }
        
        [Button]
        public void FinishGame(bool success)
        {
            if (_finished)
                return;

            _finished = true;
            InventoryController.OnFinish(success);
            GridController.OnFinish(success);
            if (success)
            {
                EventRunner.EarnCurrency(0, 50, false);
                EventRunner.LevelSuccess();
                MainManager.Instance.AudioManager.PlayAudio(AudioID.Win);
            }
            else
            {
                EventRunner.LevelFail();
            }
        }

        private void OnRemoteLevelDataLoaded(string s)
        {
            if (!TutorialMode)
                _remoteLoadingCanvas.SetActive(false);

            if (_remoteLevelDataHandler.GetDataFromServer(out var levelDataManager))
            {
                Debug.LogWarning("Remote data loaded");
                _levelDataManager = levelDataManager;
            }
            else
            {
                Debug.LogWarning("Remote data failed to load");
            }

        }

        private void OnLevelLoaded(EventArgs args)
        {
            _finished = false;
            TutorialMode = PlayerPrefs.GetInt("TutorialCompleted", 0) == 0;
            if(!TutorialMode)
                return;

            TutorialShapes = new List<Tuple<ShapeType, int, int>>();
            TutorialShapes.Add(new Tuple<ShapeType, int, int>(ShapeType.One, 2, 0));
            TutorialShapes.Add(new Tuple<ShapeType, int, int>(ShapeType.Two, 3, 1));
            TutorialShapes.Add(new Tuple<ShapeType, int, int>(ShapeType.SmallL, 4, 3));
        }

        private void OnLevelStart(EventArgs args)
        {
            var level = PlayerPrefs.GetInt("Level", 0);
            var levelCount = _levelDataManager.LevelDatas.Count;
            LevelData levelData = _levelDataManager.LevelDatas[level % levelCount];

            ScoreToWin = levelData.ScoreToWin == 0 ? 999999 : levelData.ScoreToWin;
            NumberToWin = levelData.NumberToWin == 0 ? 99 : levelData.NumberToWin;
            SuggestChance = Mathf.Clamp(levelData.SuggestChance / 10f, 0f, 1f);

            var gamePanel = MainManager.Instance.MenuManager.GamePanel;
            if (TutorialMode)
            {
                _shapeRenderCamera.SetActive(false);
               gamePanel.ToggleLevelPanel(false);
               gamePanel.ToggleCurrencyPanel(false);
                ScoreToWin = 999999;
                NumberToWin = 5;
            }
            else if (NumberToWin <= 9)
            {
                _shapeRenderCamera.SetActive(true);
                _goalTile.SetColor((ColorCode)NumberToWin);
                gamePanel.ToggleCurrencyPanel(true);
                gamePanel.ToggleLevelPanel(true);
                gamePanel.ToggleGoalPanel(true);
                gamePanel.ToggleScorePanel(false);
            }
            else
            {
                _shapeRenderCamera.SetActive(false);
                gamePanel.ToggleCurrencyPanel(true);
                gamePanel.ToggleLevelPanel(true);
                gamePanel.ToggleGoalPanel(false);
                gamePanel.ToggleScorePanel(true);
            }
            
            MainManager.Instance.MenuManager.GamePanel.ResetScore();
            Score = 0;

            /*if(level > _levelDataManager.LevelDatas.Count) 
            {
                _randomizedLevelData ??= _levelDataManager.LevelDatas.Shuffle();
                levelData = _randomizedLevelData[level % _randomizedLevelData.Count];
            }*/
            
            /*if (levelData.HardLevel)
            {
                var prefab = MainManager.Instance.AssetManager.GetPrefab("HardLevel");
                Instantiate(prefab, transform);
            }*/
            
            InventoryController = FindObjectOfType<InventoryController>();
            GridPanel = FindObjectOfType<GridPanel>();
            
            GridPanel.Initialize();
            GridPanel.SetActiveGrid(levelData.GridSize);
            GridController = GridPanel.ActiveGrid;
            GridController.Initialize();
            InventoryController.Initialize(levelData);
            
            InventoryController.GenerateItems();
            InventoryController.InputEnabled = true;

            for (int i = 0; i < 3; i++)
            {
                if (level == 2*i+2 && 
                    PlayerPrefs.GetInt($"PTutorialCompleted{i}", 0) == 0)
                {
                    EventRunner.PopupTutorial(i);
                    break;
                }
            }
            
            if(TutorialMode)
                _remoteLoadingCanvas.SetActive(false);
        }

        private void OnLevelFinished(EventArgs args)
        {
            Debug.LogWarning("Level Finished");
        }
        
        private void OnRevive(EventArgs arg0)
        {
            _finished = false;
            InventoryController.Finished = false;
            InventoryController.InputEnabled = true;
        }
        
        private void OnRvStarted(EventArgs args)
        {
            var rvArgs = args as RvArgs;
            if (rvArgs == null)
                return;
            
            TSAdsManager.ShowRewardedVideo(() => OnRvGranted(new RvArgs(rvArgs.RvType, rvArgs.Id)),
                rvArgs.RvType.ToString());
        }
        
        private void OnRvGranted(EventArgs args = null)
        {
            var rvArgs = args as RvArgs;
            Taptic.Light();
            switch (rvArgs?.RvType)
            {
                case RvType.Unlock:
                    GridController.UnlockRv(rvArgs.Id, true);
                    break;
                case RvType.Revive:
                    EventRunner.Revive();
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMoveCompleted(EventArgs args)
        {
            if (_finished)
                return;
            
            _checkInventoryTimer = MoveCheckTime;
            _checkInventory = true;
        }
        
        private void CheckInventory()
        {
            if (!GridController)
                return;
            
            _checkInventory = false;
            _checkInventoryTimer = MoveCheckTime;
            
            InventoryController.CheckItems();
            
            if(!_finished && Score >= ScoreToWin)
                FinishGame(true);
        }

        public void ToggleMergeMode()
        {
            MergeMode = MergeMode == MergeMode.Number ? MergeMode.Dual : MergeMode.Number;
        }
    }
}

