using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Helper;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Views.Activities;
using Result = Android.App.Result;

namespace MyCC.Ui.Android.Views.Fragments.AddSource
{
    public class AddAddressFragment : AddSourceFragment.Repository
    {
        private Currency _currency;
        private string _address;
        private EditText _currencyEntry;
        private EditText _nameEntry;
        private EditText _addressEntry;

        private const int RequestCodeCurrency = 1;
        private const int RequestCodeQrText = 2;
        private string NameOrDefault => string.IsNullOrWhiteSpace(AddSourceActivity.Name) ? Resources.GetString(Resource.String.Unnamed) : AddSourceActivity.Name;
        private AddSourceActivity AddSourceActivity => Activity as AddSourceActivity;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_add_address, container, false);

            _addressEntry = view.FindViewById<EditText>(Resource.Id.text_address);
            _addressEntry.TextChanged += (sender, args) => _address = string.Join(string.Empty, args.Text).TrimAll();

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

            view.FindViewById<Button>(Resource.Id.button_scan_qr).Click += (sender, args) =>
            {
                StartActivityForResult(new Intent(Context, typeof(ScanQrCodeActivity)), RequestCodeQrText);
            };

            return view;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RequestCodeCurrency && resultCode == (int)Result.Ok)
            {
                _currency = data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency).Find();
                _currencyEntry.Text = $"{_currency.Name} ({_currency.Code})";
                if (AddSourceActivity != null) AddSourceActivity.Currency = _currency;
            }
            else if (requestCode == RequestCodeQrText && resultCode == (int)Result.Ok)
            {
                var text = data.GetStringExtra(ScanQrCodeActivity.ExtraQrText);

                var tuple = text.Parse(AddressAccountRepository.AllSupportedCurrencies);

                _address = tuple.Item1.TrimAll() ?? _address;
                _currency = tuple.Item2 ?? _currency;
                var name = tuple.Item3.TrimAll() ?? NameOrDefault;

                _nameEntry.Text = name;
                _addressEntry.Text = _address;

                if (_currency != null)
                {
                    _currencyEntry.Text = $"{_currency.Name} ({_currency.Code})";
                }

                if (AddSourceActivity == null) return;
                AddSourceActivity.Currency = _currency;
                AddSourceActivity.Name = name;
            }
        }

        public override bool EntryComplete => (_currency ?? AddSourceActivity?.Currency) != null && !string.IsNullOrWhiteSpace(_address);


        public override OnlineAccountRepository GetRepository()
        {
            return EntryComplete ? AddressAccountRepository.CreateAddressAccountRepository(NameOrDefault, _currency ?? AddSourceActivity.Currency, _address) : null;
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