using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates.Models;
using Newtonsoft.Json;

namespace MyCC.Ui.DataItems
{
    public static class AssetsGraphItem
    {
        public class Data
        {
            [JsonProperty("value")]
            public readonly decimal Value;

            [JsonProperty("label")]
            public readonly string Label;

            [JsonProperty("money")]
            public readonly string Money;

            [JsonProperty("reference")]
            public readonly string Reference;

            [JsonProperty("name")]
            public readonly string Name;

            [JsonProperty("accounts")]
            public readonly AccountData[] Accounts;

            public Data(IGrouping<Currency, Account> group, Currency referenceCurrency)
            {
                var rate = MyccUtil.Rates.GetRate(new RateDescriptor(group.Key.Id, referenceCurrency.Id));

                var totalMoney = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);

                Label = group.Key.Code;
                Name = group.Key.Name;
                Value = new Money(totalMoney.Amount * rate?.Rate ?? 0, referenceCurrency).Amount;
                Money = totalMoney.TwoDigits();
                Reference = new Money(Value, referenceCurrency).TwoDigits();
                Accounts =
                    group.Select(a => new AccountData(a, rate, referenceCurrency))
                        .Where(d => d.Value > 0)
                        .OrderByDescending(d => d.Value)
                        .ToArray();
            }
        }
        public class AccountData
        {
            [JsonProperty("value")]
            public readonly decimal Value;

            [JsonProperty("label")]
            public readonly string Label;

            [JsonProperty("money")]
            public readonly string Money;

            [JsonProperty("reference")]
            public readonly string Reference;

            [JsonProperty("id")]
            public readonly int Id;

            public AccountData(Account account, ExchangeRate rate, Currency referenceCurrency)
            {
                Value = account.IsEnabled ? account.Money.Amount * rate?.Rate ?? 0 : 0;
                Label = account.Name;
                Money = (account.IsEnabled ? account.Money : new Money(0, account.Money.Currency)).TwoDigits();
                Reference = new Money(Value, referenceCurrency).TwoDigits();
                Id = account.Id;
            }
        }
    }
}