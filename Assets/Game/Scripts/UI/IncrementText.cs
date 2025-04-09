using DG.Tweening;
using TMPro;
using UnityEngine;

public class IncrementText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private string before;
    [SerializeField] private string after;
    private Tween incrementTween;
    
    public void SetValue(float from, float to)
    {
        incrementTween?.Kill();
        incrementTween = DOTween.To(() => from, x =>
            {
                myText.text = before + (int)x + after;
            }, to, duration)
            .SetEase(ease)
            .SetUpdate(true);
    }
}
