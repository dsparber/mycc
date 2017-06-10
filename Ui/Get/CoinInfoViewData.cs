using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
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
    public class CoinInfoViewData
    {
        public static HeaderDataItem HeaderData(Currency currency)
        {
            return new HeaderDataItem(currency.Name, new Money(MyccUtil.Rates.GetRate(new RateDescriptor(currency.Id, CurrencyConstants.Btc.Id))?.Rate ?? 0, CurrencyConstants.Btc).ToString8Digits());
        }

        public CoinInfoItem CoinInfo(Currency currency)
        {
            var info = CoinInfoStorage.Instance.Get(currency);
            if (info == null) return null;

            return new CoinInfoItem(info, Explorer(currency), currency);
        }

        public bool CoinInfoFetchable(Currency currency) => CoinInfoStorage.Instance.GetExplorer(currency).Any();

        private string Explorer(Currency currency) => CoinInfoFetchable(currency) ?
            string.Join(", ", CoinInfoStorage.Instance.GetExplorer(currency).Select(e => e.Name)) : null;

        public List<ReferenceValueItem> Items(Currency currency)
        {
            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { currency.Id })
                .Select(c => new ReferenceValueItem(1, MyccUtil.Rates.GetRate(new RateDescriptor(currency.Id, c))?.Rate, c))
                .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirection == SortDirection.Ascending)
                .ToList();
        }

        public static List<ICoinInfoRepository> ExplorerList(Currency currency) => CoinInfoStorage.Instance.GetExplorer(currency).ToList();

        public static DateTime LastUpdate(Currency currency)
        {
            var ratesTime = MyccUtil.Rates.LastUpdateFor(currency.Id);
            var infoTime = CoinInfoStorage.Instance.Get(currency)?.LastUpdate ?? DateTime.Now;

            return ratesTime < infoTime ? ratesTime : infoTime;
        }



        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Amount,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
               RightAligned = true,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Currency,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                RightAligned = false,
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
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private SortDirection SortDirection
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }
    }
}