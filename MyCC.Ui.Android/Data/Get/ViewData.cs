﻿using Android.Content;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data.Get
{
    public class ViewData
    {
        public static RatesViewData Rates => _instance._ratesViewData;
        public static AssetsViewData Assets => _instance._assetsViewData;
        public static CoinInfoViewData CoinInfo => _instance._coinInfoViewData;
        public static AccountDetailViewData AccountDetailDetail => _instance._accountDetailViewData;
        public static AccountsGroupViewData AccountGroup => _instance._accountsGroupViewData;

        private readonly RatesViewData _ratesViewData;
        private readonly AssetsViewData _assetsViewData;
        private readonly CoinInfoViewData _coinInfoViewData;
        private readonly AccountDetailViewData _accountDetailViewData;
        private readonly AccountsGroupViewData _accountsGroupViewData;

        private ViewData(Context context)
        {
            _ratesViewData = new RatesViewData(context);
            _assetsViewData = new AssetsViewData(context);
            _coinInfoViewData = new CoinInfoViewData(context);
            _accountDetailViewData = new AccountDetailViewData(context);
            _accountsGroupViewData = new AccountsGroupViewData(context);

            Messaging.Update.Rates.Subscribe(this, () =>
            {
                _ratesViewData.UpdateRateItems();
                Messaging.UiUpdate.ViewsWithRate.Send();
            });
            Messaging.Update.Assets.Subscribe(this, () =>
            {
                _assetsViewData.UpdateRateItems();
                Messaging.UiUpdate.Accounts.Send();
            });


            Messaging.Request.AllRates.Subscribe(this, TaskHelper.UpdateAllRates);
            Messaging.Request.AllAssetsAndRates.Subscribe(this, TaskHelper.UpdateAllAssetsAndRates);

            Messaging.Request.DataForNewAccount.Subscribe(this, TaskHelper.UpdateDataForNewAccount);

            Messaging.Request.SingleAccount.Subscribe(this, TaskHelper.UpdateBalanceAndRatesForAccount);
            Messaging.Request.AccountsByCurrency.Subscribe(this, TaskHelper.UpdateBalancesAndRatesForCurrency);

            Messaging.Request.InfoForCurrency.Subscribe(this, TaskHelper.FetchCoinInfo);
            Messaging.Request.RateAndInfo.Subscribe(this, TaskHelper.FetchCoinInfoAndRates);
        }


        private static ViewData _instance;

        public static void Init(Context context)
        {
            _instance = _instance ?? new ViewData(context);
        }

    }
}