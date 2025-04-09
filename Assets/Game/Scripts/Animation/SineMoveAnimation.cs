using System;
using UniRx;
using UnityEngine;

public class SineMoveAnimation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private AnimationCurve ease;
    private IDisposable updater;
    
    void Start()
    {
        updater = Observable.EveryUpdate()
            .TakeUntilDisable(this)
            .UniSubscribe(_ =>
            {
                float t = Mathf.PingPong(Time.time * speed, 1);
                transform.localPosition = Vector3.Lerp(startPos, endPos, ease.Evaluate(t));
            });
    }
}
