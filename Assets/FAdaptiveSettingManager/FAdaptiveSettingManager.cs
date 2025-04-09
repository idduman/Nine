using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAdaptiveSettingManager : MonoBehaviour
{
    #region Variables
    
    // Public Variables
    public static FAdaptiveSettingManager Instance;

    // Private Variables
    [SerializeField] private int averageFpsCalculateDurationInSeconds = 10; // We starting from medium quality
    [SerializeField] private int startingQualityIndex = 2; // We starting from medium quality
    [SerializeField] private int maxQualityLevelIndex = 4; // We have 5 quality level by default (4 index is max)
    // and that quality settings are: Very Low, Low, Medium, High, Very high
    [SerializeField] private int decreaseQualityTheFpsLimit = 21;
    [SerializeField] private int increaseQualityTheFpsLimit = 28;
    
    private float _accum = 0; // FPS accumulated over the interval
    private int _frames = 0; // Frames drawn over the interval
    private float _timeleft; // Left time for current interval
    private float _currentFPS = 0;

    private float _median = 0;
    private float _average = 0;

    private Dictionary<int, int> _qualityEnterCounter; // qualityInx, counterVal
    private Coroutine _checkFpsPerformanceCor;
    private const float UpdateInterval = 0.002f;
    private const float MedianLearnRate = 0.025f;
    
    #endregion Variables
    
    private void Awake()
    {
        Instance = this;
        
        _qualityEnterCounter = new();
        
        FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice = startingQualityIndex;
        ChangeUrpAssetAndRenderScale();
        _qualityEnterCounter.Add(startingQualityIndex, 1);

        Invoke(nameof(CallSetQualityBasedOnAverageFpsValue), 4);
        
        DontDestroyOnLoad(gameObject);
    }

    private void CallSetQualityBasedOnAverageFpsValue()
    {
        _checkFpsPerformanceCor = StartCoroutine(SetQualityBasedOnAvarageFpsValue());
    }

    public void SetQuality(int inx)
    {
        if (_checkFpsPerformanceCor != null)
        {
            StopCoroutine(_checkFpsPerformanceCor);
        }
        
        FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice = inx;
        ChangeUrpAssetAndRenderScale();
    }

    private IEnumerator SetQualityBasedOnAvarageFpsValue()
    {
        bool finalQualitySetted = false;

        while (!finalQualitySetted)
        {
            if (_qualityEnterCounter.ContainsKey(FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice) &&
                _qualityEnterCounter[FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice] >= 2)
            {
                break;
            }

            float timer = 0;
            _average = 0;
            _median = 0;
        
            while (timer <= averageFpsCalculateDurationInSeconds)
            {
                timer += Time.deltaTime;
            
                _timeleft -= Time.deltaTime;
                _accum += Time.timeScale/Time.deltaTime;
                ++_frames;

                if( _timeleft <= 0.0)
                {
                    _currentFPS = _accum / _frames;

                    _average += (Mathf.Abs(_currentFPS) - _average) * 0.1f;
                    _median += Mathf.Sign(_currentFPS - _median) * Mathf.Min(_average * MedianLearnRate, Mathf.Abs(_currentFPS - _median));

                    _timeleft = UpdateInterval;
                    _accum = 0.0F;
                    _frames = 0;
                }

                yield return null;
            }
            
            Debug.Log($"Final Avg. FPS: {_average}");
            
            if (_average <= decreaseQualityTheFpsLimit) // Highest FPS / 1,35
            {
                // Lower the quality
                
                Debug.Log("Lower the quality");

                if (FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice <= 0)
                {
                    finalQualitySetted = true;
                }
                else
                {
                    FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice--;
                    ChangeUrpAssetAndRenderScale();
                    
                    int indx = FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice;
            
                    if (_qualityEnterCounter.ContainsKey(indx))
                    {
                        _qualityEnterCounter[indx]++;
                    }
                    else
                    {
                        _qualityEnterCounter.Add(indx, 1);
                    }
        
                    Debug.Log($"{indx} indx's Counter: {_qualityEnterCounter[indx]}");
                }
            }
            else if (_average >= increaseQualityTheFpsLimit) // Highest FPS * 0,95
            {
                // Increase the quality
                
                Debug.Log("Increase the quality");
                
                if (FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice >= maxQualityLevelIndex)
                {
                    finalQualitySetted = true;
                }
                else
                {
                    FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice++;
                    ChangeUrpAssetAndRenderScale();
                    
                    int indx = FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice;
            
                    if (_qualityEnterCounter.ContainsKey(indx))
                    {
                        _qualityEnterCounter[indx]++;
                    }
                    else
                    {
                        _qualityEnterCounter.Add(indx, 1);
                    }
        
                    Debug.Log($"{indx} indx's Counter: {_qualityEnterCounter[indx]}");
                }
            }
            else
            {
                finalQualitySetted = true;
            }
            
            Debug.Log($"Current Quality Level: {FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice}");
        }
        
        Debug.Log($"Final Quality Setted: {FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice}");
    }

    private void ChangeUrpAssetAndRenderScale()
    {
        int indx = FAdaptiveSettingSaveManager.AdaptiveUrpAssetChoice;

        int targetFrameRate=60;
        
        switch (indx)
        {
            case 0:
            case 1:
                targetFrameRate = 30;
                break;
            case 2:
#if UNITY_IPHONE
                    targetFrameRate = 60;
#endif
#if UNITY_ANDROID
                targetFrameRate = 45;
#endif
                break;
            case 3:
                targetFrameRate = 60;
                break;
            case 4:
                targetFrameRate = 90;
                break;
            default:
                targetFrameRate = 30;
                break;
        }

        Application.targetFrameRate = targetFrameRate;

        QualitySettings.SetQualityLevel(indx, true);
    }
}