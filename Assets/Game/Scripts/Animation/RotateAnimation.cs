using System;
using UniRx;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float offset;
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to;
    [SerializeField] private AnimationCurve ease;
    private IDisposable updater;
    void OnEnable()
    {
        updater = Observable.EveryUpdate()
            .TakeUntilDisable(this)
            .UniSubscribe(_ =>
            {
                float t = Mathf.Repeat((Time.time + offset) * speed, 1);
                Vector3 targetAngles = Vector3.Lerp(from, to, ease.Evaluate(t));
                transform.localRotation = Quaternion.Euler(targetAngles);
            });
    }
}
