using UniRx;
using UnityEngine;
using System;

namespace Garawell.Utility
{
    public class UniRxUtils
    {
        public static IDisposable Delay(float second, Action action)
        {
            return Observable.Timer(TimeSpan.FromSeconds(second)).UniSubscribe(_ => action());
        }

        public static IDisposable Interval(float second, Action action)
        {
            return Observable.Interval(TimeSpan.FromSeconds(second)).UniSubscribe(_ => action());
        }

        public static IDisposable Delay(float second, Action action, GameObject obj)
        {
            return Observable.Timer(TimeSpan.FromSeconds(second)).UniSubscribe(_ => action()).AddTo(obj);
        }

        public static IDisposable Interval(float second, Action action, GameObject obj)
        {
            return Observable.Interval(TimeSpan.FromSeconds(second)).UniSubscribe(_ => action()).AddTo(obj);
        }
    }
}

