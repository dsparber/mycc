using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Newtonsoft.Json;

namespace MyCC.Ui.ViewData
{
    public class AssetsViewData
    {
        private Dictionary<Currency, AssetsGraphItem.Data[]> _graphItems;
        public Dictionary<Currency, List<AssetItem>> Items { get; private set; }
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }
        public Dictionary<Currency, DateTime> LastUpdate { get; private set; }

        public bool IsDataAvailable => Items != null && Items.Count > 0 && Items.Min(i => i.Value.Count) > 0;
        public bool IsGraphDataAvailable => _graphItems != null && _graphItems.Count > 0 && _graphItems.Min(i => i.Value.Length) > 0;

        public string JsDataString(Currency currency)
        {
            var data = JsonConvert.SerializeObject(_graphItems[currency]);
            var accountStrings = JsonConvert.SerializeObject(new[] { StringHelper.TextResolver.OneAccount, StringHelper.TextResolver.Accounts });
            var currenciesStrings = JsonConvert.SerializeObject(new[] { StringHelper.TextResolver.OneCurrency, StringHelper.TextResolver.Currencies });
            var furtherString = StringHelper.TextResolver.Further;
            var noDataString = StringHelper.TextResolver.NoDataToDisplay;
            var roundMoney = ApplicationSettings.RoundMoney.ToString();
            var baseCurrency = currency.Code;
            var culture = CultureInfo.CurrentCulture.ToString();

            return $"showChart({data}, {accountStrings}, {currenciesStrings}, \"{furtherString}\", \"{noDataString}\", \"{baseCurrency}\", \"{roundMoney}\", \"{culture}\");";
        }

        public void UpdateRateItems()
        {
            Items = LoadItems();
            _graphItems = LoadGraphItems();
            Headers = LoadHeaders();
            SortButtons = LoadSortButtons();
            LastUpdate = GetLastUpdate();

            Messaging.UiUpdate.AssetsTable.Send();
        }

        private static Dictionary<Currency, DateTime> GetLastUpdate() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyStorage.Find, c =>
        {
            var online = AccountStorage.Instance.AllElements.Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.Instance.AllElements.Any() ? AccountStorage.Instance.AllElements.Max(a => a.LastUpdate) : DateTime.Now;
            var ratesTime = AccountStorage.NeededRates.Distinct().Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            return online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;
        });

        private static SortOrder SortOrder
        {
            get { return ApplicationSettings.SortOrderAccounts; }
            set { ApplicationSettings.SortOrderAccounts = value; }
        }
        private static SortDirection SortDirection
        {
            get { return ApplicationSettings.SortDirectionAccounts; }
            set { ApplicationSettings.SortDirectionAccounts = value; }
        }

        private static Dictionary<Currency, CoinHeaderData> LoadHeaders() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyStorage.Find, c =>
        {
            var amount = AccountStorage.EnabledAccounts.Sum(a => a.Money.Amount * ExchangeRateHelper.GetRate(a.Money.Currency.Id, c)?.Rate ?? 0);
            var referenceMoney = new Money(amount, CurrencyStorage.Find(c));

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { c })
                .Select(x => new Money(amount * ExchangeRateHelper.GetRate(c, x)?.Rate ?? 0, CurrencyStorage.Find(x)))
                .ToList();

            return new CoinHeaderData(referenceMoney, additionalRefs);
        });

        private static Dictionary<Currency, AssetsGraphItem.Data[]> LoadGraphItems() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyStorage.Find, c =>
             AccountStorage.AccountsGroupedByCurrency
                        .Select(e => new AssetsGraphItem.Data(e, CurrencyStorage.Find(c)))
                        .Where(d => d.Value > 0)
                        .OrderByDescending(d => d.Value)
                        .ToArray());

        private static Dictionary<Currency, List<AssetItem>> LoadItems() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyStorage.Find, c =>
        {
            Func<Money, Money> getReference = m => new Money(m.Amount * (ExchangeRateHelper.GetRate(m.Currency.Id, c)?.Rate ?? 0), CurrencyStorage.Find(c));

            var items = AccountStorage.AccountsGroupedByCurrency.ToList();
            var enabled = items.Select(group =>
            {
                var money = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);
                return new AssetItem(money, getReference(money), true);
            }).Where(i => i.Value.Amount > 0).ToList();

            var disabled = items.Select(group =>
            {
                var money = new Money(group.Sum(a => a.IsEnabled ? 0 : a.Money.Amount), group.Key);
                return new AssetItem(money, getReference(money), false);
            }).Where(i => i.Value.Amount > 0 && !enabled.Any(x => x.Value.Currency.Equals(i.Value.Currency))).ToList();

            return ApplySort(enabled, disabled);
        });

        private static List<AssetItem> ApplySort(IEnumerable<AssetItem> enabledItems, IEnumerable<AssetItem> disabledItems)
        {
            var alphabetical = SortOrder == SortOrder.Alphabetical;
            var byValue = SortOrder == SortOrder.ByValue;

            Func<AssetItem, object> sortLambda = r => alphabetical ? r.CurrencyCode as object : byValue ? r.ReferenceValue.Amount : r.Value.Amount;

            return enabledItems.OrderByWithDirection(sortLambda, SortDirection == SortDirection.Ascending)
                .Concat(disabledItems.OrderByWithDirection(sortLambda, SortDirection == SortDirection.Ascending))
                .ToList();
        }

        private Dictionary<Currency, List<SortButtonItem>> LoadSortButtons() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyStorage.Find, c => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Currency,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            },
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Amount,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByUnits),
                RightAligned = true,
                OnClick = () =>OnSort(SortOrder.ByUnits)
            },
            new SortButtonItem
            {
                Text = string.Format(StringHelper.TextResolver.AsCurrency, new Currency(c).Code),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSort(SortOrder.ByValue)
            }
        });

        private void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            SortAndNotify();
        }

        private void SortAndNotify()
        {
            Items = ApplicationSettings.MainCurrencies.Select(CurrencyStorage.Find).ToDictionary(c => c, c => ApplySort(Items[c].Where(i => i.Enabled), Items[c].Where(i => !i.Enabled)));
            SortButtons = LoadSortButtons();
            Messaging.UiUpdate.AssetsTable.Send();
        }
    }
}