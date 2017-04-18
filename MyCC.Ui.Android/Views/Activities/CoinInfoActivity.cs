using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;
using MyCC.Core.Account.Models.Base;
using Android.Support.V4.Widget;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CoinInfoActivity : AppCompatActivity
    {
        public const string ExtraCurrency = "currency";

        private Currency _currency;

        private SortButtonFragment _sortAmount, _sortCurrency;

        private SwipeRefreshLayout _swipeToRefresh;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_coin_info);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var currencyString = Intent?.GetStringExtra(ExtraCurrency);
            if (!string.IsNullOrWhiteSpace(currencyString))
            {
                _currency = JsonConvert.DeserializeObject<Currency>(currencyString);
            }

            SupportActionBar.Title = _currency.Code;
            var header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(Resource.String.IsEqualTo), new Money(1, _currency));

            header.Data = ViewData.CoinInfo.HeaderData(_currency);

            _sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            Messaging.UiUpdate.CoinInfo.Subscribe(this, () => RunOnUiThread(SetCoinInfo));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) =>
            {
                Messaging.Request.CoinInfo.Send(_currency);
                Messaging.Request.Rates.Send(_currency);
            };

            LoadData();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        private void LoadData()
        {
            var info = ViewData.CoinInfo.CoinInfo(_currency);
            if (info == null && ViewData.CoinInfo.CoinInfoFetchable(_currency) && ConnectivityStatus.IsConnected)
            {
                Messaging.Request.CoinInfo.Send(_currency);
            }

            SetCoinInfo(info);
            SetReferenceTable();

        }

        private void SetCoinInfo() => SetCoinInfo(null);

        private void SetCoinInfo(CoinInfoItem data)
        {
            data = data ?? ViewData.CoinInfo.CoinInfo(_currency);

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
            var items = ViewData.CoinInfo.Items(_currency);
            var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortAmount.Data = ViewData.CoinInfo.SortButtons[0];
            _sortCurrency.Data = ViewData.CoinInfo.SortButtons[1];

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