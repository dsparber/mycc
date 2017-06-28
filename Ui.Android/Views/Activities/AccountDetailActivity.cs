using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Views.Fragments;
using Android.Support.V4.Widget;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Get;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class AccountDetailActivity : MyccActivity
    {
        public const string ExtraAccountId = "account";

        private int _accountId;

        private SortButtonFragment _sortAmount, _sortCurrency;
        private SwipeRefreshLayout _swipeToRefresh;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;

        private static readonly IAccountDetailViewData Data = UiUtils.Get.AccountDetail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_account_detail);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _accountId = Intent?.GetIntExtra(ExtraAccountId, -1) ?? -1;


            SupportActionBar.Title = Data.CurrencyId(_accountId).Code();
            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            _sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            Messaging.Modified.Balances.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Update.Balances.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Update.Rates.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Sort.Accounts.Subscribe(this, () => RunOnUiThread(SetData));

            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => UiUtils.Update.FetchBalanceAndRatesFor(_accountId);

            SetData();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.ShowQrCode)))
            {
                var intent = new Intent(this, typeof(ShowQrCodeActivity));
                intent.PutExtra(ShowQrCodeActivity.ExtraSourceId, Data.RepositoryId(_accountId));
                StartActivity(intent);
            }
            else if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Info)))
            {
                var intent = new Intent(this, typeof(CoinInfoActivity));
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, Data.CurrencyId(_accountId));
                StartActivity(intent);
            }
            else if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Edit)))
            {
                if (Data.IsLocal(_accountId))
                {
                    var intent = new Intent(this, typeof(EditAccountActivity));
                    intent.PutExtra(EditAccountActivity.ExtraAccountId, _accountId);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(EditSourceActivity));
                    intent.PutExtra(EditSourceActivity.ExtraRepositoryId, Data.RepositoryId(_accountId));
                    StartActivity(intent);
                }
            }
            else
            {
                Finish();
            }
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Info))
           .SetIcon(Resource.Drawable.ic_action_info).SetShowAsAction(ShowAsAction.Always);

            if (Data.ShowQrCodePossible(_accountId))
            {
                menu.Add(0, 0, 0, Resources.GetString(Resource.String.ShowQrCode))
                .SetIcon(Resource.Drawable.ic_action_qr).SetShowAsAction(ShowAsAction.Always);
            }

            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Edit))
           .SetIcon(Resource.Drawable.ic_edit).SetShowAsAction(ShowAsAction.Always);


            return true;
        }


        private void SetData()
        {
            if (AccountStorage.Instance.AllElements.FirstOrDefault(a => a.Id == _accountId) == null)
            {
                Finish();
                return;
            }

            _header.Data = Data.HeaderData(_accountId);
            _footerFragment.LastUpdate = Data.LastUpdate(_accountId);

            ViewStates Show(bool b) => b ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.text_name).Text = Data.AccountName(_accountId);
            FindViewById<TextView>(Resource.Id.text_type).Text = Data.AccountType(_accountId);

            FindViewById<TextView>(Resource.Id.text_source).Text = Data.AccountSource(_accountId);
            var addressText = FindViewById<TextView>(Resource.Id.text_address);
            addressText.Text = Data.AccountAddressString(_accountId);

            var explorerButton = FindViewById<Button>(Resource.Id.button_open_in_blockexplorer);
            explorerButton.Visibility = Data.BlockExplorerCallAllowed(_accountId) ? ViewStates.Visible : ViewStates.Gone;

            explorerButton.Click += (sender, args) =>
            {
                if (ConnectivityStatus.IsConnected)
                {
                    var intent = new Intent(this, typeof(WebviewActivity));
                    intent.PutExtra(WebviewActivity.ExtraUrl, Data.AddressClickUrl(_accountId));
                    StartActivity(intent);
                }
                else
                {
                    this.ShowInfoDialog(Resource.String.Error, Resource.String.NoInternetAccess);
                }
            };


            FindViewById(Resource.Id.label_source).Visibility = Show(Data.ShowAccountSource(_accountId));
            FindViewById(Resource.Id.text_source).Visibility = Show(Data.ShowAccountSource(_accountId));
            FindViewById(Resource.Id.label_address).Visibility = Show(Data.ShowAccountAddress(_accountId));
            addressText.Visibility = Show(Data.ShowAccountAddress(_accountId));

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = Data.ReferenceTableHeader(_accountId);


            var items = Data.GetReferenceItems(_accountId);
            var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortAmount.Data = Data.SortButtons[0];
            _sortAmount.First = true;
            _sortCurrency.Data = Data.SortButtons[1];
            _sortCurrency.Last = true;

            view.RemoveAllViews();

            foreach (var i in items)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_reference_full, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.FormattedAmount;
                v.FindViewById<TextView>(Resource.Id.text_currency).Text = i.CurrencyCode;
                v.FindViewById<TextView>(Resource.Id.text_rate).Text = i.FormattedRate;
                view.AddView(v);
            }

            _swipeToRefresh.Refreshing = false;
        }
    }
}