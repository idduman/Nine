using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Garawell.Managers.Events;
using Sirenix.OdinInspector;

namespace Garawell.Managers
{
	public class CurrencyDropManager : MonoBehaviour
	{
		[Button]
		public void FlyCurrency(Vector3 flyPosition, int increasingAmount, int currencyElementId = 0)
		{
			Image flyObject = MainManager.Instance.PoolingManager.GetGameObjectById(PoolID.Currency).GetComponent<Image>();
			flyObject.transform.position = Camera.main.WorldToScreenPoint(flyPosition);
			Menu.CurrencyUI currencyUI = MainManager.Instance.MenuManager.GamePanel.currencyPanels[currencyElementId];
			flyObject.sprite = currencyUI.currencyImage.sprite;
			flyObject.transform.SetParent(currencyUI.transform, true);
			flyObject.transform.DOLocalMove(currencyUI.flyCurrencyTargetTransform.localPosition, 1).onComplete = () =>
			{
				flyObject.gameObject.SetActive(false);
				EventRunner.EarnCurrency(currencyElementId, increasingAmount, false);
			};
		}

		[Button]
		public void CurrencyText(Vector3 position, int increasingAmount, int currencyElementId = 0, float targetScale = 0.75f)
		{
			CurrencyDropText textObject = MainManager.Instance.PoolingManager.GetGameObjectById(PoolID.CurrencyText).GetComponent<CurrencyDropText>();
			textObject.transform.localScale = Vector3.zero;
			textObject.transform.position = Camera.main.WorldToScreenPoint(position);
			Menu.CurrencyUI currencyUI = MainManager.Instance.MenuManager.GamePanel.currencyPanels[currencyElementId];
			textObject.TargetText.text = "+" + increasingAmount;
			textObject.transform.SetParent(currencyUI.transform, true);
			textObject.transform.DOScale(Vector3.one * targetScale, 0.5f).SetEase(Ease.OutBack, 5).OnComplete(() =>
			{
				EventRunner.EarnCurrency(currencyElementId, increasingAmount, false);
				textObject.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine).SetDelay(0.1f).OnComplete(() =>
				{
					textObject.gameObject.SetActive(false);
				});
			});
		}

		[Button]
		public void CurrencyDropElement(CurrencyDropElement currencyDropElement, Vector3 startPosition, float targetScale, int increasingAmount, int currencyElementId = 0, bool flyCurrency = true, float currencyEndDelay = 0.25f, float jumpPower = 3, float jumpTime = 0.75f)
		{
			CurrencyDropElementController currencyDropElementController = MainManager.Instance.PoolingManager.GetGameObjectById(PoolID.CurrencyDropElement, startPosition, Quaternion.identity, Vector3.zero).GetComponent<CurrencyDropElementController>();
			currencyDropElementController.GetCurrencyDropElement(currencyDropElement, startPosition, targetScale, increasingAmount, currencyEndDelay, flyCurrency, currencyElementId, jumpPower, jumpTime);
		}
	}
}
