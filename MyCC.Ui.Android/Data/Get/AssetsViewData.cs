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

namespace MyCC.Ui.Android.Data.Get
{
    public class AssetsViewData
    {
        public Dictionary<Currency, List<AssetItem>> Items { get; private set; }
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }
        public Dictionary<Currency, List<SortButtonItem>> SortButtons { get; private set; }

        private readonly Context _context;
        public bool IsDataAvailable => Items != null && Items.Count > 0 && Items.Min(i => i.Value.Count) > 0;


        public AssetsViewData(Context context)
        {
            _context = context;
        }

        public void UpdateRateItems()
        {
            Items = LoadRateItems();
            Headers = LoadRateHeaders();
            SortButtons = LoadSortButtons();

            Messaging.UiUpdate.AssetsTable.Send();
        }

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

        private static Dictionary<Currency, CoinHeaderData> LoadRateHeaders() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            var amount = AccountStorage.EnabledAccounts.Sum(a => a.Money.Amount * ExchangeRateHelper.GetRate(a.Money.Currency, c)?.Rate ?? 0);
            var referenceMoney = new Money(amount, c);

            var additionalRefs = ApplicationSettings.MainCurrencies
                .Except(new[] { c })
                .Select(x => new Money(amount * ExchangeRateHelper.GetRate(c, x)?.Rate ?? 0, x))
                .ToList();

            return new CoinHeaderData(referenceMoney, additionalRefs);
        });

        private static Dictionary<Currency, List<AssetItem>> LoadRateItems() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
        {
            Func<Money, Money> getReference = m => new Money(m.Amount * (ExchangeRateHelper.GetRate(m.Currency, c)?.Rate ?? 0), c);

            var items = AccountStorage.AccountsGroupedByCurrency.Select(group =>
            {
                var money = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);
                return new AssetItem(money, getReference(money));
            }).ToList();

            return ApplySort(items);
        });

        private static List<AssetItem> ApplySort(IEnumerable<AssetItem> items)
        {
            var alphabetical = SortOrder == SortOrder.Alphabetical;
            var byValue = SortOrder == SortOrder.ByValue;

            return items.OrderByWithDirection(r => alphabetical ? r.CurrencyCode as object : byValue ? r.ReferenceValue.Amount : r.Value.Amount,
                    SortDirection == SortDirection.Ascending).ToList();
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
            Items = ApplicationSettings.MainCurrencies.ToDictionary(c => c, c => ApplySort(Items[c]));
            SortButtons = LoadSortButtons();
            Messaging.UiUpdate.AssetsTable.Send();
        }
    }
}