using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Views.Fragments;
using Android.Support.V4.Widget;
using Android.Content;
using Android.Support.Design.Widget;
using MyCC.Core.Currencies;
using MyCC.Ui.Get;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class AccountGroupActivity : MyccActivity
    {
        public const string ExtraCurrencyId = "currencyId";

        private string _currencyId;

        private HeaderFragment _header;
        private SortButtonFragment _sortReferenceAmount, _sortReferenceCurrency,
        _sortAccountsName, _sortAccountsAmount, _sortDisabledName, _sortDisabledAmount;
        private SwipeRefreshLayout _swipeToRefresh;
        private FooterFragment _footerFragment;

        private static readonly IAccountsGroupViewData Data = UiUtils.Get.AccountsGroup;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_account_group);

            _currencyId = Intent?.GetStringExtra(ExtraCurrencyId);


            SupportActionBar.Title = $"\u2211 {_currencyId.Code()}";

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footerFragment = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);


            _sortReferenceAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortReferenceCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            _sortAccountsName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_name_sort);
            _sortAccountsAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_amount_sort);

            _sortDisabledName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_name_sort);
            _sortDisabledAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_amount_sort);

            Messaging.Update.Balances.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Update.Rates.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Sort.Accounts.Subscribe(this, () => RunOnUiThread(SetData));
            Messaging.Sort.ReferenceTables.Subscribe(this, () => RunOnUiThread(SetData));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) => UiUtils.Update.FetchBalancesAndRatesFor(_currencyId);

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
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, _currencyId);
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
            if (!Data.HasAccounts(_currencyId))
            {
                Finish();
                return;
            }

            _header.Data = Data.HeaderData(_currencyId);
            _footerFragment.LastUpdate = Data.LastUpdate(_currencyId);

            FindViewById<TextView>(Resource.Id.text_equal_to).Text = Data.ReferenceTableHeader(_currencyId);

            var referenceItems = Data.ReferenceItems(_currencyId);
            var viewReference = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortReferenceAmount.Data = Data.SortButtonsReference[0];
            _sortAccountsAmount.First = true;
            _sortReferenceCurrency.Data = Data.SortButtonsReference[1];
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

            var enabledItems = Data.EnabledAccountsItems(_currencyId).ToList();
            var viewEnabled = FindViewById<LinearLayout>(Resource.Id.view_enabled_accounts);

            _sortAccountsName.Data = Data.SortButtonsAccounts[0];
            _sortAccountsName.First = true;
            _sortAccountsAmount.Data = Data.SortButtonsAccounts[1];

            viewEnabled.RemoveAllViews();

            foreach (var i in enabledItems)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_account, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = i.FormattedValue;
                v.FindViewById<TextView>(Resource.Id.text_name).Text = i.Name;

                v.Click += (sender, e) =>
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, i.Id);
                    StartActivity(intent);
                };

                viewEnabled.AddView(v);
            }

            var disabledItems = Data.DisabledAccountsItems(_currencyId).ToList();
            var viewDisabled = FindViewById<LinearLayout>(Resource.Id.view_disabled_accounts);

            _sortDisabledName.Data = Data.SortButtonsAccounts[0];
            _sortDisabledName.First = true;
            _sortDisabledAmount.Data = Data.SortButtonsAccounts[1];

            viewDisabled.RemoveAllViews();

            foreach (var accountItem in disabledItems)
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_account, null);
                v.FindViewById<TextView>(Resource.Id.text_amount).Text = accountItem.FormattedValue;
                v.FindViewById<TextView>(Resource.Id.text_name).Text = accountItem.Name;

                v.Click += (sender, e) =>
                {
                    var intent = new Intent(this, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, accountItem.Id);

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