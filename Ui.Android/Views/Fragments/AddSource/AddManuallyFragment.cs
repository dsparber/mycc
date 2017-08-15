using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Views.Activities;

namespace MyCC.Ui.Android.Views.Fragments.AddSource
{
    public class AddManuallyFragment : AddSourceFragment.Account
    {
        private Currency _currency;
        private decimal _amount;
        private EditText _currencyEntry;
        private EditText _nameEntry;

        private const int RequestCodeCurrency = 1;

        private AddSourceActivity AddSourceActivity => Activity as AddSourceActivity;

        private string NameOrDefault => string.IsNullOrWhiteSpace(AddSourceActivity.Name) ? Resources.GetString(Resource.String.Unnamed) : AddSourceActivity.Name;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_add_manually, container, false);

            var amountText = view.FindViewById<EditText>(Resource.Id.text_amount);
            amountText.TextChanged += (sender, args) =>
            {
                var newText = string.Join(string.Empty, args.Text);
                var text = newText.CheckIfDecimal();
                if (!string.Equals(text, newText))
                {
                    amountText.Text = string.Empty;
                    amountText.Append(text);
                }
                decimal.TryParse(text, out _amount);
            };

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
                StartActivityForResult(intent, RequestCodeCurrency);
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
        }

        public override bool EntryComplete => (_currency ?? AddSourceActivity?.Currency) != null && _amount >= 0;

        public override LocalAccount GetAccount()
        {
            return EntryComplete ? new LocalAccount(null, NameOrDefault, new Money(_amount, _currency ?? AddSourceActivity.Currency), true, DateTime.Now, AccountStorage.Instance.LocalRepository.Id) : null;
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