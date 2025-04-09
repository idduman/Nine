using UnityEngine;
using System;
using Garawell.Managers.Events;
using Garawell.Managers.Currencies;

namespace Garawell.Managers
{
    [CreateAssetMenu(fileName = "CurrencyManager", menuName = "Scriptlable Objects/Currency Manager")]
    public class CurrencyManager : ScriptableObject
    {
        public Currency[] currencies;

        public void Initialize()
        {
            MainManager.Instance.EventManager.Register(EventTypes.CurrencyEarned, EarnMoney);
            MainManager.Instance.EventManager.Register(EventTypes.CurrencySpent, SpendMoney);

            for (int i = 0; i < currencies.Length; i++)
            {
                currencies[i].Initialize(i);
            }
        }

        public void EarnMoney(EventArgs eventArgs)
        {
            var currencyValue = eventArgs as CurrencyArgs;
            currencies[currencyValue.currencyId].Earn(currencyValue.changeAmount);
        }

        public void SpendMoney(EventArgs eventArgs)
        {
            var currencyValue = eventArgs as CurrencyArgs;
            currencies[currencyValue.currencyId].Spend(currencyValue.changeAmount);
        }
    }
}

