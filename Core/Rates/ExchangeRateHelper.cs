using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates.Repositories.Interfaces;

namespace MyCC.Core.Rates
{
    public static class ExchangeRateHelper
    {

        public static ExchangeRate GetRate(Currency.Model.Currency currency1, Currency.Model.Currency currency2) => GetRate(new ExchangeRate(currency1.Code, currency2.Code));
        public static ExchangeRate GetRate(string code1, string code2) => GetRate(new ExchangeRate(code1, code2));

        public static ExchangeRate GetRate(ExchangeRate rate)
        {
            if (string.IsNullOrWhiteSpace(rate?.ReferenceCurrencyCode) ||
                string.IsNullOrWhiteSpace(rate.SecondaryCurrencyCode)) throw new ArgumentException();

            if (rate.ReferenceCurrencyCode.Equals(rate.SecondaryCurrencyCode))
                return new ExchangeRate(rate.ReferenceCurrencyCode, rate.SecondaryCurrencyCode, 1);


            var r1 = GetRateToBtc(rate.ReferenceCurrencyCode);
            var r2 = GetRateToBtc(rate.SecondaryCurrencyCode);

            if (r1 == null || r2 == null) return null;

            return GetCombinedRate(r1, r2);
        }

        private static ExchangeRate GetRateToBtc(string currencyCode)
        {
            if (currencyCode.Equals("BTC")) return new ExchangeRate("BTC", "BTC", 1);

            if (!ExchangeRatesStorage.FixerIo.IsAvailable(new ExchangeRate("EUR", currencyCode))) return GetStoredRate(new ExchangeRate(currencyCode, "BTC"));

            var rate = currencyCode.Equals("EUR") ? new ExchangeRate("EUR", "EUR", 1) : GetStoredRate(new ExchangeRate(currencyCode, "EUR"));
            var eurBtc = GetStoredRate(new ExchangeRate("EUR", "BTC"));
            return GetCombinedRate(rate, eurBtc);
        }

        private static ExchangeRate GetStoredRate(ExchangeRate neededRate)
        {
            return ExchangeRatesStorage.Instance.StoredRates.FirstOrDefault(c => c.Equals(neededRate)) ??
                ExchangeRatesStorage.Instance.StoredRates.FirstOrDefault(c => c.Equals(neededRate.Inverse))?.Inverse;
        }

        public static Task FetchMissingRatesFor(IEnumerable<ExchangeRate> rates)
        {
            var missingRates = rates.SelectMany(GetNeededRates).Distinct();
            return FetchMissingRates(missingRates);
        }

        private static async Task FetchMissingRates(IEnumerable<ExchangeRate> missingRates)
        {
            var storedRates = ExchangeRatesStorage.Instance.StoredRates.ToList();
            var neededToFetch = missingRates.Where(r => !storedRates.Contains(r));

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

            foreach (var r in rates)
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

        private static IEnumerable<ExchangeRate> GetNeededRates(ExchangeRate rate)
        {
            if (string.IsNullOrWhiteSpace(rate?.ReferenceCurrencyCode) ||
                string.IsNullOrWhiteSpace(rate.SecondaryCurrencyCode)) throw new ArgumentNullException();

            if (rate.ReferenceCurrencyCode.Equals(rate.SecondaryCurrencyCode)) return new List<ExchangeRate>();


            var r1 = GetNeededRatesToBtc(rate.ReferenceCurrencyCode);
            var r2 = GetNeededRatesToBtc(rate.SecondaryCurrencyCode);

            return r1.Concat(r2);
        }

        private static IEnumerable<ExchangeRate> GetNeededRatesToBtc(string currencyCode)
        {
            if (currencyCode.Equals("BTC")) return new List<ExchangeRate>();

            if (!ExchangeRatesStorage.FixerIo.IsAvailable(new ExchangeRate("EUR", currencyCode))) return new List<ExchangeRate> { new ExchangeRate(currencyCode, "BTC") };

            return new List<ExchangeRate> {
                new ExchangeRate(currencyCode, "EUR"),
                new ExchangeRate("EUR", "BTC")
            }.Where(r => r.ReferenceCurrencyCode != r.SecondaryCurrencyCode);
        }



        private static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
        {
            if (rate1 == null || rate2 == null) return null;

            var r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1));

            var r1 = InvertIfNeeded(rate1, CommonCurrency(rate1, rate2));
            var r2 = InvertIfNeeded(rate2, CommonCurrency(rate2, rate1));

            r.Rate = r2.Rate / r1.Rate;

            return r;
        }

        private static string DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null) return null;

            return r2.Contains(r1.ReferenceCurrencyCode) ? r1.SecondaryCurrencyCode : r1.ReferenceCurrencyCode;
        }

        private static string CommonCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            return r2.Contains(r1.ReferenceCurrencyCode) ? r1.ReferenceCurrencyCode : r2.Contains(r1.SecondaryCurrencyCode) ? r1.SecondaryCurrencyCode : null;
        }

        private static ExchangeRate InvertIfNeeded(ExchangeRate rate, string currencyCode)
        {
            return currencyCode.Equals(rate.ReferenceCurrencyCode) ? rate : rate.Inverse;
        }
    }
}
