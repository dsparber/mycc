using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Settings;
using MyCC.Core.Account.Storage;

namespace MyCC.Core.Rates
{
    public static class ExchangeRateHelper
    {

        public static ExchangeRate GetRate(ExchangeRate rate, int? repository = null) => GetRate(new Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency), new Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency), repository);

        public static ExchangeRate GetRate(string currency1Id, string currency2Id, int? repository = null) => GetRate(currency1Id.ToCurrency(), currency2Id.ToCurrency(), repository);

        public static ExchangeRate GetRate(Currency currency1, Currency currency2, int? repository = null)
        {
            if (string.IsNullOrWhiteSpace(currency1?.Code) ||
                string.IsNullOrWhiteSpace(currency2?.Code)) throw new ArgumentException();

            if (currency1.Equals(currency2)) return new ExchangeRate(currency1.Id, currency2.Id, DateTime.Now, 1);


            var r1 = GetRateToBtc(currency1, repository);
            var r2 = GetRateToBtc(currency2, repository);

            if (r1 == null || r2 == null) return null;

            return GetCombinedRate(r1, r2);
        }

        private static ExchangeRate GetRateToBtc(Currency currency, int? repository = null)
        {
            if (currency.Code.Equals("BTC")) return new ExchangeRate(CurrencyConstants.Btc.Id, CurrencyConstants.Btc.Id, DateTime.Now, 1);

            if (currency.CryptoCurrency) return GetStoredRate(currency, CurrencyConstants.Btc);
            if (currency.Code.Equals("USD")) return GetStoredRate(currency, CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);

            var rate = currency.Code.Equals("EUR") ? new ExchangeRate(CurrencyConstants.Eur.Id, CurrencyConstants.Eur.Id, DateTime.Now, 1) : GetStoredRate(currency, CurrencyConstants.Eur);
            var eurBtc = GetStoredRate(CurrencyConstants.Eur, CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);

            if (eurBtc != null) return GetCombinedRate(rate, eurBtc);

            var eurUsd = GetStoredRate(CurrencyConstants.Eur, CurrencyConstants.Usd);
            var usdBtc = GetStoredRate(CurrencyConstants.Usd, CurrencyConstants.Btc, repository ?? ApplicationSettings.PreferredBitcoinRepository);
            return GetCombinedRate(rate, GetCombinedRate(eurUsd, usdBtc));
        }

        public static ExchangeRate GetStoredRate(Currency c1, Currency c2, int? repository = null)
        {
            var neededRate = new ExchangeRate(c1.Id, c2.Id, DateTime.Now);
            var rates = repository == null ?
                ExchangeRatesStorage.Instance.StoredRates.ToList() :
                ExchangeRatesStorage.Instance.Repositories.First(r => r.TypeId == repository).Rates;

            return rates.Find(c => c?.Equals(neededRate) ?? false) ??
                    rates.Find(c => c?.Equals(neededRate.Inverse) ?? false)?.Inverse;
        }

        public static Task FetchMissingRates(Action<double> progressCallback = null)
        {
            return FetchRates(NeededRates, true, progressCallback);
        }

        private static async Task FetchRates(IEnumerable<ExchangeRate> rates, bool excludeStoredRates = false, Action<double> progressCallback = null)
        {
            var neededRates = rates.SelectMany(GetNeededRates).Distinct();

            IEnumerable<ExchangeRate> neededToFetch;
            if (excludeStoredRates){
                var storedRates = ExchangeRatesStorage.Instance.StoredRates.ToList();
                neededToFetch = neededRates.Where(r => !(storedRates.Contains(r) || storedRates.Contains(r.Inverse))).ToList();
            }
            else{
                neededToFetch = neededRates;
            }

            var neededToFetchCount = neededToFetch.Count();
            if (neededToFetchCount == 0) return;

            var fetchedFixerIo = false;
            var fetchedCryptoToFiat = false;

            var progress = .0;
            foreach (var r in neededToFetch)
            {
                progressCallback?.Invoke(progress / neededToFetchCount); progress += 1;

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

        public static Task UpdateRates(IEnumerable<ExchangeRate> rates = null, Action<double> progressCallback = null)
        {
            return FetchRates(rates ?? NeededRates, progressCallback: progressCallback);
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
                    await ((ISingleRateRepository)r).FetchRate(new ExchangeRate(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id));
                    var eurRate = new ExchangeRate(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id);
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


            var r1 = GetNeededRatesToBtc(new Currency(rate.ReferenceCurrencyCode, rate.ReferenceCurrencyIsCryptoCurrency));
            var r2 = GetNeededRatesToBtc(new Currency(rate.SecondaryCurrencyCode, rate.SecondaryCurrencyIsCryptoCurrency));

            return r1.Concat(r2);
        }

        public static IEnumerable<ExchangeRate> NeededRates 
        {
            get
            {
                var referenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToList();
                var usedCurrencies = AccountStorage.UsedCurrencies
                                                   .Concat(ApplicationSettings.AllReferenceCurrencies)
                                                   .Concat(ApplicationSettings.WatchedCurrencies);
                
                return usedCurrencies.SelectMany(currencyId => referenceCurrencies.Select(referenceCurrencyId => new ExchangeRate(currencyId, referenceCurrencyId))).Distinct();
            }
        }

        private static IEnumerable<ExchangeRate> GetNeededRatesToBtc(Currency currency)
        {
            if (currency.Code.Equals("BTC")) return new List<ExchangeRate>();

            if (currency.CryptoCurrency) return new List<ExchangeRate> { new ExchangeRate(currency.Id, CurrencyConstants.Btc.Id) };

            return (currency.Equals(CurrencyConstants.Usd) ?
                new List<ExchangeRate> { new ExchangeRate(CurrencyConstants.Usd.Id, CurrencyConstants.Btc.Id) } :
                new List<ExchangeRate> {
                    new ExchangeRate(currency.Id, CurrencyConstants.Eur.Id),
                    new ExchangeRate(CurrencyConstants.Eur.Id, CurrencyConstants.Btc.Id)
                }).Where(r => r.ReferenceCurrencyCode != r.SecondaryCurrencyCode);
        }



        private static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
        {
            if (rate1 == null || rate2 == null) return null;

            var r = new ExchangeRate(DifferentCurrency(rate1, rate2).Id, DifferentCurrency(rate2, rate1).Id, new[] { rate1.LastUpdate, rate2.LastUpdate }.Min());

            var r1 = InvertIfNeeded(rate1, CommonCurrency(rate1, rate2));
            var r2 = InvertIfNeeded(rate2, CommonCurrency(rate2, rate1));

            if (r1.Rate != null && r1.Rate != 0) // To avoid divided by 0
            {
                r.Rate = r2.Rate / r1.Rate;
            }

            return r;
        }

        private static Currency DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
        {
            if (CommonCurrency(r1, r2) == null) return null;

            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ?
                     new Currency(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) :
                     new Currency(r1.ReferenceCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency);
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
