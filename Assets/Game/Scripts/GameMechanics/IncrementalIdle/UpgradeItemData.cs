using Garawell.Managers;
using Garawell.Managers.Events;
using Garawell.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


namespace EC.GameMechanics.IncrementalIdle
{
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Shop/Upgradeable Item")]
    public class UpgradeItemData : ScriptableObject
    {
        [TabGroup("Item Data")] public string name;
        [TabGroup("Item Data")] public float value;
        [TabGroup("Item Data")] public AnimationCurve valueCurve;
        [TabGroup("Item Data")] public float minValue;
        [TabGroup("Item Data")] public float maxValue;
        [TabGroup("Item Data")] public int currentLevel;
        [TabGroup("Item Data")] public int maxLevel;

        [TabGroup("Pricing")] public float startPrice;
        [TabGroup("Pricing")] public float costPerUpdate;
        [TabGroup("Pricing")] public float coefficient;
        [TabGroup("Pricing")] public float fixedNumber;
        private float currentPrice;

        public float Progression => Mathf.InverseLerp(0, maxLevel, currentLevel);

        public int CurrentPrice => (int)currentPrice;

        public UnityAction OnLevelUp;

        [Button]
        private void SetLevel()
        {
            PlayerPrefs.SetInt(name + "lvl", currentLevel);
        }

        public void Initialize()
        {
            currentLevel = PlayerPrefs.GetInt(name + "lvl", 0);
            currentPrice = PlayerPrefs.GetFloat(name + "Price", startPrice);
            value = Mathf.Lerp(minValue, maxValue, valueCurve.Evaluate(currentLevel / (float)maxLevel));
        }

        public bool Upgrade()
        {
            if (currentLevel == maxLevel || MainManager.Instance.CurrencyManager.currencies[0].currencyAmount < currentPrice)
                return false;

            // Spend Money
            EventRunner.SpendCurrency(0, CurrentPrice, false);

            // Update Level
            currentLevel++;
            PlayerPrefs.SetInt(name + "lvl", currentLevel);

            // Update value
            value = Mathf.Lerp(minValue, maxValue, valueCurve.Evaluate(currentLevel / (float)maxLevel));

            // Update Price
            currentPrice += Mathf.RoundToInt((costPerUpdate * currentLevel)+ startPrice);
            PlayerPrefs.SetFloat(name + "Price", currentPrice);
            

            OnLevelUp?.Invoke();

            return true;
        }

        public void ResetData()
        {
            PlayerPrefs.SetInt(name + "lvl", 0);
            PlayerPrefs.SetFloat(name + "Price", startPrice);
        }
    }
}

