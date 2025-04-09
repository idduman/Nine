using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

namespace Garawell.Managers.Menu
{
    public class CurrencyUI : MonoBehaviour
    {
        public TextMeshProUGUI currencyAmountText;
        public Image currencyImage;
        public Transform flyCurrencyTargetTransform;

        [SerializeField] private float punch = 0.2f;
        [SerializeField] private float punchDuration = 0.5f;
        [SerializeField] private int vibrato = 6;
        [SerializeField] private Transform punchAnimationParent;

        private string currencySign;
        private int currencyAmount;
        private Coroutine incrementRoutine;
        private Tween punchTween;

        private GameObject _startParent;
        private Vector2 _startPos;

        public void Initialize(string currencySign, int currencyAmount)
        {
            _startPos = transform.position;
            this.currencySign = currencySign;
            SetCurrency(currencyAmount);

            _startParent = transform.parent.parent.gameObject;

            MainManager.Instance.EventManager.Register(EventTypes.LevelStart, MoveToGameCanvas);
            MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, MoveToEndPanel);
            MainManager.Instance.EventManager.Register(EventTypes.LevelFail, MoveToEndPanel);
        }

        private void MoveToGameCanvas(EventArgs eventArgs)
        {
            transform.parent.SetParent(_startParent.transform);
            transform.parent.localScale = Vector3.one; // sebebini bilmiyorum 0 oluyor
            transform.localPosition = Vector3.zero;
        }

        private void MoveToEndPanel(EventArgs eventArgs)
        {
            StartCoroutine(DelayedSetParent());
        }

        private IEnumerator DelayedSetParent()
        {
            yield return new WaitForEndOfFrame();

            if (MainManager.Instance.MenuManager.YouWin)
            {
                transform.parent.SetParent(MainManager.Instance.MenuManager.SuccessPanel.transform);
            }
            else
            {

                transform.parent.SetParent(MainManager.Instance.MenuManager.FailPanel.transform);
            }

            transform.parent.localScale = Vector3.one; // sebebini bilmiyorum 0 oluyor
            transform.localPosition = Vector3.zero;
        }

        public void SetCurrency(int nextAmount)
        {
            if (currencyAmount < nextAmount)
            {
                PunchAnimation();
            }
            else if (currencyAmount > nextAmount)
            {
                PunchAnimation();
            }
            currencyAmount = nextAmount;
            currencyAmountText.text = nextAmount + "" + currencySign;
        }

        public void SetCurrencyIncremental(int nextAmount)
        {
            if (incrementRoutine != null)
            {
                StopCoroutine(incrementRoutine);
            }
            PunchAnimation();
            incrementRoutine = StartCoroutine(MoneyChangeRoutine(nextAmount));
        }

        public IEnumerator MoneyChangeRoutine(int nextAmount)
        {
            float lerpValue = 0;
            while (lerpValue < 1)
            {
                currencyAmount = (int)Mathf.Lerp(currencyAmount, nextAmount, lerpValue);
                currencyAmountText.text = currencyAmount + currencySign;
                lerpValue += Time.deltaTime * 2f;
                yield return null;
            }
            SetCurrency(nextAmount);
        }

        private void PunchAnimation()
        {
            punchTween?.Kill(true);
            punchTween = punchAnimationParent.DOPunchScale(Vector3.one * punch, punchDuration, vibrato);
        }
    }
}

