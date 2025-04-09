using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System;

namespace Garawell.Managers
{
	public class CurrencyDropElementController : MonoBehaviour
	{
		[SerializeField] private GameObject[] dropElements;

		private Tween scaleTween;
		private Tween jumpTween;

		private void SetCurrencyDropElement(CurrencyDropElement currencyDropElement)
		{
			for (int i = 0; i < dropElements.Length; i++)
			{
				if (i == (int)currencyDropElement)
				{
					dropElements[i].SetActive(true);
				}
				else
				{
					dropElements[i].SetActive(false);
				}
			}
		}

		public void GetCurrencyDropElement(CurrencyDropElement currencyDropElement, Vector3 startPosition, float targetScale, int increasingAmount, float currencyEndDelay = 0.25f, bool flyCurrency = true, int currencyElementId = 0, float jumpPower = 3, float jumpTime = 0.75f)
		{
			scaleTween?.Kill(true);
			jumpTween?.Kill(true);
			SetCurrencyDropElement(currencyDropElement);
			transform.localScale = Vector3.zero;
			transform.position = startPosition;
			transform.eulerAngles = new Vector3(0, 170 * UnityEngine.Random.Range(0, 1), 0);
			scaleTween = transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutSine);
			jumpTween = transform.DOJump(new Vector3(startPosition.x + UnityEngine.Random.Range(-1.5f, 1.5f), 0, startPosition.z + UnityEngine.Random.Range(-1.5f, 1.5f)), jumpPower, 0, jumpTime).OnComplete(() =>
			{
				Observable.Timer(TimeSpan.FromSeconds(currencyEndDelay)).TakeUntilDisable(this).UniSubscribe(l =>
				{
					if (flyCurrency)
					{
						Managers.MainManager.Instance.CurrencyDropManager.FlyCurrency(transform.position, increasingAmount, currencyElementId);
					}
					else
					{
						Managers.MainManager.Instance.CurrencyDropManager.CurrencyText(transform.position, increasingAmount, currencyElementId);
					}
					gameObject.SetActive(false);
				});
			});
		}
	}
}
