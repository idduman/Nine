using System.Collections;
using System;

namespace Garawell.Managers.Events
{
    public class CurrencyArgs : EventArgs
    {
        public int currencyId;
        public int changeAmount;
        public bool addIncrementally;

        public CurrencyArgs(int currencyId, int changeAmount, bool addIncrementally)
        {
            this.currencyId = currencyId;
            this.changeAmount = changeAmount;
            this.addIncrementally = addIncrementally;
        }
    }
}

