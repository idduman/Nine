using DG.Tweening;
using UnityEngine;

public static class TimeManager
{
    public static Tween timeTween;

    public static void SetTime(float timeScale)
    {
        timeTween?.Kill();
        Time.timeScale = timeScale;
    }
    
    public static void SetTime(float timeScale = 1f, float duration = 1f, Ease ease = Ease.OutCirc)
    {
        timeTween?.Kill();
        timeTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScale, duration)
            .SetEase(ease)
            .SetUpdate(true);
    }
}
