using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateServer
{
    class CurrenciesExchangeRate
    {
        public string CurrencyPair { get; set; }
        public decimal CurrencyRatio { get; set; }

        public CurrenciesExchangeRate(string currencyPair, decimal currencyRatio)
        {
            CurrencyPair = currencyPair;
            CurrencyRatio = currencyRatio;
        }
    }
}
