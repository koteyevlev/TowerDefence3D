using System;

namespace TowerDefence3d.Scripts.Setup
{
    public class Currency
    {

        public event EventHandler<CurrencyEventArgs> CurrencyChanged;
        private float _currency;
        public float Value => _currency;

        public Currency(float firstCurrency)
        {
            _currency = firstCurrency;
            if (CurrencyChanged != null)
            {
                OnCurrencyChanged( new CurrencyEventArgs 
                { 
                    NewValue = _currency,
                });
            }
        }

        private void OnCurrencyChanged(CurrencyEventArgs e)
        {
            EventHandler<CurrencyEventArgs> handler = CurrencyChanged;
            handler?.Invoke(this, e);
        }

        public Currency()
        {
        }

        public void SetCurrency(float currency)
        {
            _currency = currency;
            OnCurrencyChanged(new CurrencyEventArgs
            {
                NewValue = _currency,
            });
        }

        internal bool CanAfford(float purchaseCost)
        {
            return _currency >= purchaseCost;
        }

        public class CurrencyEventArgs : EventArgs
        {
            public float NewValue { get; set; }
        }

        protected void ChangeCurrency(float increment)
        {
            if (increment > 0)
            {
                _currency += increment;
                OnCurrencyChanged( new CurrencyEventArgs
                { 
                    NewValue = _currency,
                });
            }
        }
    }
}