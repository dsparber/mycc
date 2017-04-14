using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Data
{
    public class AssetsGraphViewData
    {
        private Dictionary<Currency, AssetsGraphItem.Data[]> _items;
        public Dictionary<Currency, CoinHeaderData> Headers { get; private set; }

        public string JsDataString(Currency currency)
        {
            var data = JsonConvert.SerializeObject(_items[currency]);
            var accountStrings = JsonConvert.SerializeObject(new[] { _context.Resources.GetString(Resource.String.OneAccount), _context.Resources.GetString(Resource.String.Accounts) });
            var currenciesStrings = JsonConvert.SerializeObject(new[] { _context.Resources.GetString(Resource.String.OneCurrency), _context.Resources.GetString(Resource.String.Currencies) });
            var furtherString = _context.Resources.GetString(Resource.String.Further);
            var noDataString = _context.Resources.GetString(Resource.String.NoDataToDisplay);
            var roundMoney = ApplicationSettings.RoundMoney.ToString();
            var baseCurrency = currency.Code;
            var culture = CultureInfo.CurrentCulture.ToString();

            return $"showChart({data}, {accountStrings}, {currenciesStrings}, \"{furtherString}\", \"{noDataString}\", \"{baseCurrency}\", \"{roundMoney}\", \"{culture}\");";
        }

        private readonly Context _context;

        public AssetsGraphViewData(Context context)
        {
            _context = context;
        }

        public void UpdateItems()
        {
            _items = LoadItems();
            Headers = LoadRateHeaders();

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

        public bool IsReady => _items != null;

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

        private static Dictionary<Currency, AssetsGraphItem.Data[]> LoadItems() => ApplicationSettings.MainCurrencies.ToDictionary(c => c, c =>
             AccountStorage.AccountsGroupedByCurrency
                        .Select(e => new AssetsGraphItem.Data(e, c))
                        .Where(d => d.Value > 0)
                        .OrderByDescending(d => d.Value)
                        .ToArray());

        private static List<AssetItem> ApplySort(IEnumerable<AssetItem> items)
        {
            var alphabetical = SortOrder == SortOrder.Alphabetical;
            var byValue = SortOrder == SortOrder.ByValue;

            return items.OrderByWithDirection(r => alphabetical ? r.CurrencyCode as object : byValue ? r.ReferenceValue.Amount : r.Value.Amount,
                    SortDirection == SortDirection.Ascending).ToList();
        }
    }
}