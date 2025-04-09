using System;
using UnityEngine;

namespace Garawell.Managers.Currencies
{
    [System.Serializable]
    public class Currency
    {
        public Sprite currencyImage;
        public string currencySign;
        public int currencyAmount;
        public bool resetOnLevelChange;

        private int currencyId;
        
       
        public int CurrencyId { get => currencyId; }

        public void Initialize(int cId)
        {
            if(resetOnLevelChange)
            {
                MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, Reset);
            }
            this.currencyId = cId;
            if(!resetOnLevelChange)
            {
                currencyAmount = PlayerPrefs.GetInt("Currency" + currencyId, 0);
            }
        }

        public void Reset(EventArgs args)
        {
            PlayerPrefs.SetInt("Currency" + currencyId, 0);
            currencyAmount = 0;
        }

        public void Spend(int spentAmount)
        {
            currencyAmount -= spentAmount;
            SetPlayerPref();
        }

        public void Earn(int earningsAmount)
        {
            currencyAmount += earningsAmount;
            SetPlayerPref();
        }

        public void SetPlayerPref()
        {
            PlayerPrefs.SetInt("Currency" + currencyId, currencyAmount);
        }
    }
}

