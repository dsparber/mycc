using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Get.Implementations
{
    internal class CoinInfoViewData : ICoinInfoViewData
    {
        public HeaderItem GetHeaderData(string currencyId)
        {
            return new HeaderItem(currencyId.FindName(), new Money(MyccUtil.Rates.GetRate(new RateDescriptor(currencyId, CurrencyConstants.Btc.Id))?.Rate ?? 0, CurrencyConstants.Btc).EightDigits());
        }

        public CoinInfoItem GetInfos(string currencyId)
        {
            var info = CoinInfoStorage.Instance.Get(currencyId);
            return info == null ? null : new CoinInfoItem(info, Explorer(currencyId).Select(item => item.Name), currencyId);
        }

        public bool InfosAvailable(string currencyId) => CoinInfoStorage.Instance.GetExplorer(currencyId).Any();

        public IEnumerable<ReferenceValueItem> ReferenceValues(string currencyId)
        {
            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { currencyId })
                .Select(c => new ReferenceValueItem(1, MyccUtil.Rates.GetRate(new RateDescriptor(currencyId, c))?.Rate, c))
                .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirection == SortDirection.Ascending)
                .ToList();
        }

        public IEnumerable<(string Name, string WebLink)> Explorer(string currencyId) => CoinInfoStorage.Instance.GetExplorer(currencyId).Select(explorer => (explorer.Name, explorer.WebUrl(currencyId))).ToList();

        public DateTime LastUpdate(string currencyId)
        {
            var ratesTime = MyccUtil.Rates.LastUpdateFor(currencyId);
            var infoTime = CoinInfoStorage.Instance.Get(currencyId)?.LastUpdate ?? DateTime.Now;

            return ratesTime < infoTime ? ratesTime : infoTime;
        }

        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = TextResolver.Instance.Amount,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.ByValue),
               RightAligned = true,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = TextResolver.Instance.Currency,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            }
        };

        private static void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            Messaging.Sort.ReferenceTables.Send();
        }


        private static SortOrder SortOrder
        {
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private static SortDirection SortDirection
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }
    }
}