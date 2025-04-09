using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Garawell.Utility
{
	public class Delayer
	{
		private static List<int> cancelledDelays = new List<int>();

		public static int Delay(float duration, UnityAction action)
        {
			int randomValue = 0;
			do
			{
				randomValue = Random.Range(0, int.MaxValue);
			}
			while(cancelledDelays.Contains(randomValue));
			DelayAsync(duration, action, randomValue);
			return randomValue;
		}

		private static async void DelayAsync(float duration, UnityAction action, int delayId)
        {
			await StartDelay(duration, action, delayId);
        }
		private static async Task StartDelay(float duration, UnityAction action, int delayId)
		{
			var end = Time.time + duration;
			while(Time.time < end)
            {
				await Task.Yield();
            }
			if(cancelledDelays.Contains(delayId))
            {
				cancelledDelays.Remove(delayId);
				return;
            }
			action?.Invoke();
		}

		public static void CancelDelay(int delayId)
        {
			if(!cancelledDelays.Contains(delayId))
			cancelledDelays.Add(delayId);
        }
	}
}

