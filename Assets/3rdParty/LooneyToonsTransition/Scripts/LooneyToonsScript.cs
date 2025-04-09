using UnityEngine;
using DG.Tweening;
using Garawell.Managers.Events;

public class LooneyToonsScript : MonoBehaviour
{
    public RectTransform looneyToonsTransition;
    private float _ltTransitionWidth;
    private Tween _currentTransitionTween;

    void Awake()
    {
        if (!looneyToonsTransition)
            return;
        
        _ltTransitionWidth = looneyToonsTransition.sizeDelta.x;
    }
    public void CloseTheCircle(float appearTime, bool finishGame)
    {
        if (!looneyToonsTransition)
            return;
        
        _currentTransitionTween.Kill(false);
        float tempWidth = looneyToonsTransition.sizeDelta.x;
        if (finishGame)
        {
            _currentTransitionTween = DOTween.To(() => tempWidth, x => tempWidth = x, 0f, appearTime)
                .SetUpdate(true)
                .OnUpdate(() => looneyToonsTransition.sizeDelta = new Vector2(tempWidth, looneyToonsTransition.sizeDelta.y)).OnComplete(() => EventRunner.LevelFinish());
        }
        else
        {
            _currentTransitionTween = DOTween.To(() => tempWidth, x => tempWidth = x, 0f, appearTime)
                .SetUpdate(true)
                .OnUpdate(() => looneyToonsTransition.sizeDelta = new Vector2(tempWidth, looneyToonsTransition.sizeDelta.y)).OnComplete(() => EventRunner.LevelRestart());
        }
    }

    public void OpenTheCircle(float disappearTime)
    {
        if (!looneyToonsTransition)
            return;
        
        _currentTransitionTween.Kill(false);
        float tempWidth = looneyToonsTransition.sizeDelta.x;
        _currentTransitionTween = DOTween.To(() => tempWidth, x => tempWidth = x, _ltTransitionWidth, disappearTime)
            .SetUpdate(true)
            .OnUpdate(() => looneyToonsTransition.sizeDelta = new Vector2(tempWidth, looneyToonsTransition.sizeDelta.y));
    }
}
