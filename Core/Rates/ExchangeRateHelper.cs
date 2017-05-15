using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Settings;

namespace MyCC.Core.Rates
{
    public static class ExchangeRateHelper
    {

        public static ExchangeRate GetRate(ExchangeRate rate, int? repository = null) => GetRate(new Currencies.Model.Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency), new Currencies.Model.Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency), repository);

        public static ExchangeRate GetRate(Currencies.Model.Currency currency1, Currencies.Model.Currency currency2, int? repository = null)
        {
            if (string.IsNullOrWhiteSpace(currency1?.Code) ||
                string.IsNullOrWhiteSpace(currency2?.Code)) throw new ArgumentException();

            if (currency1.Equals(currency2)) return new ExchangeRate(currency1, currency2, DateTime.Now, 1);


            var r1 = GetRateToBtc(currency1, repository);
            var r2 = GetRateToBtc(currency2, repository);

            if (r1 == null || r2 == null) return null;

            return GetCombinedRate(r1, r2);
        }

        private static ExchangeRate GetRateToBtc(Currencies.Model.Currency currency, int? repository = null)
        {
            if (currency.Code.Equals("BTC")) return new ExchangeRate(Currencies.CurrencyConstants.Btc, Currencies.CurrencyConstants.Btc, DateTime.Now, 1);

            if (currency.CryptoCurrency) return GetStoredRate(currency, Currencies.CurrencyConstants.Btc);
            if (currency.Code.Equals("USD")) return GetStoredRate(currency, Currencies.CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);

            var rate = currency.Code.Equals("EUR") ? new ExchangeRate(Currencies.CurrencyConstants.Eur, Currencies.CurrencyConstants.Eur, DateTime.Now, 1) : GetStoredRate(currency, Currencies.CurrencyConstants.Eur);
            var eurBtc = GetStoredRate(Currencies.CurrencyConstants.Eur, Currencies.CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);

            if (eurBtc != null) return GetCombinedRate(rate, eurBtc);

            var eurUsd = GetStoredRate(Currencies.CurrencyConstants.Eur, Currencies.CurrencyConstants.Usd);
            var usdBtc = GetStoredRate(Currencies.CurrencyConstants.Usd, Currencies.CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);
            return GetCombinedRate(rate, GetCombinedRate(eurUsd, usdBtc));
        }

        public static ExchangeRate GetStoredRate(Currencies.Model.Currency c1, Currencies.Model.Currency c2, int? repository = null)
        {
            var neededRate = new ExchangeRate(c1, c2, DateTime.Now);
            var rates = repository == null ?
                ExchangeRatesStorage.Instance.StoredRates.ToList() :
                ExchangeRatesStorage.Instance.Repositories.First(r => r.TypeId == repository).Rates;

            return rates.Find(c => c?.Equals(neededRate) ?? false) ??
                    rates.Find(c => c?.Equals(neededRate.Inverse) ?? false)?.Inverse;
        }

        public static Task FetchMissingRatesFor(IEnumerable<ExchangeRate> rates, Action<double> progressCallback = null)
        {
            var missingRates = rates.SelectMany(GetNeededRates).Distinct();
            return FetchMissingRates(missingRates, progressCallback);
        }

        private static async Task FetchMissingRates(IEnumerable<ExchangeRate> missingRates, Action<double> progressCallback = null)
        {
            var storedRates = ExchangeRatesStorage.Instance.StoredRates.ToList();
            var neededToFetch = missingRates.Where(r => !(storedRates.Contains(r) || storedRates.Contains(r.Inverse))).ToList();

            var fetchedFixerIo = false;
            var fetchedCryptoToFiat = false;

            var progress = .0;
            foreach (var r in neededToFetch)
            {
                progressCallback?.Invoke(progress / neededToFetch.Count); progress += 1;

                if (ExchangeRatesStorage.FixerIo.IsAvailable(r) || ExchangeRatesStorage.FixerIo.IsAvailable(r.Inverse))
                {
                    if (fetchedFixerIo) continue;

                    fetchedFixerIo = true;
                    await ExchangeRatesStorage.FixerIo.FetchRates();
                }
                else if (ExchangeRatesStorage.PreferredBtcRepository.IsAvailable(r) || ExchangeRatesStorage.PreferredBtcRepository.IsAvailable(r.Inverse))
                {
                    if (fetchedCryptoToFiat) continue;

                    fetchedCryptoToFiat = true;
                    if (ExchangeRatesStorage.PreferredBtcRepository is IMultipleRatesRepository)
                    {
                        await ((IMultipleRatesRepository)ExchangeRatesStorage.PreferredBtcRepository).FetchRates();
                    }
                    else
                    {
                        await ((ISingleRateRepository)ExchangeRatesStorage.PreferredBtcRepository).FetchRate(ExchangeRatesStorage.PreferredBtcRepository.IsAvailable(r) ? r : r.Inverse);
                    }
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
            progressCallback?.Invoke(1);
        }

        public static async Task UpdateRates(IEnumerable<ExchangeRate> rates, Action<double> progressCallback = null)
        {
            var updatedRepositories = new List<int>();

            var ratesToUpdate = rates.SelectMany(GetNeededRates).ToList();

            var progress = .0;
            foreach (var r in ratesToUpdate)
            {
                progressCallback?.Invoke(progress / ratesToUpdate.Count); progress += 1;

                var supportedRepo = ExchangeRatesStorage.PreferredBtcRepository.IsAvailable(r) || ExchangeRatesStorage.PreferredBtcRepository.IsAvailable(r.Inverse) ? ExchangeRatesStorage.PreferredBtcRepository : null;
                supportedRepo = supportedRepo ?? ExchangeRatesStorage.Instance.Repositories.FirstOrDefault(repo => repo.IsAvailable(r) || repo.IsAvailable(r.Inverse));

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
                    await ((ISingleRateRepository)supportedRepo).FetchRate(supportedRepo.IsAvailable(r) ? r : r.Inverse);
                }
            }
            progressCallback?.Invoke(1);
        }

        public static async Task FetchDollarBitcoinRates(Action<double> progressCallback = null)
        {
            var repos = ExchangeRatesStorage.Instance.Repositories.Where(r => r.RatesType == RateRepositoryType.CryptoToFiat).ToList();

            var progress = .0;
            foreach (var r in repos)
            {

                var multiRepo = r as IMultipleRatesRepository;
                if (multiRepo != null)
                {
                    await multiRepo.FetchRates();
                }
                else
                {
                    await ((ISingleRateRepository)r).FetchRate(new ExchangeRate(Currencies.CurrencyConstants.Btc, Currencies.CurrencyConstants.Usd));
                    var eurRate = new ExchangeRate(Currencies.CurrencyConstants.Btc, Currencies.CurrencyConstants.Eur);
                    if (r.IsAvailable(eurRate)) await ((ISingleRateRepository)r).FetchRate(eurRate);
                }

                progress += 1;
                progressCallback?.Invoke(progress / repos.Count);
            }
        }

        public static IEnumerable<ExchangeRate> GetNeededRates(ExchangeRate rate)
        {
            if (string.IsNullOrWhiteSpace(rate?.ReferenceCurrencyCode) ||
                string.IsNullOrWhiteSpace(rate.SecondaryCurrencyCode)) throw new ArgumentNullException();

            if (rate.ReferenceCurrencyCode.Equals(rate.SecondaryCurrencyCode)) return new List<ExchangeRate>();


            var r1 = GetNeededRatesToBtc(new Currencies.Model.Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency));
            var r2 = GetNeededRatesToBtc(new Currencies.Model.Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency));

            return r1.Concat(r2);
        }

        private static IEnumerable<ExchangeRate> GetNeededRatesToBtc(Currencies.Model.Currency currency)
        {
            if (currency.Code.Equals("BTC")) return new List<ExchangeRate>();

            if (currency.CryptoCurrency) return new List<ExchangeRate> { new ExchangeRate(currency, Currencies.CurrencyConstants.Btc) };

            return (currency.Equals(Currencies.CurrencyConstants.Usd) ?
                new List<ExchangeRate> { new ExchangeRate(Currencies.CurrencyConstants.Usd, Currencies.CurrencyConstants.Btc) } :
                new List<ExchangeRate> {
                    new ExchangeRate(currency, Currencies.CurrencyConstants.Eur),
                    new ExchangeRate(Currencies.CurrencyConstants.Eur, Currencies.CurrencyConstants.Btc)
                }).Where(r => r.ReferenceCurrencyCode != r.SecondaryCurrencyCode);
        }



        private static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
        {
            if (rate1 == null || rate2 == null) return null;

            var r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1), new[] { rate1.LastUpdate, rate2.LastUpdate }.Min());

            var r1 = InvertIfNeeded(rate1, CommonCurrency(rate1, rate2));
            var r2 = InvertIfNeeded(rate2, CommonCurrency(rate2, rate1));

            if (r1.Rate != null && r1.Rate != 0) // To avoid divided by 0
            {
                r.Rate = r2.Rate / r1.Rate;
            }

            return r;
        }

        private static Currencies.Model.Currency DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null) return null;

            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ?
                     new Currencies.Model.Currency(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) :
                     new Currencies.Model.Currency(r1.ReferenceCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency);
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
