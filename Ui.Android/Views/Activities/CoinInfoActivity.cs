using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;
using MyCC.Core.Account.Models.Base;
using Android.Support.V4.Widget;
using MyCC.Core.Account.Storage;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using MyCC.Ui.ViewData;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CoinInfoActivity : MyccActivity
    {
        public const string ExtraCurrency = "currency";
        public const string ExtraShowAccountsButton = "showAccountsButton";

        private Currency _currency;

        private SortButtonFragment _sortAmount, _sortCurrency;
        private SwipeRefreshLayout _swipeToRefresh;
        private FooterFragment _footerFragment;
        private HeaderFragment _header;
        private bool _showAccountsButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_coin_info);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _showAccountsButton = Intent?.GetBooleanExtra(ExtraShowAccountsButton, false) ?? false;
            var currencyString = Intent?.GetStringExtra(ExtraCurrency);
            if (!string.IsNullOrWhiteSpace(currencyString))
            {
                _currency = JsonConvert.DeserializeObject<Currency>(currencyString);
            }

            SupportActionBar.Title = _currency.Code;
            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(Resource.String.IsEqualTo), new Money(1, _currency));

            _sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            Messaging.UiUpdate.CoinInfo.Subscribe(this, () => RunOnUiThread(SetCoinInfo));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => Messaging.Request.RateAndInfo.Send(_currency);

            LoadData();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (!_showAccountsButton || !AccountStorage.AccountsWithCurrency(_currency).Any()) return true;


            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Assets))
                .SetIcon(Resource.Drawable.ic_action_table).SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Assets)))
            {
                var accounts = AccountStorage.AccountsWithCurrency(_currency);

                if (accounts.Count == 1)
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, accounts.First().Id);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(AccountGroupActivity));
                    intent.PutExtra(AccountGroupActivity.ExtraCurrencyId, _currency.Id);
                    StartActivity(intent);
                }
            }
            else
            {
                Finish();
            }
            return true;
        }

        private void LoadData()
        {
            var info = ViewData.ViewData.CoinInfo.CoinInfo(_currency);
            if (info == null && ViewData.ViewData.CoinInfo.CoinInfoFetchable(_currency) && ConnectivityStatus.IsConnected)
            {
                Messaging.Request.InfoForCurrency.Send(_currency);
            }

            SetCoinInfo(info);
            SetReferenceTable();

        }

        private void SetCoinInfo() => SetCoinInfo(null);

        private void SetCoinInfo(CoinInfoItem data)
        {
            data = data ?? ViewData.ViewData.CoinInfo.CoinInfo(_currency);

            _header.Data = CoinInfoViewData.HeaderData(_currency);
            _footerFragment.LastUpdate = CoinInfoViewData.LastUpdate(_currency);

            Func<bool?, ViewStates> show = b => b != null && b.Value ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.text_currency_code).Text = _currency.Code;
            FindViewById<TextView>(Resource.Id.text_currency_name).Text = _currency.Name;



            FindViewById<TextView>(Resource.Id.text_blockexplorer).Text = data?.Explorer;
            FindViewById(Resource.Id.view_blockexplorer).Visibility = show(data?.HasExplorer);



            FindViewById<TextView>(Resource.Id.text_coin_type).Text = data?.Type;
            FindViewById(Resource.Id.view_coin_type).Visibility = show(data?.HasType);

            FindViewById<TextView>(Resource.Id.text_algorithm).Text = data?.Algorithm;
            FindViewById(Resource.Id.view_algorithm).Visibility = show(data?.HasAlgorithm);

            FindViewById<TextView>(Resource.Id.text_difficulty).Text = data?.Difficulty;
            FindViewById(Resource.Id.view_difficulty).Visibility = show(data?.HasDifficulty);

            FindViewById<TextView>(Resource.Id.text_hashrate).Text = data?.Hashrate;
            FindViewById(Resource.Id.view_hashrate).Visibility = show(data?.HasHashrate);



            FindViewById<TextView>(Resource.Id.text_blockheight).Text = data?.Blockheight;
            FindViewById(Resource.Id.view_blockheight).Visibility = show(data?.HasBlockheight);

            FindViewById<TextView>(Resource.Id.text_blockreward).Text = data?.Blockreward;
            FindViewById(Resource.Id.view_blockreward).Visibility = show(data?.HasBlockreward);

            FindViewById<TextView>(Resource.Id.text_blocktime).Text = data?.Blocktime;
            FindViewById(Resource.Id.view_blocktime).Visibility = show(data?.HasBlocktime);



            FindViewById<TextView>(Resource.Id.text_supply).Text = data?.Supply;
            FindViewById(Resource.Id.view_supply).Visibility = show(data?.HasSupply);

            FindViewById<TextView>(Resource.Id.text_maxsupply).Text = data?.MaxSupply;
            FindViewById(Resource.Id.view_maxsupply).Visibility = show(data?.HasMaxSupply);

            FindViewById<TextView>(Resource.Id.text_marketcap).Text = data?.Difficulty;
            FindViewById(Resource.Id.view_marketcap).Visibility = show(data?.HasDifficulty);

            SetReferenceTable();

            _swipeToRefresh.Refreshing = false;
        }

        private void SetReferenceTable()
        {
            var items = ViewData.ViewData.CoinInfo.Items(_currency);
            var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortAmount.Data = ViewData.ViewData.CoinInfo.SortButtons[0];
            _sortAmount.First = true;
            _sortCurrency.Data = ViewData.ViewData.CoinInfo.SortButtons[1];
            _sortCurrency.Last = true;

            view.RemoveAllViews();

            foreach (var i in items)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_reference_small, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.FormattedAmount;
                v.FindViewById<TextView>(Resource.Id.text_currency).Text = i.CurrencyCode;
                view.AddView(v);
            }
        }
    }
}