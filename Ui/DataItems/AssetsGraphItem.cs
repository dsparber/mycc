using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using Newtonsoft.Json;

namespace MyCC.Ui.DataItems
{
    public class AssetsGraphItem
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
                var rate = new ExchangeRate(group.Key, referenceCurrency);
                rate = ExchangeRateHelper.GetRate(rate) ?? rate;

                var totalMoney = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);

                Label = group.Key.Code;
                Name = group.Key.Name;
                Value = totalMoney.Amount * rate.Rate ?? 0;
                Money = totalMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                Reference = new Money(Value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
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
                Value = account.IsEnabled ? account.Money.Amount * rate.Rate ?? 0 : 0;
                Label = account.Name;
                Money = (account.IsEnabled ? account.Money : new Money(0, account.Money.Currency)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                Reference = new Money(Value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                Id = account.Id;
            }
        }
    }
}