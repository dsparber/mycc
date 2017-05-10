using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;

namespace MyCC.Ui.ViewData
{
    public class RatesViewData
    {
        public static Dictionary<Currency, List<RateItem>> Items => LoadRateItems();
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }
        public Dictionary<Currency, DateTime> LastUpdate { get; private set; }


        public bool IsDataAvailable => Items != null && Items.Count > 0 && Items.Min(i => i.Value.Count) > 0;

        public void UpdateRateItems()
        {
            Headers = LoadRateHeaders();
            SortButtons = LoadSortButtons();
            LastUpdate = GetLastUpdate();

            Messaging.UiUpdate.RatesOverview.Send();
        }

        private static Dictionary<Currency, DateTime> GetLastUpdate() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            return CurrencySettingsData.EnabledCurrencies
                .Select(e => new ExchangeRate(c, e))
                .SelectMany(ExchangeRateHelper.GetNeededRates)
                .Distinct()
                .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now)
                .DefaultIfEmpty(DateTime.Now)
                .Min();
        });


        private static Dictionary<Currency, CoinHeaderData> LoadRateHeaders() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            var referenceMoney = new Money(ExchangeRateHelper.GetRate(Currency.Btc, c)?.Rate ?? 0, c);

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { c })
                .Select(x => new Money(ExchangeRateHelper.GetRate(Currency.Btc, x)?.Rate ?? 0, x))
                .ToList();

            return new CoinHeaderData(referenceMoney, additionalRefs);
        });

        private static Dictionary<Currency, List<RateItem>> LoadRateItems() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            Func<Currency, Money> getReference = currency => new Money(ExchangeRateHelper.GetRate(currency, c)?.Rate ?? 0, currency);

            var items = CurrencySettingsData.EnabledCurrencies.Except(new[] { c }).Select(x => new RateItem(x, getReference(x)));

            return ApplySort(items);
        });

        private static List<RateItem> ApplySort(IEnumerable<RateItem> items)
        {
            var alphabetical = ApplicationSettings.SortOrderRates == SortOrder.Alphabetical;

            return items.OrderByWithDirection(r => alphabetical ? r.CurrencyCode as object : r.ReferenceValue.Amount,
                    ApplicationSettings.SortDirectionRates == SortDirection.Ascending).ToList();
        }

        private Dictionary<Currency, List<SortButtonItem>> LoadSortButtons() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Currency,
                SortDirection = SortDirectionHelper.GetSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () =>
                {
                    ApplicationSettings.SortDirectionRates = SortDirectionHelper.GetNewSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical);
                    ApplicationSettings.SortOrderRates = SortOrder.Alphabetical;

                    SortAndNotify();
                }
            },
            new SortButtonItem
            {
                Text = string.Format(StringHelper.TextResolver.AsCurrency, c.Code),
                SortDirection = SortDirectionHelper.GetSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () =>
                {
                    ApplicationSettings.SortDirectionRates = SortDirectionHelper.GetNewSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.ByValue);
                    ApplicationSettings.SortOrderRates = SortOrder.ByValue;

                    SortAndNotify();
                }
            }
        });

        private void SortAndNotify()
        {
            SortButtons = LoadSortButtons();
            Messaging.UiUpdate.RatesOverview.Send();
        }
    }
}