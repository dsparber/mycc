using System;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;

namespace MyCC.Core.Currencies
{
    public static class CurrencyTools
    {
        public static Tuple<bool, Currency> Merge(this Currency c1, Currency c2)
        {
            if (string.IsNullOrWhiteSpace(c1?.Id) || string.IsNullOrWhiteSpace(c2.Name) || !string.Equals(c1.Id, c2.Id)) throw new InvalidOperationException("Different currencies can not be merged!");

            var updateName = !c1.Name.ToLower().Equals(c2.Name.ToLower());
            c1.BalanceSourceFlags = c1.BalanceSourceFlags.AddFlags(c2.BalanceSourceFlags);
            c1.Name = updateName ? c2.Name : c1.Name;

            return Tuple.Create(updateName || c1.BalanceSourceFlags != c2.BalanceSourceFlags, c1);
        }
    }
}