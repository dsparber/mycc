using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Data.Get
{
    public class AssetsViewData
    {
        private Dictionary<Currency, AssetsGraphItem.Data[]> _graphItems;
        public Dictionary<Currency, List<AssetItem>> Items { get; private set; }
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }
        public Dictionary<Currency, DateTime> LastUpdate { get; private set; }

        private readonly Context _context;
        public bool IsDataAvailable => Items != null && Items.Count > 0 && Items.Min(i => i.Value.Count) > 0;
        public bool IsGraphDataAvailable => _graphItems != null && _graphItems.Count > 0 && _graphItems.Min(i => i.Value.Length) > 0;


        public AssetsViewData(Context context)
        {
            _context = context;
        }

        public string JsDataString(Currency currency)
        {
            var data = JsonConvert.SerializeObject(_graphItems[currency]);
            var accountStrings = JsonConvert.SerializeObject(new[] { _context.Resources.GetString(Resource.String.OneAccount), _context.Resources.GetString(Resource.String.Accounts) });
            var currenciesStrings = JsonConvert.SerializeObject(new[] { _context.Resources.GetString(Resource.String.OneCurrency), _context.Resources.GetString(Resource.String.Currencies) });
            var furtherString = _context.Resources.GetString(Resource.String.Further);
            var noDataString = _context.Resources.GetString(Resource.String.NoDataToDisplay);
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

        private static Dictionary<Currency, DateTime> GetLastUpdate() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
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

        private static Dictionary<Currency, CoinHeaderData> LoadHeaders() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            var amount = AccountStorage.EnabledAccounts.Sum(a => a.Money.Amount * ExchangeRateHelper.GetRate(a.Money.Currency, c)?.Rate ?? 0);
            var referenceMoney = new Money(amount, c);

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { c })
                .Select(x => new Money(amount * ExchangeRateHelper.GetRate(c, x)?.Rate ?? 0, x))
                .ToList();

            return new CoinHeaderData(referenceMoney, additionalRefs);
        });

        private static Dictionary<Currency, AssetsGraphItem.Data[]> LoadGraphItems() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
             AccountStorage.AccountsGroupedByCurrency
                        .Select(e => new AssetsGraphItem.Data(e, c))
                        .Where(d => d.Value > 0)
                        .OrderByDescending(d => d.Value)
                        .ToArray());

        private static Dictionary<Currency, List<AssetItem>> LoadItems() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            Func<Money, Money> getReference = m => new Money(m.Amount * (ExchangeRateHelper.GetRate(m.Currency, c)?.Rate ?? 0), c);

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

        private Dictionary<Currency, List<SortButtonItem>> LoadSortButtons() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Currency),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            },
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Amount),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByUnits),
                TextGravity = GravityFlags.Right,
                OnClick = () =>OnSort(SortOrder.ByUnits)
            },
            new SortButtonItem
            {
                Text = string.Format(_context.Resources.GetString(Resource.String.AsCurrency), c.Code),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
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
            Items = ApplicationSettings.MainCurrencies.ToDictionary(c => c, c => ApplySort(Items[c].Where(i => i.Enabled), Items[c].Where(i => !i.Enabled)));
            SortButtons = LoadSortButtons();
            Messaging.UiUpdate.AssetsTable.Send();
        }
    }
}