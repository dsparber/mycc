using System;
using System.Collections.Generic;
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

namespace MyCC.Ui.Android.Data
{
    public class RatesViewData
    {
        public Dictionary<Currency, List<RateItem>> Items { get; private set; }
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }

        private readonly Context _context;

        public RatesViewData(Context context)
        {
            _context = context;
        }

        public void UpdateRateItems()
        {
            Items = LoadRateItems();
            Headers = LoadRateHeaders();
            SortButtons = LoadSortButtons();

            Messaging.UiUpdate.RatesOverview.Send();
        }



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

            var items = ApplicationSettings.WatchedCurrencies
                 .Concat(ApplicationSettings.AllReferenceCurrencies)
                 .Concat(AccountStorage.UsedCurrencies)
                 .Distinct().Except(new[] { c })
                 .Select(x => new RateItem(x, getReference(x)));

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
                Text = _context.Resources.GetString(Resource.String.Currency),
                SortDirection = SortDirectionHelper.GetSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () =>
                {
                    ApplicationSettings.SortDirectionRates = SortDirectionHelper.GetNewSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.Alphabetical);
                    ApplicationSettings.SortOrderRates = SortOrder.Alphabetical;

                    SortAndNotify();
                }
            },
            new SortButtonItem
            {
                Text = string.Format(_context.Resources.GetString(Resource.String.AsCurrency), c.Code),
                SortDirection = SortDirectionHelper.GetSortDirection(ApplicationSettings.SortOrderRates, ApplicationSettings.SortDirectionRates, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
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
            Items = ApplicationSettings.MainCurrencies.ToDictionary(c => c, c => ApplySort(Items[c]));
            SortButtons = LoadSortButtons();
            Messaging.UiUpdate.RatesOverview.Send();
        }
    }
}