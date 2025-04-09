using System;
using UnityEngine;
using Voodoo.Tiny.Sauce.Internal.Analytics;

namespace Voodoo.Tiny.Sauce.Internal.Ads
{
    public class TSFSDisplayConditions
    {
        private const string PREFS_FIRST_APP_LAUNCH = "TinySauce.Interstitial.FirstAppLaunch";
        
        private const int DEFAULT_DELAY_BEFORE_FIRST_INTERSTITIAL = 30;
        private const int DEFAULT_DELAY_BETWEEN_INTERSTITIAL = 30;
        private const int DEFAULT_MAX_LEVELS_BETWEEN_INTERSTITIAL = 3;
        private const int DEFAULT_DELAY_BETWEEN_RV_AND_INTERSTITIAL = 5;

        
        private bool _hasFirstAdBeenDisplayed;
        private int _delayBeforeFirstInterstitial;
        
        private float _lastInterstitialTime;
        private int _delayBetweenInterstitial;
        
        private int _levelsPlayedSinceLastInterstitial;
        private int _maxLevelsPlayedBetweenInterstitial;
        
        private float _lastRVTime;
        private int _delayBetweenRVAndInterstitial;

        
        public TSFSDisplayConditions() : this(
            DEFAULT_DELAY_BEFORE_FIRST_INTERSTITIAL,
            DEFAULT_DELAY_BETWEEN_INTERSTITIAL,
            DEFAULT_MAX_LEVELS_BETWEEN_INTERSTITIAL,
            DEFAULT_DELAY_BETWEEN_RV_AND_INTERSTITIAL)
        { }

        public TSFSDisplayConditions(
            int delayBeforeFirstInterstitial,
            int delayBetweenInterstitial,
            int maxLevelsBetweenInterstitial,
            int delayBetweenRvAndInterstitial)
        {
            _hasFirstAdBeenDisplayed = PlayerPrefs.HasKey(PREFS_FIRST_APP_LAUNCH);            
            _delayBeforeFirstInterstitial = delayBeforeFirstInterstitial;

            _lastInterstitialTime = Time.unscaledTime;
            _delayBetweenInterstitial = delayBetweenInterstitial;

            _levelsPlayedSinceLastInterstitial = AnalyticsStorageHelper.GetGameCount();
            _maxLevelsPlayedBetweenInterstitial = maxLevelsBetweenInterstitial;

            _lastRVTime = Time.unscaledTime - delayBetweenRvAndInterstitial;
            _delayBetweenRVAndInterstitial = delayBetweenRvAndInterstitial;
            if (_delayBetweenRVAndInterstitial == -1)
                _delayBetweenRVAndInterstitial = DEFAULT_DELAY_BETWEEN_RV_AND_INTERSTITIAL;
            //Debug.Log($" ===== FSDisplayConditions INITIALIZED :: {_delayBeforeFirstInterstitial}, {_delayBetweenInterstitial}, {_maxLevelsPlayedBetweenInterstitial}, {_delayBetweenRVAndInterstitial} ===== ");
        }

        internal bool AreConditionsMet()
        {
            bool isInterstitialAvailableAfterRV = Time.unscaledTime >= _lastRVTime + _delayBetweenRVAndInterstitial;
            bool isInterstitialAvailableAfterInterstitial = Time.unscaledTime >= _lastInterstitialTime + _delayBetweenInterstitial;
            bool hasEnoughLevelPlayed = AnalyticsStorageHelper.GetGameCount() - _levelsPlayedSinceLastInterstitial >= _maxLevelsPlayedBetweenInterstitial;

            if (!isInterstitialAvailableAfterRV)
            {
                Debug.LogWarning($"! Ad not displayed ! -> delay after Rewarded Video not reached yet: {Time.unscaledTime - _lastRVTime}/{_delayBetweenRVAndInterstitial}");
                return false;
            }
            if (!_hasFirstAdBeenDisplayed)
            {
                Debug.LogWarning($"! Ad not displayed ! -> delay before first interstitial ad not reached yet: {Time.unscaledTime}/{_delayBeforeFirstInterstitial}");
                return Time.unscaledTime >= _delayBeforeFirstInterstitial;
            }
            if (!isInterstitialAvailableAfterInterstitial && !hasEnoughLevelPlayed)
            {
                Debug.LogWarning($"! Ad not displayed ! -> delay or number of level between interstitial ads not reached yet: DelayBetweenInterstitialAds = {Time.unscaledTime - _lastInterstitialTime}/{_delayBetweenInterstitial} || LevelsBetweenInterstitials = {AnalyticsStorageHelper.GetGameCount() - _levelsPlayedSinceLastInterstitial}/{_maxLevelsPlayedBetweenInterstitial}");
            }
            return isInterstitialAvailableAfterInterstitial || hasEnoughLevelPlayed;
        }
        
        internal void InterstitialDisplayed()
        {
            SetFirstAdDisplayedIfNotAlready();

            _levelsPlayedSinceLastInterstitial = AnalyticsStorageHelper.GetGameCount();
            _lastInterstitialTime = Time.unscaledTime;
        }
        
        internal void RVDisplayed()
        {
            SetFirstAdDisplayedIfNotAlready();
            
            _lastRVTime = Time.unscaledTime;
        }

        private void SetFirstAdDisplayedIfNotAlready()
        {
            if (!_hasFirstAdBeenDisplayed)
            {
                _hasFirstAdBeenDisplayed = true;
                PlayerPrefs.SetInt(PREFS_FIRST_APP_LAUNCH, 1);
            }
        }
    }
}