using System;
using System.Collections.Generic;
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

namespace MyCC.Ui.Get.Implementations
{
    internal class RatesOverviewData : IRatesOverviewData, ICachedData
    {
        private static Dictionary<string, List<RateItem>> _items;
        private Dictionary<string, CoinHeaderItem> _headers;
        private Dictionary<string, List<SortButtonItem>> _sortButtons;
        private DateTime _lastUpdate;

        private void Init()
        {
            _items = new Dictionary<string, List<RateItem>>();
            _headers = new Dictionary<string, CoinHeaderItem>();
            _sortButtons = new Dictionary<string, List<SortButtonItem>>();
            _lastUpdate = DateTime.MinValue;
        }

        public void ResetCache() => Init();

        public RatesOverviewData()
        {
            Init();
        }

        public bool IsDataAvailable => EnabledCurrencyIds.Any();

        public List<RateItem> RateItemsFor(string currencyId) => _items.TryGetValue(currencyId, out var value) && value != null ? value : _items[currencyId] = LoadRateItems(currencyId);

        public CoinHeaderItem HeaderFor(string currencyId) => _headers.TryGetValue(currencyId, out var value) && value != null ? value : _headers[currencyId] = LoadRateHeaders(currencyId);

        public List<SortButtonItem> SortButtonsFor(string currencyId) => _sortButtons.TryGetValue(currencyId, out var value) && value != null ? value : _sortButtons[currencyId] = LoadSortButtons(currencyId);

        public DateTime LastUpdate => _lastUpdate = _lastUpdate != DateTime.MinValue ? _lastUpdate : _lastUpdate = MyccUtil.Rates.LastUpdate();

        public IEnumerable<string> EnabledCurrencyIds => AccountStorage.UsedCurrencies
            .Concat(ApplicationSettings.AllReferenceCurrencies)
            .Concat(ApplicationSettings.WatchedCurrencies)
            .Except(ApplicationSettings.DisabledCurrencyIds)
            .Distinct()
            .Where(c => c != null);

        private static CoinHeaderItem LoadRateHeaders(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;

            var rate = MyccUtil.Rates.GetRate(new RateDescriptor(CurrencyConstants.Btc.Id, currencyId));
            var referenceMoney = new Money(rate?.Rate ?? 0, currencyId.Find());

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { currencyId })
                .Select(x => new Money(MyccUtil.Rates.GetRate(new RateDescriptor(CurrencyConstants.Btc.Id, x))?.Rate ?? 0, x.Find()));

            return new CoinHeaderItem(referenceMoney, additionalRefs);
        }

        private List<RateItem> LoadRateItems(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;

            decimal GetReference(string referenceCurrencyId) => MyccUtil.Rates.GetRate(new RateDescriptor(referenceCurrencyId, currencyId))?.Rate ?? 0;

            return ApplySort(EnabledCurrencyIds.Except(new[] { currencyId }).Select(id => new RateItem(id, GetReference(id))));
        }

        private static List<RateItem> ApplySort(IEnumerable<RateItem> items)
        {
            var alphabetical = ApplicationSettings.SortOrderRates == SortOrder.Alphabetical;

            return items.OrderByWithDirection(r => alphabetical ? r.CurrencyCode as object : r.ReferenceValue,
                    ApplicationSettings.SortDirectionRates == SortDirection.Ascending).ToList();
        }

        private List<SortButtonItem> LoadSortButtons(string currencyId)
        {
            if (!ApplicationSettings.MainCurrencies.Contains(currencyId)) return null;


            return new[]
            {
                new SortButtonItem
                {
                    Text = StringUtils.TextResolver.Currency,
                    SortAscending = SortDirectionHelper.GetSortAscending(ApplicationSettings.SortOrderRates,ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical),
                    RightAligned = false,
                    OnClick = () =>
                    {
                        ApplicationSettings.SortDirectionRates =SortDirectionHelper.GetNewSortDirection(ApplicationSettings.SortOrderRates,ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical);
                        ApplicationSettings.SortOrderRates = SortOrder.Alphabetical;

                        SortAndNotify();
                    }
                },
                new SortButtonItem
                {
                    Text = string.Format(StringUtils.TextResolver.AsCurrency, currencyId.Code()),
                    SortAscending = SortDirectionHelper.GetSortAscending(ApplicationSettings.SortOrderRates,ApplicationSettings.SortDirectionRates, SortOrder.ByValue),
                    RightAligned = true,
                    OnClick = () =>
                    {
                        ApplicationSettings.SortDirectionRates =SortDirectionHelper.GetNewSortDirection(ApplicationSettings.SortOrderRates,ApplicationSettings.SortDirectionRates, SortOrder.ByValue);
                        ApplicationSettings.SortOrderRates = SortOrder.ByValue;

                        SortAndNotify();
                    }
                }
            }.ToList();
        }

        private void SortAndNotify()
        {
            foreach (var key in _items.ToList().Select(pair => pair.Key))
            {
                _items[key] = _items[key] != null ? ApplySort(_items[key]) : null;
            }
            _sortButtons = new Dictionary<string, List<SortButtonItem>>();
            Messaging.UiUpdate.RatesOverview.Send();
        }
    }
}