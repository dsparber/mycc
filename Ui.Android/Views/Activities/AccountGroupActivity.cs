using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Views.Fragments;
using Android.Support.V4.Widget;
using MyCC.Core.Settings;
using Android.Content;
using Android.Support.Design.Widget;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Ui.Messages;
using MyCC.Ui.ViewData;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class AccountGroupActivity : MyccActivity
    {
        public const string ExtraCurrencyId = "currencyId";

        private Currency _currency;

        private HeaderFragment _header;
        private SortButtonFragment _sortReferenceAmount, _sortReferenceCurrency,
        _sortAccountsName, _sortAccountsAmount, _sortDisabledName, _sortDisabledAmount;
        private SwipeRefreshLayout _swipeToRefresh;
        private FooterFragment _footerFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_account_group);

            var currencyId = Intent?.GetStringExtra(ExtraCurrencyId);
            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                _currency = currencyId.Find();
            }

            SupportActionBar.Title = $"\u2211 {_currency.Code}";

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);


            _sortReferenceAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortReferenceCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            _sortAccountsName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_name_sort);
            _sortAccountsAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_amount_sort);

            _sortDisabledName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_name_sort);
            _sortDisabledAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_amount_sort);

            Messaging.UiUpdate.AccountDetail.Subscribe(this, () => RunOnUiThread(SetData));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => Messaging.Request.AccountsByCurrency.Send(_currency);

            FindViewById<FloatingActionButton>(Resource.Id.button_add).Click += (sender, args) => StartActivity(new Intent(Application.Context, typeof(AddSourceActivity)));

            SetData();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Info))
           .SetIcon(Resource.Drawable.ic_action_info).SetShowAsAction(ShowAsAction.Always);


            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Info)))
            {
                var intent = new Intent(this, typeof(CoinInfoActivity));
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, JsonConvert.SerializeObject(_currency));
                StartActivity(intent);
            }
            else
            {
                Finish();
            }
            return true;
        }


        private void SetData()
        {
            if (!AccountsGroupViewData.DisabledAccountsItems(_currency).Any() && !AccountsGroupViewData.EnabledAccountsItems(_currency).Any())
            {
                Finish();
                return;
            }

            _header.Data = AccountsGroupViewData.HeaderData(_currency);
            _footerFragment.LastUpdate = AccountsGroupViewData.LastUpdate(_currency);

            var money = AccountsGroupViewData.GetEnabledSum(_currency);
            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(money.Amount == 1 ? Resource.String.IsEqualTo : Resource.String.AreEqualTo), money);


            var referenceItems = AccountsGroupViewData.ReferenceItems(_currency);
            var viewReference = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortReferenceAmount.Data = ViewData.ViewData.AccountGroup.SortButtonsReference[0];
            _sortAccountsAmount.First = true;
            _sortReferenceCurrency.Data = ViewData.ViewData.AccountGroup.SortButtonsReference[1];
            _sortReferenceCurrency.Last = true;

            viewReference.RemoveAllViews();

            foreach (var i in referenceItems)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_reference_full, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.FormattedAmount;
                v.FindViewById<TextView>(Resource.Id.text_currency).Text = i.CurrencyCode;
                v.FindViewById<TextView>(Resource.Id.text_rate).Text = i.FormattedRate;
                viewReference.AddView(v);
            }

            var enabledItems = AccountsGroupViewData.EnabledAccountsItems(_currency).ToList();
            var viewEnabled = FindViewById<LinearLayout>(Resource.Id.view_enabled_accounts);

            _sortAccountsName.Data = ViewData.ViewData.AccountGroup.SortButtonsAccounts[0];
            _sortAccountsName.First = true;
            _sortAccountsAmount.Data = ViewData.ViewData.AccountGroup.SortButtonsAccounts[1];

            viewEnabled.RemoveAllViews();

            foreach (var i in enabledItems)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_account, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = i.Name;

                v.Click += (sender, e) =>
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, i.Id);
                    StartActivity(intent);
                };

                viewEnabled.AddView(v);
            }

            var disabledItems = AccountsGroupViewData.DisabledAccountsItems(_currency).ToList();
            var viewDisabled = FindViewById<LinearLayout>(Resource.Id.view_disabled_accounts);

            _sortDisabledName.Data = ViewData.ViewData.AccountGroup.SortButtonsAccounts[0];
            _sortDisabledName.First = true;
            _sortDisabledAmount.Data = ViewData.ViewData.AccountGroup.SortButtonsAccounts[1];

            viewDisabled.RemoveAllViews();

            foreach (var i in disabledItems)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_account, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = i.Name;

                v.Click += (sender, e) =>
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, i.Id);

                    StartActivity(intent);
                };

                viewDisabled.AddView(v);
            }

            FindViewById(Resource.Id.view_accounts).Visibility = enabledItems.Any() ? ViewStates.Visible : ViewStates.Gone;
            FindViewById(Resource.Id.view_disabled_accounts_container).Visibility = disabledItems.Any() ? ViewStates.Visible : ViewStates.Gone;

            _swipeToRefresh.Refreshing = false;
        }
    }
}