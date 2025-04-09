using System;
using DG.Tweening;
using Garawell.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EC.GameMechanics.IncrementalIdle
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField, TabGroup("Item")] private UpgradeItemData item;

        [SerializeField, TabGroup("Animations")] private Vector2 punch;
        [SerializeField, TabGroup("Animations")] private int punchVibrato;
        [SerializeField, TabGroup("Animations")] private float punchDuration;
        [SerializeField, TabGroup("Animations")] private float inactiveAlpha;

        [SerializeField, TabGroup("UI")] private TextMeshProUGUI level;
        [SerializeField, TabGroup("UI")] private TextMeshProUGUI price;
        [SerializeField, TabGroup("UI")] private TextMeshProUGUI currentValue;

        private CanvasGroup group;

        private Tween punchTween;

        public void Initialize()
        {
            group = GetComponent<CanvasGroup>();
            MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, arg0 => UpdateButton());
            MainManager.Instance.EventManager.Register(EventTypes.CurrencySpent, arg0 => UpdateButton());
            UpdateButton();
        }

        public void UpdateButton()
        {
            SetupUI();
        }

        private void OnEnable()
        {
            currentValue.text = item.value.ToString("F0");
       
        }

        public void SetupUI()
        {
            bool interactable = MainManager.Instance.CurrencyManager.currencies[0].currencyAmount >= item.CurrentPrice && item.currentLevel < item.maxLevel;
            group.interactable = interactable;
            group.alpha = interactable ? 1f : inactiveAlpha;
            level.text = item.currentLevel.ToString();
            price.text = item.currentLevel < item.maxLevel ? ((int)item.CurrentPrice).ToString() : "Max";
        }

        public void Upgrade()
        {
            if (item.Upgrade())
            {
                punchTween?.Kill(true);
                punchTween = transform.DOPunchScale(punch, punchDuration, punchVibrato);
                SetupUI();
            }
        }
    }

}
