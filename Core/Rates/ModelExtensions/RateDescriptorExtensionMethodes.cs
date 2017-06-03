using System;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.ModelExtensions
{
    public static class RateDescriptorExtensionMethodes
    {
        public static RateDescriptor Inverse(this RateDescriptor exchangeRate)
        => new RateDescriptor(exchangeRate.SecondaryCurrencyId, exchangeRate.ReferenceCurrencyId);

        public static bool HasEqualCurrencies(this RateDescriptor rateDescriptor) =>
            rateDescriptor.ReferenceCurrencyId.Equals(rateDescriptor.SecondaryCurrencyId);

        public static bool HasOnlyFiatCurrencies(this RateDescriptor rateDescriptor) =>
            !rateDescriptor.ReferenceCurrencyId.IsCrypto() && !rateDescriptor.SecondaryCurrencyId.IsCrypto();

        public static bool HasOnlyCryptoCurrencies(this RateDescriptor rateDescriptor) =>
            rateDescriptor.ReferenceCurrencyId.IsCrypto() && rateDescriptor.SecondaryCurrencyId.IsCrypto();

        public static bool HasCryptoAndFiatCurrency(this RateDescriptor rateDescriptor) =>
            !rateDescriptor.HasOnlyFiatCurrencies() && !rateDescriptor.HasOnlyCryptoCurrencies();

        public static string GetFiatCurrencyId(this RateDescriptor rateDescriptor)
        {
            if (!rateDescriptor.HasCryptoAndFiatCurrency())
                throw new InvalidOperationException("Descriptor does not consist of a fiat and crypto currency");

            return rateDescriptor.ReferenceCurrencyId.IsFiat() ? rateDescriptor.ReferenceCurrencyId : rateDescriptor.SecondaryCurrencyId;
        }

        public static string GetCryptoCurrencyId(this RateDescriptor rateDescriptor)
        {
            if (!rateDescriptor.HasCryptoAndFiatCurrency())
                throw new InvalidOperationException("Descriptor does not consist of a fiat and crypto currency");

            return rateDescriptor.ReferenceCurrencyId.IsCrypto() ? rateDescriptor.ReferenceCurrencyId : rateDescriptor.SecondaryCurrencyId;
        }


        public static bool ContainsCurrency(this RateDescriptor rateDescriptor, string currencyId)
        => rateDescriptor.ReferenceCurrencyId.Equals(currencyId) ||
           rateDescriptor.SecondaryCurrencyId.Equals(currencyId);

        public static string GetCurrencyApartFrom(this RateDescriptor rateDescriptor, string currencyId)
            => rateDescriptor.ReferenceCurrencyId.Equals(currencyId) ? rateDescriptor.SecondaryCurrencyId : rateDescriptor.ReferenceCurrencyId;

        public static bool IsBtcToUsdOrEur(this RateDescriptor rateDescriptor) =>
            rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id) &&
            (rateDescriptor.ContainsCurrency(CurrencyConstants.Usd.Id) || rateDescriptor.ContainsCurrency(CurrencyConstants.Eur.Id));

        public static bool IsBtcToUsd(this RateDescriptor rateDescriptor) =>
            rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id) &&
            rateDescriptor.ContainsCurrency(CurrencyConstants.Usd.Id);
    }
}