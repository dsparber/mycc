using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;

namespace MyCC.Core.Currencies
{
    public static class CurrencyHelper
    {
        public static Currency Find(this string id)
        {
            CurrencyStorage.Instance.CurrencyDictionary.TryGetValue(id, out var currency);
            return currency ?? CurrencyStorage.Instance.Currencies.FirstOrDefault(c => c.Id.Equals(id)) ?? id.ToCurrency();
        }

        public static Currency ToCurrency(this string currencyId) =>
            new Currency(currencyId.Substring(0, currencyId.Length - 1), null, currencyId[currencyId.Length - 1] == '1');

        public static Currency Find(string code, bool isCrypto) => Find($"{code}{(isCrypto ? 1 : 0)}");
        public static Currency Find(this Currency currency) => currency.Id.Find();

        public static string FindName(this string currencyId) => currencyId.Find().Name;
        public static string FindName(this Currency currency) => currency.Id.Find().Name;
        public static string Code(this string currencyId) => currencyId.ToCurrency().Code;
        public static bool IsCrypto(this string currencyId) => currencyId.ToCurrency().IsCrypto;
        public static bool IsFiat(this string currencyId) => currencyId.ToCurrency().IsFiat;

        public static IEnumerable<Currency> Currencies(this int flags) => CurrencyStorage.Instance.Currencies.Where(c => c.BalanceSourceFlags.IsSet(flags));
        public static IEnumerable<string> CurrencyIds(this int flags) => flags.Currencies().Select(currency => currency.Id);

        public static Tuple<bool, Currency> Merge(this Currency c1, Currency c2)
        {
            if (string.IsNullOrWhiteSpace(c1?.Id) || string.IsNullOrWhiteSpace(c2.Name) || !string.Equals(c1.Id, c2.Id)) throw new InvalidOperationException("Different currencies can not be merged!");

            var updateName = !c1.Name.ToLower().Equals(c2.Name.ToLower());
            c1.BalanceSourceFlags = c1.BalanceSourceFlags.AddFlags(c2.BalanceSourceFlags);
            c1.Name = updateName ? c2.Name : c1.Name;

            return Tuple.Create(updateName || c1.BalanceSourceFlags != c2.BalanceSourceFlags, c1);
        }

        public static bool IsSet(this Currency currency, int sourceFlag) =>
            currency.BalanceSourceFlags.IsSet(sourceFlag);
    }
}