using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;
using ZXing.OneD.RSS.Expanded.Decoders;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CoinInfoActivity : AppCompatActivity
    {
        public const string ExtraCurrency = "currency";

        private Currency _currency;

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

            header.Data = ViewData.CoinInfo.HeaderData(_currency);

            Messaging.UiUpdate.ReferenceTable.Subscribe(this, () => RunOnUiThread(SetReferenceTable));
            Messaging.UiUpdate.CoinInfo.Subscribe(this, () => RunOnUiThread(SetCoinInfo));

            Task.Run(() => LoadData(true));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        private void LoadData(bool requestData = false)
        {
            var info = ViewData.CoinInfo.CoinInfo(_currency);
            if (requestData && info == null && ViewData.CoinInfo.CoinInfoFetchable(_currency) && ConnectivityStatus.IsConnected)
            {
                Messaging.Request.CoinInfo.Send(_currency);
            }

            SetCoinInfo(info);

        }

        private void SetCoinInfo() => SetCoinInfo(null);

        private void SetCoinInfo(CoinInfoItem data)
        {
            data = data ?? ViewData.CoinInfo.CoinInfo(_currency);
            if (data == null) return;

            Func<bool, ViewStates> show = b => b ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.text_currency_code).Text = _currency.Code;
            FindViewById<TextView>(Resource.Id.text_currency_name).Text = _currency.Name;



            FindViewById<TextView>(Resource.Id.text_blockexplorer).Text = data.Explorer;
            FindViewById(Resource.Id.view_blockexplorer).Visibility = show(data.HasExplorer);



            FindViewById<TextView>(Resource.Id.text_coin_type).Text = data.Type;
            FindViewById(Resource.Id.view_coin_type).Visibility = show(data.HasType);

            FindViewById<TextView>(Resource.Id.text_algorithm).Text = data.Algorithm;
            FindViewById(Resource.Id.view_algorithm).Visibility = show(data.HasAlgorithm);

            FindViewById<TextView>(Resource.Id.text_difficulty).Text = data.Difficulty;
            FindViewById(Resource.Id.view_difficulty).Visibility = show(data.HasDifficulty);

            FindViewById<TextView>(Resource.Id.text_hashrate).Text = data.Hashrate;
            FindViewById(Resource.Id.view_hashrate).Visibility = show(data.HasHashrate);



            FindViewById<TextView>(Resource.Id.text_blockheight).Text = data.Blockheight;
            FindViewById(Resource.Id.view_blockheight).Visibility = show(data.HasBlockheight);

            FindViewById<TextView>(Resource.Id.text_blockreward).Text = data.Blockreward;
            FindViewById(Resource.Id.view_blockreward).Visibility = show(data.HasBlockreward);

            FindViewById<TextView>(Resource.Id.text_blocktime).Text = data.Blocktime;
            FindViewById(Resource.Id.view_blocktime).Visibility = show(data.HasBlocktime);



            FindViewById<TextView>(Resource.Id.text_supply).Text = data.Supply;
            FindViewById(Resource.Id.view_supply).Visibility = show(data.HasSupply);

            FindViewById<TextView>(Resource.Id.text_maxsupply).Text = data.MaxSupply;
            FindViewById(Resource.Id.view_maxsupply).Visibility = show(data.HasMaxSupply);

            FindViewById<TextView>(Resource.Id.view_marketcap).Text = data.Difficulty;
            FindViewById(Resource.Id.view_marketcap).Visibility = show(data.HasDifficulty);

        }

        private void SetReferenceTable()
        {
            var items = ViewData.CoinInfo.Items(_currency);
        }
    }
}