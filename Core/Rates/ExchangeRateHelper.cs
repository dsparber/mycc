using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;

namespace MyCC.Core.Rates
{
    public static class ExchangeRateHelper
    {

        public static ExchangeRate GetRate(ExchangeRate rate) => GetRate(new Currency.Model.Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency), new Currency.Model.Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency));

        public static ExchangeRate GetRate(Currency.Model.Currency currency1, Currency.Model.Currency currency2)
        {
            if (string.IsNullOrWhiteSpace(currency1?.Code) ||
                string.IsNullOrWhiteSpace(currency2?.Code)) throw new ArgumentException();

            if (currency1.Equals(currency2)) return new ExchangeRate(currency1, currency2, DateTime.Now, 1);


            var r1 = GetRateToBtc(currency1);
            var r2 = GetRateToBtc(currency2);

            if (r1 == null || r2 == null) return null;

            return GetCombinedRate(r1, r2);
        }

        private static ExchangeRate GetRateToBtc(Currency.Model.Currency currency)
        {
            if (currency.Code.Equals("BTC")) return new ExchangeRate(Currency.Model.Currency.Btc, Currency.Model.Currency.Btc, DateTime.Now, 1);

            if (currency.IsCryptoCurrency) return GetStoredRate(currency, Currency.Model.Currency.Btc);

            var rate = currency.Code.Equals("EUR") ? new ExchangeRate(Currency.Model.Currency.Eur, Currency.Model.Currency.Eur, DateTime.Now, 1) : GetStoredRate(currency, Currency.Model.Currency.Eur);
            var eurBtc = GetStoredRate(Currency.Model.Currency.Eur, Currency.Model.Currency.Btc);
            return GetCombinedRate(rate, eurBtc);
        }

        private static ExchangeRate GetStoredRate(Currency.Model.Currency c1, Currency.Model.Currency c2)
        {
            var neededRate = new ExchangeRate(c1, c2, DateTime.Now);
            var rates = ExchangeRatesStorage.Instance.StoredRates.ToList();
            return rates.Find(c => c?.Equals(neededRate) ?? false) ??
                    rates.Find(c => c?.Equals(neededRate.Inverse) ?? false)?.Inverse;
        }

        public static Task FetchMissingRatesFor(IEnumerable<ExchangeRate> rates)
        {
            var missingRates = rates.SelectMany(GetNeededRates).Distinct();
            return FetchMissingRates(missingRates);
        }

        private static async Task FetchMissingRates(IEnumerable<ExchangeRate> missingRates)
        {
            var storedRates = ExchangeRatesStorage.Instance.StoredRates.ToList();
            var neededToFetch = missingRates.Where(r => !(storedRates.Contains(r) || storedRates.Contains(r.Inverse)));

            var fetchedFixerIo = false;
            var fetchedBtce = false;

            foreach (var r in neededToFetch)
            {
                if (ExchangeRatesStorage.FixerIo.IsAvailable(r) || ExchangeRatesStorage.FixerIo.IsAvailable(r.Inverse))
                {
                    if (fetchedFixerIo) continue;

                    fetchedFixerIo = true;
                    await ExchangeRatesStorage.FixerIo.FetchRates();
                }
                else if (ExchangeRatesStorage.Btce.IsAvailable(r) || ExchangeRatesStorage.Btce.IsAvailable(r.Inverse))
                {
                    if (fetchedBtce) continue;

                    fetchedBtce = true;
                    await ExchangeRatesStorage.Btce.FetchRates();
                }
                else
                {
                    var supportedRepo =
                        ExchangeRatesStorage.Instance.Repositories.FirstOrDefault(repo => repo.IsAvailable(r)) ??
                        ExchangeRatesStorage.Instance.Repositories.FirstOrDefault(repo => repo.IsAvailable(r.Inverse));

                    if (supportedRepo == null) continue;

                    if (supportedRepo is IMultipleRatesRepository)
                    {
                        await ((IMultipleRatesRepository)supportedRepo).FetchRates();
                    }
                    else
                    {
                        await ((ISingleRateRepository)supportedRepo).FetchRate(supportedRepo.IsAvailable(r) ? r : r.Inverse);
                    }
                }
            }
        }

        public static async Task UpdateRates(IEnumerable<ExchangeRate> rates)
        {
            var updatedRepositories = new List<int>();

            var ratesToUpdate = rates.SelectMany(GetNeededRates);

            foreach (var r in ratesToUpdate)
            {
                var supportedRepo = ExchangeRatesStorage.Instance.Repositories.FirstOrDefault(repo => repo.IsAvailable(r));
                if (supportedRepo == null) continue;

                var multiRepo = supportedRepo as IMultipleRatesRepository;
                if (multiRepo != null)
                {
                    if (updatedRepositories.Contains(multiRepo.TypeId)) continue;

                    updatedRepositories.Add(multiRepo.TypeId);
                    await multiRepo.FetchRates();
                }
                else
                {
                    await ((ISingleRateRepository)supportedRepo).FetchRate(r);
                }
            }
        }

        public static IEnumerable<ExchangeRate> GetNeededRates(ExchangeRate rate)
        {
            if (string.IsNullOrWhiteSpace(rate?.ReferenceCurrencyCode) ||
                string.IsNullOrWhiteSpace(rate.SecondaryCurrencyCode)) throw new ArgumentNullException();

            if (rate.ReferenceCurrencyCode.Equals(rate.SecondaryCurrencyCode)) return new List<ExchangeRate>();


            var r1 = GetNeededRatesToBtc(new Currency.Model.Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency));
            var r2 = GetNeededRatesToBtc(new Currency.Model.Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency));

            return r1.Concat(r2);
        }

        private static IEnumerable<ExchangeRate> GetNeededRatesToBtc(Currency.Model.Currency currency)
        {
            if (currency.Code.Equals("BTC")) return new List<ExchangeRate>();

            if (currency.IsCryptoCurrency) return new List<ExchangeRate> { new ExchangeRate(currency, Currency.Model.Currency.Btc) };

            return new List<ExchangeRate> {
                new ExchangeRate(currency, Currency.Model.Currency.Eur),
                new ExchangeRate(Currency.Model.Currency.Eur, Currency.Model.Currency.Btc)
            }.Where(r => r.ReferenceCurrencyCode != r.SecondaryCurrencyCode);
        }



        private static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
        {
            if (rate1 == null || rate2 == null) return null;

            var r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1), new[] { rate1.LastUpdate, rate2.LastUpdate }.Min());

            var r1 = InvertIfNeeded(rate1, CommonCurrency(rate1, rate2));
            var r2 = InvertIfNeeded(rate2, CommonCurrency(rate2, rate1));

            r.Rate = r2.Rate / r1.Rate;

            return r;
        }

        private static Currency.Model.Currency DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null) return null;

            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ?
                     new Currency.Model.Currency(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) :
                     new Currency.Model.Currency(r1.ReferenceCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency);
        }

        private static string CommonCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ? r1.ReferenceCurrencyCode : r2.Contains(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) ? r1.SecondaryCurrencyCode : null;
        }

        private static ExchangeRate InvertIfNeeded(ExchangeRate rate, string currencyCode)
        {
            return currencyCode.Equals(rate.ReferenceCurrencyCode) ? rate : rate.Inverse;
        }
    }
}
