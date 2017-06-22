using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using MyCC.Core.Account.Models.Base;
using Android.Support.V4.Widget;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CoinInfoActivity : MyccActivity
    {
        public const string ExtraCurrency = "currency";
        public const string ExtraShowAccountsButton = "showAccountsButton";

        private string _currencyId;

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

            _showAccountsButton = Intent.GetBooleanExtra(ExtraShowAccountsButton, false);
            _currencyId = Intent.GetStringExtra(ExtraCurrency);

            SupportActionBar.Title = _currencyId.Code();
            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(Resource.String.IsEqualTo), new Money(1, _currencyId.Find()));

            _sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            Messaging.Update.CoinInfo.Subscribe(this, () => RunOnUiThread(SetCoinInfo));
            Messaging.Update.Rates.Subscribe(this, () => RunOnUiThread(SetCoinInfo));
            Messaging.Sort.ReferenceTables.Subscribe(this, () => RunOnUiThread(SetReferenceTable));

            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => UiUtils.Update.FetchCoinInfoAndRatesFor(_currencyId);

            var explorerButtosView = FindViewById<LinearLayout>(Resource.Id.view_open_in_blockexplorer);
            var explorerList = UiUtils.Get.CoinInfo.Explorer(_currencyId).ToList();
            foreach (var explorer in explorerList)
            {
                var button = new Button(this)
                {
                    Text = explorerList.Count == 1 ? Resources.GetString(Resource.String.OpenInBlockExplorer)
                            : $"{Resources.GetString(Resource.String.OpenInBlockExplorer)} ({explorer.Name})"
                };
                button.SetTextColor(Color.White);
                button.Click += (sender, args) =>
                {
                    if (ConnectivityStatus.IsConnected)
                    {
                        var intent = new Intent(this, typeof(WebviewActivity));
                        intent.PutExtra(WebviewActivity.ExtraUrl, explorer.WebLink);
                        StartActivity(intent);
                    }
                    else
                    {
                        this.ShowInfoDialog(Resource.String.Error, Resource.String.NoInternetAccess);
                    }
                };
                explorerButtosView.AddView(button);
            }
            explorerButtosView.Visibility = explorerList.Count > 0 ? ViewStates.Visible : ViewStates.Gone;

            LoadData();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (!_showAccountsButton || !AccountStorage.AccountsWithCurrency(_currencyId).Any()) return true;


            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Assets))
                .SetIcon(Resource.Drawable.ic_action_table).SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Assets)))
            {
                var accounts = AccountStorage.AccountsWithCurrency(_currencyId);

                if (accounts.Count == 1)
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, accounts.First().Id);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(AccountGroupActivity));
                    intent.PutExtra(AccountGroupActivity.ExtraCurrencyId, _currencyId);
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
            var info = UiUtils.Get.CoinInfo.GetInfos(_currencyId);
            if (info == null && UiUtils.Get.CoinInfo.InfosAvailable(_currencyId) && ConnectivityStatus.IsConnected)
            {
                UiUtils.Update.FetchCoinInfoFor(_currencyId);
            }

            SetCoinInfo(info);
            SetReferenceTable();

        }

        private void SetCoinInfo() => SetCoinInfo(null);

        private void SetCoinInfo(CoinInfoItem data)
        {
            data = data ?? UiUtils.Get.CoinInfo.GetInfos(_currencyId);

            _header.Data = UiUtils.Get.CoinInfo.GetHeaderData(_currencyId);
            _footerFragment.LastUpdate = UiUtils.Get.CoinInfo.LastUpdate(_currencyId);

            ViewStates Show(bool? b) => b != null && b.Value ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.text_currency_code).Text = _currencyId.Code();
            FindViewById<TextView>(Resource.Id.text_currency_name).Text = _currencyId.FindName();

            FindViewById<TextView>(Resource.Id.text_blockexplorer).Text = data?.Explorer;
            FindViewById(Resource.Id.view_blockexplorer).Visibility = Show(data?.HasExplorer);


            FindViewById(Resource.Id.view_group_coin_stats).Visibility = Show(data != null && (data.HasType || data.HasAlgorithm || data.HasDifficulty || data.HasHashrate));

            FindViewById<TextView>(Resource.Id.text_coin_type).Text = data?.Type;
            FindViewById(Resource.Id.view_coin_type).Visibility = Show(data?.HasType);

            FindViewById<TextView>(Resource.Id.text_algorithm).Text = data?.Algorithm;
            FindViewById(Resource.Id.view_algorithm).Visibility = Show(data?.HasAlgorithm);

            FindViewById<TextView>(Resource.Id.text_difficulty).Text = data?.Difficulty;
            FindViewById(Resource.Id.view_difficulty).Visibility = Show(data?.HasDifficulty);

            FindViewById<TextView>(Resource.Id.text_hashrate).Text = data?.Hashrate;
            FindViewById(Resource.Id.view_hashrate).Visibility = Show(data?.HasHashrate);


            FindViewById(Resource.Id.view_group_block_stats).Visibility = Show(data != null && (data.HasBlockheight || data.HasBlockreward || data.HasBlocktime));

            FindViewById<TextView>(Resource.Id.text_blockheight).Text = data?.Blockheight;
            FindViewById(Resource.Id.view_blockheight).Visibility = Show(data?.HasBlockheight);

            FindViewById<TextView>(Resource.Id.text_blockreward).Text = data?.Blockreward;
            FindViewById(Resource.Id.view_blockreward).Visibility = Show(data?.HasBlockreward);

            FindViewById<TextView>(Resource.Id.text_blocktime).Text = data?.Blocktime;
            FindViewById(Resource.Id.view_blocktime).Visibility = Show(data?.HasBlocktime);



            FindViewById(Resource.Id.view_group_supply).Visibility = Show(data != null && (data.HasSupply || data.HasMaxSupply || data.HasMarketCap));

            FindViewById<TextView>(Resource.Id.text_supply).Text = data?.Supply;
            FindViewById(Resource.Id.view_supply).Visibility = Show(data?.HasSupply);

            FindViewById<TextView>(Resource.Id.text_maxsupply).Text = data?.MaxSupply;
            FindViewById(Resource.Id.view_maxsupply).Visibility = Show(data?.HasMaxSupply);

            FindViewById<TextView>(Resource.Id.text_marketcap).Text = data?.MarketCap;
            FindViewById(Resource.Id.view_marketcap).Visibility = Show(data?.HasMarketCap);

            SetReferenceTable();

            _swipeToRefresh.Refreshing = false;
        }

        private void SetReferenceTable()
        {
            var items = UiUtils.Get.CoinInfo.ReferenceValues(_currencyId);
            var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortAmount.Data = UiUtils.Get.CoinInfo.SortButtons[0];
            _sortAmount.First = true;
            _sortCurrency.Data = UiUtils.Get.CoinInfo.SortButtons[1];
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