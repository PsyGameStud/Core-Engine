using System;
using System.Collections.Generic;

namespace Services.SaveSystem.SavingData
{
    [Serializable]
    public class CurrencySaveData : SaveData
    {
        public List<CurrencyData> Currencies = new ();
    }

    public class CurrencyData
    {
        public CurrencyType CurrencyType;
        public ulong CurrencyValue;
    }

    public enum CurrencyType
    {
        Money,
    }
}
