using DG.Tweening;
using UnityEngine;

public class ECButton : MonoBehaviour
{
    [SerializeField] private float punch = 0.1f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private int vibrato = 6;
    private Tween punchTween;
    
    public void OnClick()
    {
        Punch();
    }

    private void Punch()
    {
        punchTween?.Kill(true);
        punchTween = transform.DOPunchScale(Vector3.one * punch, duration, vibrato)
            .SetUpdate(true);
    }
}
