using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;
using MyCC.Core.Account.Models.Base;
using Android.Support.V4.Widget;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;

namespace MyCC.Ui.Android.Views.Activities
{
	[Activity(Theme = "@style/MyCC")]
	public class AccountDetailActivity : AppCompatActivity
	{
		public const string ExtraAccountId = "account";

		private FunctionalAccount _account;

		private SortButtonFragment _sortAmount, _sortCurrency;

		private SwipeRefreshLayout _swipeToRefresh;

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
			var header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);

			header.Data = AccountViewData.HeaderData(_account);

			_sortAmount = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
			_sortCurrency = (SortButtonFragment)SupportFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);

			Messaging.UiUpdate.AccountDetail.Subscribe(this, () => RunOnUiThread(SetData));


			_swipeToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

			_swipeToRefresh.Refresh += (sender, e) =>
			{
				Messaging.Request.Account.Send(_account);
				Messaging.Request.Rates.Send(_account);
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
			Func<bool, ViewStates> show = b => b ? ViewStates.Visible : ViewStates.Gone;

			FindViewById<TextView>(Resource.Id.text_name).Text = ViewData.AccountDetail.AccountName(_account);
			FindViewById<TextView>(Resource.Id.text_type).Text = ViewData.AccountDetail.AccountType(_account);


			FindViewById<TextView>(Resource.Id.text_source).Text = ViewData.AccountDetail.AccountSource(_account);
			FindViewById<TextView>(Resource.Id.text_address).Text = ViewData.AccountDetail.AccountAddress(_account);

			FindViewById(Resource.Id.label_source).Visibility = show(ViewData.AccountDetail.ShowAccountSource(_account));
			FindViewById(Resource.Id.text_source).Visibility = show(ViewData.AccountDetail.ShowAccountSource(_account));
			FindViewById(Resource.Id.label_address).Visibility = show(ViewData.AccountDetail.ShowAccountAddress(_account));
			FindViewById(Resource.Id.text_address).Visibility = show(ViewData.AccountDetail.ShowAccountAddress(_account));

			FindViewById<TextView>(Resource.Id.text_equal_to).Text = string.Format(Resources.GetString(_account.Money.Amount == 1 ? Resource.String.IsEqualTo : Resource.String.AreEqualTo), _account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney));


			var items = AccountViewData.Items(_account);
			var view = FindViewById<LinearLayout>(Resource.Id.view_reference);

			_sortAmount.Data = ViewData.AccountDetail.SortButtons[0];
			_sortCurrency.Data = ViewData.AccountDetail.SortButtons[1];

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