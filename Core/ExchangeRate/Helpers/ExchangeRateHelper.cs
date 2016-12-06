using System;
using System.Threading.Tasks;
using MyCryptos.Core.ExchangeRate.Storage;
using MyCryptos.Core.Types;

namespace MyCryptos.Core.ExchangeRate.Helpers
{
    public static class ExchangeRateHelper
    {
        public static Model.ExchangeRate GetRate(Model.ExchangeRate rate)
        {
            return GetRate(rate.ReferenceCurrency, rate.SecondaryCurrency);
        }

        public static Model.ExchangeRate GetRate(Currency.Model.Currency referenceCurrency,
            Currency.Model.Currency secondaryCurrency)
        {
            if (referenceCurrency == null || secondaryCurrency == null)
            {
                return null;
            }

            var rateReference = GetDirectRate(referenceCurrency, Currency.Model.Currency.Btc);
            var rateSecondary = GetDirectRate(Currency.Model.Currency.Btc, secondaryCurrency);

            if (rateReference == null || rateSecondary == null)
            {
                return null;
            }

            var rate = GetCombinedRate(rateReference, rateSecondary);

            return Equals(rate.ReferenceCurrency, referenceCurrency) ? rate : rate.Inverse;
        }

        private static Model.ExchangeRate GetDirectRate(Currency.Model.Currency referenceCurrency, Currency.Model.Currency secondaryCurrency)
        {
            if (referenceCurrency.Equals(secondaryCurrency))
            {
                return new Model.ExchangeRate(referenceCurrency, secondaryCurrency, 1);
            }

            var exchangeRate = new Model.ExchangeRate(referenceCurrency, secondaryCurrency);

            var available = AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
            var availableInverse = AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse);

            if (!available && !availableInverse) return null;

            return available ? ExchangeRateStorage.Instance.Find(exchangeRate) : ExchangeRateStorage.Instance.Find(exchangeRate.Inverse)?.Inverse;
        }

        public static Task<Model.ExchangeRate> GetRate(Model.ExchangeRate rate, FetchSpeedEnum speed)
        {
            return GetRate(rate.ReferenceCurrency, rate.SecondaryCurrency, speed);
        }

        private static async Task<Model.ExchangeRate> GetRate(Currency.Model.Currency referenceCurrency, Currency.Model.Currency secondaryCurrency, FetchSpeedEnum speed)
        {
            if (referenceCurrency == null || secondaryCurrency == null)
            {
                return null;
            }

            var rateReference = await GetDirectRate(referenceCurrency, Currency.Model.Currency.Btc, speed);
            var rateSecondary = await GetDirectRate(Currency.Model.Currency.Btc, secondaryCurrency, speed);

            if (rateReference == null || rateSecondary == null)
            {
                return null;
            }

            var rate = GetCombinedRate(rateReference, rateSecondary);

            return Equals(rate.ReferenceCurrency, referenceCurrency) ? rate : rate.Inverse;

        }

        private static async Task<Model.ExchangeRate> GetDirectRate(Currency.Model.Currency referenceCurrency, Currency.Model.Currency secondaryCurrency, FetchSpeedEnum speed)
        {
            if (referenceCurrency.Equals(secondaryCurrency))
            {
                return new Model.ExchangeRate(referenceCurrency, secondaryCurrency, 1);
            }

            var exchangeRate = new Model.ExchangeRate(referenceCurrency, secondaryCurrency);

            var exists = AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
            var existsInverse = AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse);

            if (!exists && !existsInverse) return null;

            await AddAndFetch(!exists, speed, exchangeRate);

            return exists ? ExchangeRateStorage.Instance.Find(exchangeRate) : ExchangeRateStorage.Instance.Find(exchangeRate.Inverse).Inverse;
        }

        private static Task AddRate(Model.ExchangeRate exchangeRate)
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

        private static async Task AddAndFetch(bool inverse, FetchSpeedEnum speed, Model.ExchangeRate exchangeRate)
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
                case FetchSpeedEnum.Slow: await ExchangeRateStorage.Instance.FetchOnline(); break;
                case FetchSpeedEnum.Medium: await ExchangeRateStorage.Instance.FetchNew(); break;
                case FetchSpeedEnum.Fast: await ExchangeRateStorage.Instance.LoadFromDatabase(); break;
                default: throw new ArgumentOutOfRangeException(nameof(speed), speed, null);
            }
        }

        private static Model.ExchangeRate GetCombinedRate(Model.ExchangeRate rate1, Model.ExchangeRate rate2)
        {
            var r = new Model.ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1));

            var r1 = GetFor(rate1, CommonCurrency(rate1, rate2));
            var r2 = GetFor(rate2, CommonCurrency(rate2, rate1));

            r.Rate = r2.Rate / r1.Rate;

            return r;
        }

        private static Currency.Model.Currency DifferentCurrency(Model.ExchangeRate r1, Model.ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null)
            {
                return null;
            }
            return r2.Contains(r1.ReferenceCurrency) ? r1.SecondaryCurrency : r2.Contains(r1.SecondaryCurrency) ? r1.ReferenceCurrency : null;
        }

        private static Currency.Model.Currency CommonCurrency(Model.ExchangeRate r1, Model.ExchangeRate r2)
        {
            return r2.Contains(r1.ReferenceCurrency) ? r1.ReferenceCurrency : r2.Contains(r1.SecondaryCurrency) ? r1.SecondaryCurrency : null;
        }

        private static Model.ExchangeRate GetFor(Model.ExchangeRate rate, Currency.Model.Currency currency)
        {
            return currency.Equals(rate.ReferenceCurrency) ? rate : currency.Equals(rate.SecondaryCurrency) ? rate.Inverse : null;
        }
    }
}
