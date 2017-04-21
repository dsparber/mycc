using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Fragments;
using Android.Support.V4.Widget;
using MyCC.Core.Settings;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using System.Linq;
using Android.Content;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class AccountGroupActivity : AppCompatActivity
    {
        public const string ExtraCurrencyId = "currencyId";

        private Currency _currency;

        private HeaderFragment _header;

        private SortButtonFragment _sortReferenceAmount, _sortReferenceCurrency,
        _sortAccountsName, _sortAccountsAmount, _sortDisabledName, _sortDisabledAmount;

        private SwipeRefreshLayout _swipeToRefresh;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_account_group);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var currencyId = Intent?.GetStringExtra(ExtraCurrencyId);
            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                _currency = CurrencyStorage.Instance.AllElements.Find(c => currencyId.Equals(c?.Id));
            }

            SupportActionBar.Title = $"\u2211 {_currency.Code}";

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = AccountsGroupViewData.HeaderData(_currency);


            _sortReferenceAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            _sortReferenceCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

            _sortAccountsName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_name_sort);
            _sortAccountsAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_account_amount_sort);

            _sortDisabledName = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_name_sort);
            _sortDisabledAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_disabled_amount_sort);

            Messaging.UiUpdate.AccountDetail.Subscribe(this, () => RunOnUiThread(SetData));


            _swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            _swipeToRefresh.Refresh += (sender, e) =>
            {
                Messaging.Request.Accounts.Send(_currency);
                Messaging.Request.Rates.Send(_currency);
            };

            SetData();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
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

            var money = AccountsGroupViewData.GetEnabledSum(_currency);
            FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(money.Amount == 1 ? Resource.String.IsEqualTo : Resource.String.AreEqualTo), money.ToStringTwoDigits(ApplicationSettings.RoundMoney));


            var referenceItems = AccountsGroupViewData.ReferenceItems(_currency);
            var viewReference = FindViewById<LinearLayout>(Resource.Id.view_reference);

            _sortReferenceAmount.Data = ViewData.AccountGroup.SortButtonsReference[0];
            _sortAccountsAmount.First = true;
            _sortReferenceCurrency.Data = ViewData.AccountGroup.SortButtonsReference[1];
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

            _sortAccountsName.Data = ViewData.AccountGroup.SortButtonsAccounts[0];
            _sortAccountsName.First = true;
            _sortAccountsAmount.Data = ViewData.AccountGroup.SortButtonsAccounts[1];

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

            _sortDisabledName.Data = ViewData.AccountGroup.SortButtonsAccounts[0];
            _sortDisabledName.First = true;
            _sortDisabledAmount.Data = ViewData.AccountGroup.SortButtonsAccounts[1];

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