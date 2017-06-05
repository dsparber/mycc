using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Get
{
    public class RatesViewData
    {
        public static Dictionary<Currency, List<RateItem>> Items => LoadRateItems();
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }
        public DateTime LastUpdate { get; private set; }


        public bool IsDataAvailable => Items != null && Items.Count > 0 && Items.Min(i => i.Value.Count) > 0;

        public void UpdateItems()
        {
            Headers = LoadRateHeaders();
            SortButtons = LoadSortButtons();
            LastUpdate = MyccUtil.Rates.LastUpdate();

            Messaging.UiUpdate.RatesOverview.Send();
        }


        private static Dictionary<Currency, CoinHeaderData> LoadRateHeaders() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyHelper.Find, c =>
        {

            var referenceMoney = new Money(MyccUtil.Rates.GetRate(new RateDescriptor(CurrencyConstants.Btc.Id, c))?.Rate ?? 0, c.Find());

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { c })
                .Select(x => new Money(MyccUtil.Rates.GetRate(new RateDescriptor(CurrencyConstants.Btc.Id, x))?.Rate ?? 0, x.Find()));

            return new CoinHeaderData(referenceMoney, additionalRefs);
        });

        private static Dictionary<Currency, List<RateItem>> LoadRateItems() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyHelper.Find, c =>
        {
            Money GetReference(Currency currency) => new Money(MyccUtil.Rates.GetRate(new RateDescriptor(currency.Id, c))?.Rate ?? 0, currency);

            var items = CurrencySettingsData.EnabledCurrencies.Except(new[] { c.ToCurrency() }).Select(x => new RateItem(x, GetReference(x)));

            return ApplySort(items);
        });

        private static List<RateItem> ApplySort(IEnumerable<RateItem> items)
        {
            var alphabetical = ApplicationSettings.SortOrderRates == SortOrder.Alphabetical;

            return items.OrderByWithDirection(r => alphabetical ? r.CurrencyCode as object : r.ReferenceValue.Amount,
                    ApplicationSettings.SortDirectionRates == SortDirection.Ascending).ToList();
        }

        private Dictionary<Currency, List<SortButtonItem>> LoadSortButtons() => ApplicationSettings.MainCurrencies.ToDictionary(CurrencyHelper.Find, c => new List<SortButtonItem>
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
                Text = string.Format(StringHelper.TextResolver.AsCurrency, c.ToCurrency().Code),
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