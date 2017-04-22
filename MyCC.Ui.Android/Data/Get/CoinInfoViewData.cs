using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data.Get
{
    public class CoinInfoViewData
    {
        private readonly Context _context;

        public CoinInfoViewData(Context context)
        {
            _context = context;
        }

        public HeaderDataItem HeaderData(Currency currency)
        {
            return new HeaderDataItem(currency.Name, new Money(ExchangeRateHelper.GetRate(currency, Currency.Btc)?.Rate ?? 0, Currency.Btc).ToString8Digits());
        }

        public CoinInfoItem CoinInfo(Currency currency)
        {
            var info = CoinInfoStorage.Instance.Get(currency);
            if (info == null) return null;

            return new CoinInfoItem(info, Explorer(currency), _context, currency);
        }

        public bool CoinInfoFetchable(Currency currency) => CoinInfoStorage.Instance.GetExplorer(currency).Any();

        private string Explorer(Currency currency) => CoinInfoFetchable(currency) ?
            string.Join(", ", CoinInfoStorage.Instance.GetExplorer(currency).Select(e => e.Name)) : null;

        public List<ReferenceValueItem> Items(Currency currency)
        {
            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { currency })
                .Select(c => new ReferenceValueItem(1, ExchangeRateHelper.GetRate(currency, c)))
                .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Value, SortDirection == SortDirection.Ascending)
                .ToList();
        }

        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Amount),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Currency),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            }
        };

        private void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            Messaging.UiUpdate.ReferenceTables.Send();
        }



        private SortOrder SortOrder
        {
            get { return ApplicationSettings.SortOrderReferenceValues; }
            set { ApplicationSettings.SortOrderReferenceValues = value; }
        }

        private SortDirection SortDirection
        {
            get { return ApplicationSettings.SortDirectionReferenceValues; }
            set { ApplicationSettings.SortDirectionReferenceValues = value; }
        }
    }
}