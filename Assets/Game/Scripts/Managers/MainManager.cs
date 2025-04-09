using Garawell.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using Garawell.Managers.Events;
using System.Collections;
using Nine;

namespace Garawell.Managers
{
    public class MainManager : MonoSingleton<MainManager>
    {   
        [SerializeField] private EditorGameSettings editorGameSettings;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private AssetManager assetManager;
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private SDKManager sdkManager;
        [SerializeField] private SettingsManager settingsManager;
        [SerializeField] private PoolingManager poolingManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private CurrencyDropManager currencyDropManager;
        [SerializeField] private NineManager nineManager;

        private MenuManager menuManager;
        private EventManager eventManager;
        private UtilityManager utilityManager;
        private RemoteConfigManager remoteConfigManager = new RemoteConfigManager();

        private string lastLoadedScene = "";
        private GameObject lastLoadedScenePrefab;
        
        public FloatingJoystick joystick;

        public MenuManager MenuManager { get => menuManager; set => menuManager = value; }
        public GameManager GameManager { get => gameManager; }
        public CurrencyManager CurrencyManager { get => currencyManager; }
        public CurrencyDropManager CurrencyDropManager { get => currencyDropManager; }
        public EventManager EventManager { get => eventManager; }
        public AssetManager AssetManager { get => assetManager; }
        public ShopManager ShopManager { get => shopManager; }
        public SDKManager SdkManager { get => sdkManager; }
        public SettingsManager SettingsManager { get => settingsManager; }
        public string LastLoadedScene { get => lastLoadedScene; set => lastLoadedScene = value; }
        public GameObject LastLoadedScenePrefab { get => lastLoadedScenePrefab; set => lastLoadedScenePrefab = value; }
        public RemoteConfigManager RemoteConfigManager { get => remoteConfigManager; }
        public EditorGameSettings EditorGameSettings { get => editorGameSettings; }
        public PoolingManager PoolingManager { get => poolingManager; }
        public AudioManager AudioManager { get => audioManager; }
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        public void Initialize()
        {
            Debug.Log("INIT");
            assetManager.Initialize();
            eventManager = new EventManager();
            eventManager.Initialize();
            
#if UNITY_EDITOR
            if (editorGameSettings.ActivateDebugger)
                Garawell.Utility.Debugger.ECDebugger.Initialize();
            
            utilityManager = new UtilityManager();
            utilityManager.Initialize();
#endif

            menuManager = FindObjectOfType<MenuManager>();
            currencyManager.Initialize();
            settingsManager.Initialize();
            menuManager.Initialize();
            sdkManager.Initialize();
            poolingManager.Initialize();
            audioManager.Initialize();
            gameManager.Initialize();
            inputManager.Initialize();
            nineManager.Initialize();
            SceneManager.LoadScene("LevelLoaderScene", LoadSceneMode.Additive);
        }
        public void UnloadScene(string sceneName)
        {
            StartCoroutine(UnloadRoutine(sceneName));
        }

        public IEnumerator UnloadRoutine(string sceneName)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.UnloadScene(sceneName);
        }
    }
}
