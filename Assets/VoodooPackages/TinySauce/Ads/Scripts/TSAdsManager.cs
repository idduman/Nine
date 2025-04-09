using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Tiny.Sauce.Internal.Ads
{
    public class TSAdsManager : MonoBehaviour
    {
        private static bool _areAdsEnabled = true;
        private static bool hasTSAdsManagerBeenAddedToScene = false;
        
        private static TSAdDisplayer adDisplayerPrefab;
        
        private static TSAdDisplayer adDisplayer;
        private static TSFSDisplayConditions fsDisplayConditions;
        
        private static TSAdData _currentAdData;
        
        private const string AD_SDK_NAME = "vdfats";
        
        private const string MISSING_SCRIPT_ERROR = "TSAdsManager is missing, please add it to the TinySauce prefab in your first scene";
        
        public static bool AreAdsEnabled { get => _areAdsEnabled; }


        private void Awake()
        {
            hasTSAdsManagerBeenAddedToScene = true;
            
            InitAdDisplayerPrefab();
            InitFSDisplayConditions();
            _currentAdData = new TSAdData();
        }

        
#region [PUBLIC_FUNCTIONS]
        /// <summary>
        /// Enables or disables TinySauce Ads.
        /// </summary>
        /// <param name="areAdsEnabled">Whether you want to enable or disable ads</param>
        public static void ToggleAds(bool areAdsEnabled)
        {
            CheckIfInstantiated();
            
            _areAdsEnabled = areAdsEnabled;
        }

        /// <summary>
        /// Set custom conditions to display Interstitial ad.
        /// </summary>
        /// <param name="delayBeforeFirstInterstitial">Delay before the very first Interstitial can be displayed after installing the game - in seconds</param>
        /// <param name="delayBetweenInterstitials">Delay between 2 Interstitials (also depends on maxLevelsBetweenInterstitials) - in seconds</param>
        /// <param name="maxLevelsBetweenInterstitials">Number of levels played before an Interstitial can be displayed again (also depends on delayBetweenInterstitials)</param>
        /// <param name="delayBetweenRewardedVideoAndInterstitial">Delay before an Interstitial can be displayed after displaying a Rewarded Video - in seconds</param>
        public static void SetFSDisplayConditions(
            int delayBeforeFirstInterstitial,
            int delayBetweenInterstitials,
            int maxLevelsBetweenInterstitials,
            int delayBetweenRewardedVideoAndInterstitial = -1)
        {
            CheckIfInstantiated();
            
            fsDisplayConditions = new TSFSDisplayConditions(
                delayBeforeFirstInterstitial,
                delayBetweenInterstitials, 
                maxLevelsBetweenInterstitials,
                delayBetweenRewardedVideoAndInterstitial);
        }

        /// <summary>
        /// Set custom cooldown before an ad can be closed
        /// </summary>
        /// <param name="interstitialCloseCooldown">Cooldown before an interstitial can be closed</param>
        /// <param name="rvCloseCooldown">Cooldown before a rewarded video can be closed</param>
        public static void SetAdCloseButtonCooldown(float interstitialCloseCooldown, float rvCloseCooldown)
        {
            CheckIfInstantiated();
            
            TSAdDisplayer.SetAdCloseButtonCooldown(interstitialCloseCooldown, rvCloseCooldown);
        }

        /// <summary>
        /// Display an Interstitial ad if the conditions are met.
        /// </summary>
        /// <param name="onAdClosed">Callback when an ad is closed</param>
        /// <param name="adPlacement">Location and/or purpose of the ad - MAX 64 CHAR - e.g. "between-levels" or "end-level-bonus-coins"</param>
        public static void ShowInterstitial(Action onAdClosed = null, string adPlacement = "not-specified")
        {
            CheckIfInstantiated();
            
            if (_areAdsEnabled && AreFSDisplayConditionsMet())
            {
                OpenAdDisplayer(onAdClosed, TSAdType.Fake_Interstitial, adPlacement);
                fsDisplayConditions.InterstitialDisplayed();
            }
        }
        
        /// <summary>
        /// Display a Rewarded Video ad if the conditions are met.
        /// </summary>
        /// <param name="onAdClosed">Callback when an ad is closed (should be used to give a reward after a Rewarded Video)</param>
        /// <param name="adPlacement">Location and/or purpose of the ad - MAX 64 CHAR - e.g. "between-levels" or "end-level-bonus-coins"</param>
        public static void ShowRewardedVideo(Action onAdClosed, string adPlacement = "not-specified")
        {
            CheckIfInstantiated();
            
            if (_areAdsEnabled)
            {
                OpenAdDisplayer(onAdClosed, TSAdType.Fake_RewardedVideo, adPlacement);
                fsDisplayConditions.RVDisplayed();
            }
        }

        /// <summary>
        /// This method will only skip the CloseButton cooldown inside the Unity Editor
        /// </summary>
        public static void SkipCloseButtonCooldown()
        {
#if UNITY_EDITOR
            CheckIfInstantiated();
            
            adDisplayer.SkipCloseButtonCooldown();
#endif
        }
#endregion

#region [ADS]
        private static void CheckIfInstantiated()
        {
            if (!hasTSAdsManagerBeenAddedToScene)
            {
#if UNITY_EDITOR
                EditorApplication.isPaused = true;
#endif
                throw new Exception(MISSING_SCRIPT_ERROR);
            }
        }
        
        private void InitAdDisplayerPrefab()
        {
            if (adDisplayerPrefab == null)
            {
                TSAdDisplayer[] adDisplayerList = Resources.LoadAll<TSAdDisplayer>("Prefabs");
                adDisplayerPrefab = adDisplayerList[0];
            }
            
            if (adDisplayerPrefab == null)
                throw new Exception("There is no AdDisplayer prefab in the 'Assets/VoodooPackages/TinySauce/Ads/Resources/Prefabs' folder, please try importing the .unitypackage again");
        }
        
        private void InitFSDisplayConditions()
        {
            fsDisplayConditions = new TSFSDisplayConditions();
        }

        private static void OpenAdDisplayer(Action onAdClosed, TSAdType adType, string adPlacement)
        {
            if (adDisplayerPrefab == null)
                throw new Exception($"{MISSING_SCRIPT_ERROR} -> adDisplayerPrefab = null");
            
            if (TSAdDisplayer.Instance == null)
            {
                InitAdInfo(adType, adPlacement);
                adDisplayer = Instantiate(adDisplayerPrefab);
                if (onAdClosed != null) adDisplayer.On_AdClosed += onAdClosed;
                else if (adType == TSAdType.Fake_RewardedVideo) Debug.LogError("You are showing a Rewarded Video Ad, but you did not implement any reward");
            }
        }
        
        private static bool AreFSDisplayConditionsMet()
        {
            if (fsDisplayConditions == null)
                throw new Exception($"{MISSING_SCRIPT_ERROR} -> fsDisplayConditions = null");
            
            return fsDisplayConditions.AreConditionsMet();
        }

        private static void InitAdInfo(TSAdType adType, string adPlacement)
        {
            _currentAdData.adType = adType;
            _currentAdData.adSdkName = AD_SDK_NAME;
            _currentAdData.adPlacement = adPlacement;
            TSAdDisplayer.CurrentAdData = _currentAdData;
        }
#endregion
    }
}