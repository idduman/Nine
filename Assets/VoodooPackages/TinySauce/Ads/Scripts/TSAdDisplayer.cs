using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Voodoo.Tiny.Sauce.Internal.Analytics;
using Random = UnityEngine.Random;

namespace Voodoo.Tiny.Sauce.Internal.Ads
{
    public class TSAdDisplayer : MonoBehaviour
    {
        [SerializeField] private TSAdImageBehaviour adImage;
        [SerializeField] private TsAdAnimationBehavior adAnimation;
        [SerializeField] private Button closeBtn;

        
        private static TSAdDisplayer _instance;
        internal static TSAdDisplayer Instance { get => _instance; }
        
        private static TSAdData _currentAdData;
        private static TSFakeAdContent[] adList;

        
        private const string CLOSE_BTN_TXT = "X";
        private const float DEFAULT_MIN_CLOSE_TIME_INTERSTITIAL = 5;
        private const float DEFAULT_MIN_CLOSE_TIME_RV = 30;
        private static float minCloseTimeInterstitial = -1;
        private static float minCloseTimeRV = -1;

        
        private EventSystem eventSystemPrefab;
        private EventSystem eventSystem;

        private bool isAppPaused = false;
        private bool isFirstFrameAfterPause = false;
        
        private float gameTimeScale;

        private Text closeBtnText;
        private float minCloseTimeCurrentAd;
        private float cooldown;
        
        internal Action On_AdClosed;

        
        internal static TSAdData CurrentAdData { set => _currentAdData = value; }
        

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            InitCloseBtn();
            InitAdList();
            InitEventSystem();
        }

        private void Start()
        {
            gameTimeScale = Time.timeScale;
            Time.timeScale = 0;

            SetupFakeAd();
        }

        private void Update()
        {
            if (cooldown > 0)
                UpdateCloseBtn();
            else
                EnableCloseBtn();
        }

        private void OnDestroy()
        {
            _instance = null;
            Time.timeScale = gameTimeScale;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            isAppPaused = !hasFocus;

            if (isAppPaused) isFirstFrameAfterPause = true;
        }


#region [ADS]
        private void InitAdList()
        {
            if (adList == null)
                adList = Resources.LoadAll<TSFakeAdContent>("FakeAds");
        }

        private void SetupFakeAd()
        {
            if (adList != null && adList.Length > 0)
            {
                _currentAdData.fakeAdContent = adList[Random.Range(0, adList.Length)].Load();
                InitAdInfo();
            }
            else
            {
                Debug.LogError("There are no fake ads in the 'Assets/VoodooPackages/TinySauce/Ads/Resources/FakeAds' folder");
                CloseAd();
            }
        }
        
        private void InitAdInfo()
        {
             adImage.CurrentAdData = _currentAdData;

            switch (_currentAdData.adType)
            {
                case TSAdType.Fake_Interstitial:
                    AnalyticsManager.TrackInterstitialShow(new AdShownEventAnalyticsInfo {
                        AdTag = _currentAdData.adType.ToString(),
                        AdNetworkName = _currentAdData.adSdkName,
                        adPlacement = _currentAdData.adPlacement,
                        GameCount = AnalyticsStorageHelper.GetGameCount()
                    });
                    break;
                case TSAdType.Fake_RewardedVideo:
                    AnalyticsManager.TrackRewardedShow(new AdShownEventAnalyticsInfo {
                        AdTag = _currentAdData.adType.ToString(),
                        AdNetworkName = _currentAdData.adSdkName,
                        adPlacement = _currentAdData.adPlacement,
                        GameCount = AnalyticsStorageHelper.GetGameCount()
                    });
                    break;
            }
            
            adAnimation.SetPosition(_currentAdData.fakeAdContent.position);
            
            Debug.Log($"Pew! Pretending to send a GameAnalytics AdEvent because it doesn't actually work in the editor (AdShown event, AdType '{_currentAdData.adType.ToString()}', AdPlacement '{_currentAdData.adPlacement}')");
        }
#endregion []

#region [CLOSE_BUTTON]
        private void InitCloseBtn()
        {
            if(_currentAdData.adType == TSAdType.Fake_Interstitial)
                closeBtn.onClick.AddListener(ShowNextScreen);
            else
                closeBtn.onClick.AddListener(CloseAd);
            closeBtnText = closeBtn.GetComponentInChildren<Text>();

            if (minCloseTimeInterstitial < 0) 
                minCloseTimeInterstitial = DEFAULT_MIN_CLOSE_TIME_INTERSTITIAL;
            if (minCloseTimeRV < 0) 
                minCloseTimeRV = DEFAULT_MIN_CLOSE_TIME_RV;
            
            switch (_currentAdData.adType)
            {
                case TSAdType.Fake_Interstitial:
                    minCloseTimeCurrentAd = minCloseTimeInterstitial;
                    break;
                case TSAdType.Fake_RewardedVideo:
                    minCloseTimeCurrentAd = minCloseTimeRV;
                    break;
            }

            closeBtn.enabled = false;
            cooldown = minCloseTimeCurrentAd;
            closeBtnText.text = minCloseTimeCurrentAd.ToString();
        }
        
        private void UpdateCloseBtn()
        {
            closeBtnText.text = Mathf.Ceil(cooldown).ToString();
            
            if (!isAppPaused)
            {
                if (isFirstFrameAfterPause)
                    isFirstFrameAfterPause = false;
                else
                    cooldown -= Time.unscaledDeltaTime;
            }
        }
        
        private void EnableCloseBtn()
        {
            closeBtnText.text = CLOSE_BTN_TXT;
            closeBtn.enabled = true;
        }
        
        internal static void SetAdCloseButtonCooldown(float interstitialCloseCooldown, float rvCloseCooldown)
        {
            minCloseTimeInterstitial = interstitialCloseCooldown;
            minCloseTimeRV = rvCloseCooldown;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only call this method inside a "#if UNITY_EDITOR" preprocessor directive
        /// </summary>
        internal void SkipCloseButtonCooldown()
        {
            cooldown = 0;
        }
#endif
#endregion []

        
        private  void InitEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            
            if (eventSystemPrefab == null)
                eventSystemPrefab = Resources.LoadAll<EventSystem>("Prefabs")[0];
                    
            if (eventSystemPrefab == null)
                Debug.LogError("There is no TSEventSystem prefab in the 'Assets/VoodooPackages/TinySauce/Resources/Prefabs' folder");

            eventSystem = Instantiate(eventSystemPrefab);
        }

        private void ShowNextScreen()
        {
            adImage.NextScreen();
            
            adAnimation.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(CloseAd);
            closeBtnText = closeBtn.GetComponentInChildren<Text>();

            if (minCloseTimeInterstitial < 0) 
                minCloseTimeInterstitial = DEFAULT_MIN_CLOSE_TIME_INTERSTITIAL;
            if (minCloseTimeRV < 0) 
                minCloseTimeRV = DEFAULT_MIN_CLOSE_TIME_RV;
            
            switch (_currentAdData.adType)
            {
                case TSAdType.Fake_Interstitial:
                    minCloseTimeCurrentAd = minCloseTimeInterstitial;
                    break;
                case TSAdType.Fake_RewardedVideo:
                    minCloseTimeCurrentAd = minCloseTimeRV;
                    break;
            }

            closeBtn.enabled = false;
            cooldown = minCloseTimeCurrentAd;
            closeBtnText.text = minCloseTimeCurrentAd.ToString();
        }

        private void CloseAd()
        {
            closeBtn.onClick.RemoveAllListeners();
            
            if (eventSystem != null)
                Destroy(eventSystem.gameObject);
            
            On_AdClosed?.Invoke();
            Destroy(gameObject);
        }
    }
}
