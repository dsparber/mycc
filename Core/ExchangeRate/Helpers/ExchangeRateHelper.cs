using System;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Models;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Helpers
{
    public static class ExchangeRateHelper
    {
        public static ExchangeRate GetRate(ExchangeRate rate)
        {
            return GetRate(rate.ReferenceCurrency, rate.SecondaryCurrency);
        }

        public static ExchangeRate GetRate(Currency referenceCurrency, Currency secondaryCurrency)
        {
            if (referenceCurrency == null || secondaryCurrency == null)
            {
                return null;
            }

            var rate = GetDirectRate(referenceCurrency, secondaryCurrency);

            if (rate != null)
            {
                return rate;
            }

            // Indirect match (one intermediate currency)
            var referenceCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(referenceCurrency);
            var secondaryCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(secondaryCurrency);

            return (from r1 in referenceCurrencyRates from r2 in secondaryCurrencyRates where FindMatch(r1, r2, referenceCurrency, secondaryCurrency) let e1 = ExchangeRateStorage.Instance.Find(r1) ?? r1 let e2 = ExchangeRateStorage.Instance.Find(r2) ?? r2 select GetCombinedRate(e1, e2)).FirstOrDefault();
        }

        private static ExchangeRate GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency)
        {
            if (referenceCurrency.Equals(secondaryCurrency))
            {
                return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
            }

            var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

            if (!AvailableRatesStorage.Instance.IsAvailable(exchangeRate) && !AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse)) return null;

            if (AvailableRatesStorage.Instance.IsAvailable(exchangeRate))
            {
                return ExchangeRateStorage.Instance.Find(exchangeRate);
            }

            var e = ExchangeRateStorage.Instance.Find(exchangeRate.Inverse);
            return e?.Inverse;
        }

        public static Task<ExchangeRate> GetRate(ExchangeRate rate, FetchSpeedEnum speed)
        {
            return GetRate(rate.ReferenceCurrency, rate.SecondaryCurrency, speed);
        }

        public static async Task<ExchangeRate> GetRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
        {
            if (referenceCurrency == null || secondaryCurrency == null)
            {
                return null;
            }

            var rate = await GetDirectRate(referenceCurrency, secondaryCurrency, speed);

            if (rate != null)
            {
                return rate;
            }

            // Indirect match (one intermediate currency)
            var referenceCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(referenceCurrency);
            var secondaryCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(secondaryCurrency);


            foreach (var r1 in referenceCurrencyRates)
            {
                foreach (var r2 in secondaryCurrencyRates)
                {
                    if (FindMatch(r1, r2, referenceCurrency, secondaryCurrency))
                    {
                        await AddRate(r1);
                        await AddRate(r2);
                        await Fetch(speed);

                        var e1 = ExchangeRateStorage.Instance.Find(r1) ?? r1;
                        var e2 = ExchangeRateStorage.Instance.Find(r2) ?? r2;

                        return GetCombinedRate(e1, e2);
                    }
                }
            }
            return null;
        }

        private static async Task<ExchangeRate> GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
        {
            if (referenceCurrency.Equals(secondaryCurrency))
            {
                return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
            }

            var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

            var exists = AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
            var existsInverse = AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse);

            if (exists || existsInverse)
            {
                await AddAndFetch(!exists, speed, exchangeRate);

                if (exists)
                {
                    return ExchangeRateStorage.Instance.Find(exchangeRate);
                }
                return ExchangeRateStorage.Instance.Find(exchangeRate.Inverse).Inverse;
            }
            return null;
        }

        private static Task AddRate(ExchangeRate exchangeRate)
        {
            if (ExchangeRateStorage.Instance.AllElements.Contains(exchangeRate))
            {
                return Task.Factory.StartNew(() => { });
            }

            foreach (var r in AvailableRatesStorage.Instance.Repositories)
            {
                if (!r.IsAvailable(exchangeRate)) continue;

                exchangeRate.ParentId = r.ExchangeRateRepository.Id;
                return r.ExchangeRateRepository.AddOrUpdate(exchangeRate);
            }
            return Task.Factory.StartNew(() => { });
        }

        private static async Task AddAndFetch(bool inverse, FetchSpeedEnum speed, ExchangeRate exchangeRate)
        {
            if (inverse)
            {
                await AddRate(exchangeRate.Inverse);
            }
            else
            {
                await AddRate(exchangeRate);
            }

            await Fetch(speed);
        }

        private static async Task Fetch(FetchSpeedEnum speed)
        {
            switch (speed)
            {
                case FetchSpeedEnum.SLOW: await ExchangeRateStorage.Instance.Fetch(); break;
                case FetchSpeedEnum.MEDIUM: await ExchangeRateStorage.Instance.FetchNew(); break;
                case FetchSpeedEnum.FAST: await ExchangeRateStorage.Instance.FetchFast(); break;
                default: throw new ArgumentOutOfRangeException(nameof(speed), speed, null);
            }
        }

        private static bool FindMatch(ExchangeRate r1, ExchangeRate r2, Currency ref1, Currency ref2)
        {
            return (r1.Contains(r2.ReferenceCurrency) || r1.Contains(r2.SecondaryCurrency)) && !CommonCurrency(r1, r2).Equals(ref1) && !CommonCurrency(r1, r2).Equals(ref2);
        }

        private static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
        {
            var r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1));

            var r1 = GetFor(rate1, CommonCurrency(rate1, rate2));
            var r2 = GetFor(rate2, CommonCurrency(rate2, rate1));

            r.Rate = r2.Rate / r1.Rate;

            return r;
        }

        private static Currency DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null)
            {
                return null;
            }
            return r2.Contains(r1.ReferenceCurrency) ? r1.SecondaryCurrency : r2.Contains(r1.SecondaryCurrency) ? r1.ReferenceCurrency : null;
        }

        private static Currency CommonCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            return r2.Contains(r1.ReferenceCurrency) ? r1.ReferenceCurrency : r2.Contains(r1.SecondaryCurrency) ? r1.SecondaryCurrency : null;
        }

        private static ExchangeRate GetFor(ExchangeRate rate, Currency currency)
        {
            return currency.Equals(rate.ReferenceCurrency) ? rate : currency.Equals(rate.SecondaryCurrency) ? rate.Inverse : null;
        }
    }
}
