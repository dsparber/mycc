using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Fragments;
using MyCC.Core.Account.Models.Base;
using Android.Support.V4.Widget;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class AccountDetailActivity : MyccActivity
    {
        public const string ExtraAccountId = "account";

        private FunctionalAccount _account;

        private SortButtonFragment _sortAmount, _sortCurrency;
        private SwipeRefreshLayout _swipeToRefresh;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_account_detail);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var accountId = Intent?.GetIntExtra(ExtraAccountId, -1) ?? -1;
            if (accountId != -1)
            {
                _account = AccountStorage.Instance.AllElements.Find(a => a.Id == accountId);
            }

            SupportActionBar.Title = _account.Money.Currency.Code;
            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            _sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            Messaging.UiUpdate.AccountDetail.Subscribe(this, () => RunOnUiThread(SetData));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => Messaging.Request.SingleAccount.Send(_account);

            SetData();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Info)))
            {
                var intent = new Intent(this, typeof(CoinInfoActivity));
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, JsonConvert.SerializeObject(_account.Money.Currency));
                StartActivity(intent);
            }
            else if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Edit)))
            {
                if (_account is LocalAccount)
                {
                    var intent = new Intent(this, typeof(EditAccountActivity));
                    intent.PutExtra(EditAccountActivity.ExtraAccountId, _account.Id);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(EditSourceActivity));
                    intent.PutExtra(EditSourceActivity.ExtraRepositoryId, AccountStorage.RepositoryOf(_account).Id);
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
            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Edit))
           .SetIcon(Resource.Drawable.ic_edit).SetShowAsAction(ShowAsAction.Always);


            return true;
        }


        private void SetData()
        {
            if (AccountStorage.Instance.AllElements.FirstOrDefault(a => a.Id == _account.Id) == null)
            {
                Finish();
                return;
            }

            _header.Data = AccountDetailViewData.HeaderData(_account);
            _footerFragment.LastUpdate = AccountDetailViewData.LastUpdate(_account);

            Func<bool, ViewStates> show = b => b ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.text_name).Text = ViewData.AccountDetail.AccountName(_account);
            FindViewById<TextView>(Resource.Id.text_type).Text = ViewData.AccountDetail.AccountType(_account);

            FindViewById<TextView>(Resource.Id.text_source).Text = ViewData.AccountDetail.AccountSource(_account);
            var addressText = FindViewById<TextView>(Resource.Id.text_address);
            addressText.Text = ViewData.AccountDetail.AccountAddressString(_account);

            if (ViewData.AccountDetail.AddressClickable(_account))
            {
                addressText.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.colorPrimary)));
                addressText.Clickable = true;
                addressText.PaintFlags = PaintFlags.UnderlineText;
                addressText.Click += (sender, args) =>
                {
                    var intent = new Intent(this, typeof(WebviewActivity));
                    intent.PutExtra(WebviewActivity.ExtraUrl, ViewData.AccountDetail.AddressClickUrl(_account));
                    StartActivity(intent);
                };
            }

            FindViewById(Resource.Id.label_source).Visibility = show(ViewData.AccountDetail.ShowAccountSource(_account));
            FindViewById(Resource.Id.text_source).Visibility = show(ViewData.AccountDetail.ShowAccountSource(_account));
            FindViewById(Resource.Id.label_address).Visibility = show(ViewData.AccountDetail.ShowAccountAddress(_account));
            addressText.Visibility = show(ViewData.AccountDetail.ShowAccountAddress(_account));

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(_account.Money.Amount == 1 ? Resource.String.IsEqualTo : Resource.String.AreEqualTo), _account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney));


            var items = AccountDetailViewData.Items(_account);
            var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortAmount.Data = ViewData.AccountDetail.SortButtons[0];
            _sortAmount.First = true;
            _sortCurrency.Data = ViewData.AccountDetail.SortButtons[1];
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