using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Newtonsoft.Json;

namespace MyCC.Ui.Get.Implementations
{
    internal class AssetsOverviewData : IAssetsOverviewData, ICachedData
    {
        private Dictionary<string, AssetsGraphItem.Data[]> _graphItems;
        private Dictionary<string, List<AssetItem>> _tableItems;
        private Dictionary<string, CoinHeaderItem> _headers;
        private Dictionary<string, List<SortButtonItem>> _sortButtons;
        private DateTime _lastUpdate;
        private bool? _isDataAvailable;

        private void Init()
        {
            _graphItems = new Dictionary<string, AssetsGraphItem.Data[]>();
            _tableItems = new Dictionary<string, List<AssetItem>>();
            _headers = new Dictionary<string, CoinHeaderItem>();
            _sortButtons = new Dictionary<string, List<SortButtonItem>>();
            _lastUpdate = DateTime.MinValue;
            _isDataAvailable = null;
        }

        public AssetsOverviewData()
        {
            Init();
        }

        public List<AssetItem> TableItemsFor(string currencyId) => _tableItems.TryGetValue(currencyId, out var value) && value != null ? value : _tableItems[currencyId] = LoadItems(currencyId);
        private Array GraphItemsFor(string currencyId) => _graphItems.TryGetValue(currencyId, out var value) && value != null ? value : _graphItems[currencyId] = LoadGraphItems(currencyId);

        public CoinHeaderItem HeaderFor(string currencyId) => _headers.TryGetValue(currencyId, out var value) && value != null ? value : _headers[currencyId] = LoadHeaders(currencyId);

        public List<SortButtonItem> SortButtonsFor(string currencyId) => _sortButtons.TryGetValue(currencyId, out var value) && value != null ? value : _sortButtons[currencyId] = LoadSortButtons(currencyId);

        public DateTime LastUpdate => _lastUpdate = _lastUpdate != DateTime.MinValue ? _lastUpdate : GetLastUpdate();

        public bool IsDataAvailable => (_isDataAvailable = _isDataAvailable ?? AccountStorage.Instance.AllElements.Any()).Value;
        public bool IsGraphDataAvailable => GraphItemsFor(CurrencyConstants.Btc.Id)?.Length > 0;

        public string GrapItemsJsFor(string currencyId)
        {
            var data = JsonConvert.SerializeObject(GraphItemsFor(currencyId) ?? new AssetsGraphItem.Data[] { });
            var accountStrings = JsonConvert.SerializeObject(new[] { StringUtils.TextResolver.OneAccount, StringUtils.TextResolver.Accounts });
            var currenciesStrings = JsonConvert.SerializeObject(new[] { StringUtils.TextResolver.OneCurrency, StringUtils.TextResolver.Currencies });
            var furtherString = StringUtils.TextResolver.Further;
            var noDataString = StringUtils.TextResolver.NoDataToDisplay;
            var roundMoney = ApplicationSettings.RoundMoney.ToString();
            var baseCurrency = currencyId.Code();
            var culture = CultureInfo.CurrentCulture.ToString();

            var js = $"showChart({data}, {accountStrings}, {currenciesStrings}, \"{furtherString}\", \"{noDataString}\", \"{baseCurrency}\", \"{roundMoney}\", \"{culture}\");";
            return js;
        }

        private static DateTime GetLastUpdate()
        {
            var online = AccountStorage.Instance.AllElements.Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.Instance.AllElements.Any() ? AccountStorage.Instance.AllElements.Max(a => a.LastUpdate) : DateTime.Now;
            var ratesTime = MyccUtil.Rates.LastUpdate();

            return online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;
        }

        private static SortOrder SortOrder
        {
            get => ApplicationSettings.SortOrderAccounts;
            set => ApplicationSettings.SortOrderAccounts = value;
        }
        private static SortDirection SortDirection
        {
            get => ApplicationSettings.SortDirectionAccounts;
            set => ApplicationSettings.SortDirectionAccounts = value;
        }

        private static CoinHeaderItem LoadHeaders(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;

            var enabledAccounts = AccountStorage.EnabledAccounts.ToList();
            Money GetReferenceValue(string cId)
            {
                var amount = enabledAccounts.GroupBy(account => account.Money.Currency)
                .Sum(group => group.Sum(account => account.Money.Amount) * MyccUtil.Rates.GetRate(new RateDescriptor(group.Key.Id, cId))?.Rate ?? 0);
                return new Money(amount, cId.Find());
            }

            var referenceMoney = GetReferenceValue(currencyId);
            var additionalRefs = ApplicationSettings.MainCurrencies.Except(new[] { currencyId }).Select(GetReferenceValue);

            return new CoinHeaderItem(referenceMoney, additionalRefs);
        }

        private static AssetsGraphItem.Data[] LoadGraphItems(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;

            return AccountStorage.AccountsGroupedByCurrency
                .Select(e => new AssetsGraphItem.Data(e, currencyId.Find()))
                .Where(d => d.Value > 0)
                .OrderByDescending(d => d.Value)
                .ToArray();
        }

        private static List<AssetItem> LoadItems(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return new List<AssetItem>();

            Money GetReference(Money m) => new Money(m.Amount * (MyccUtil.Rates.GetRate(new RateDescriptor(m.Currency.Id, currencyId))?.Rate ?? 0), currencyId.Find());

            var items = AccountStorage.AccountsGroupedByCurrency.ToList();
            var enabled = items.Select(group =>
            {
                var money = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);
                return new AssetItem(money, GetReference(money), true);
            }).ToList();

            var disabled = items.Select(group =>
            {
                var money = new Money(group.Sum(a => a.IsEnabled ? 0 : a.Money.Amount), group.Key);
                return new AssetItem(money, GetReference(money), false);
            }).Where(i => !enabled.Any(x => x.CurrencyId.Equals(i.CurrencyId))).ToList();

            return ApplySort(enabled, disabled);
        }

        private static List<AssetItem> ApplySort(List<AssetItem> tableItems) => ApplySort(tableItems.Where(i => i.Enabled), tableItems.Where(i => !i.Enabled));

        private static List<AssetItem> ApplySort(IEnumerable<AssetItem> enabledItems, IEnumerable<AssetItem> disabledItems)
        {
            var alphabetical = SortOrder == SortOrder.Alphabetical;
            var byValue = SortOrder == SortOrder.ByValue;

            object SortLambda(AssetItem r) => alphabetical ? r.CurrencyCode as object : byValue ? r.ReferenceAmount : r.Amount;

            return enabledItems.OrderByWithDirection(SortLambda, SortDirection == SortDirection.Ascending)
                .Concat(disabledItems.OrderByWithDirection(SortLambda, SortDirection == SortDirection.Ascending))
                .ToList();
        }

        private List<SortButtonItem> LoadSortButtons(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;

            return new[]
            {
                new SortButtonItem
                {
                    Text = StringUtils.TextResolver.Currency,
                    SortAscending =SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.Alphabetical),
                    RightAligned = false,
                    OnClick = () => OnSort(SortOrder.Alphabetical)
                },
                new SortButtonItem
                {
                    Text = StringUtils.TextResolver.Amount,
                    SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.ByUnits),
                    RightAligned = true,
                    OnClick = () => OnSort(SortOrder.ByUnits)
                },
                new SortButtonItem
                {
                    Text = string.Format(StringUtils.TextResolver.AsCurrency, currencyId.Code()),
                    SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.ByValue),
                    RightAligned = true,
                    OnClick = () => OnSort(SortOrder.ByValue)
                }
            }.ToList();
        }

        private void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            SortAndNotify();
        }

        private void SortAndNotify()
        {
            foreach (var key in _tableItems.ToList().Select(pair => pair.Key))
            {
                _tableItems[key] = _tableItems[key] != null ? ApplySort(_tableItems[key]) : null;
            }
            _sortButtons = new Dictionary<string, List<SortButtonItem>>();
            Messaging.Sort.Assets.Send();
        }

        public void ResetCache() => Init();
    }
}