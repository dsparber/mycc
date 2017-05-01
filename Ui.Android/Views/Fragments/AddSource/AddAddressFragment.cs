using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Fragments.AddSource
{
	public class AddAddressFragment : AddSourceFragment.Repository
	{
		private Currency _currency;
		private string _address;
		private EditText _currencyEntry;
		private EditText _nameEntry;

		private const int RequestCodeCurrency = 1;
		private string NameOrDefault => string.IsNullOrWhiteSpace(AddSourceActivity.Name) ? Resources.GetString(Resource.String.Unnamed) : AddSourceActivity.Name;
		private AddSourceActivity AddSourceActivity => Activity as AddSourceActivity;


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_add_address, container, false);

			var addressText = view.FindViewById<EditText>(Resource.Id.text_address);
			addressText.TextChanged += (sender, args) => _address = string.Join(string.Empty, args.Text).TrimAll();

			_nameEntry = view.FindViewById<EditText>(Resource.Id.text_name);
			_nameEntry.Text = AddSourceActivity?.Name;
			_nameEntry.AfterTextChanged += (sender, args) =>
			{
				if (!_nameEntry.HasFocus) return;

				var name = _nameEntry.Text;
				if (AddSourceActivity != null) AddSourceActivity.Name = name.TrimAll();
			};

			_currencyEntry = view.FindViewById<EditText>(Resource.Id.text_currency);
			_currencyEntry.Click += (sender, args) =>
			{
				var intent = new Intent(Context, typeof(CurrencyPickerActivity));
				intent.PutExtra(CurrencyPickerActivity.ExtraOnlyAddressCurrencies, true);
				StartActivityForResult(intent, RequestCodeCurrency);
			};

			return view;
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == RequestCodeCurrency && resultCode == (int)Result.Ok)
			{
				_currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
				_currencyEntry.Text = $"{_currency.Name} ({_currency.Code})";
				if (AddSourceActivity != null) AddSourceActivity.Currency = _currency;
			}
		}

		public override bool EntryComplete => _currency != null && !string.IsNullOrWhiteSpace(_address);


		public override OnlineAccountRepository GetRepository()
		{
			return EntryComplete ? AddressAccountRepository.CreateAddressAccountRepository(NameOrDefault, _currency, _address) : null;
		}

		public override void OnResume()
		{
			base.OnResume();
			if (_currencyEntry == null || AddSourceActivity == null) return;

			_nameEntry.Text = AddSourceActivity.Name;
			_currency = AddSourceActivity.Currency;
			_currencyEntry.Text = _currency != null ? $"{_currency.Name} ({_currency.Code})" : string.Empty;
		}
	}
}