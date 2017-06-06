using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies.Models;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;
using StringHelper = MyCC.Ui.Helpers.StringHelper;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Label = "@string/Editing", Theme = "@style/MyCC")]

    public class EditAccountActivity : MyccActivity
    {
        private const int RequestCodeCurrency = 1;

        public const string ExtraAccountId = "accountId";
        private FunctionalAccount _account;
        private HeaderFragment _header;
        private EditText _editCurrency;

        private string _name;
        private decimal _amount;
        private Currency _currency;
        private bool _enabled;
        private Money Money => new Money(_amount, _currency);
        private string NameOrDefault => string.IsNullOrWhiteSpace(_name) ? Resources.GetString(Resource.String.Unnamed) : _name;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_edit_account);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var accountId = Intent?.GetIntExtra(ExtraAccountId, -1) ?? -1;
            if (accountId == -1) throw new NullReferenceException("The account id needs to be specified and passed with the intent!");

            _account = AccountStorage.Instance.AllElements.Find(a => a.Id == accountId);

            _name = _account.Name;
            _currency = _account.Money.Currency;
            _amount = _account.Money.Amount;
            _enabled = _account.IsEnabled;

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.MainText = Money.ToString();
            _header.InfoText = Resources.GetString(Resource.String.ManuallyAdded);

            var editName = FindViewById<EditText>(Resource.Id.text_name);
            editName.Text = _name;
            editName.AfterTextChanged += (sender, args) => _name = editName.Text;

            var editAmount = FindViewById<EditText>(Resource.Id.text_amount);
            editAmount.Text = _amount.ToString(CultureInfo.CurrentCulture);
            editAmount.TextChanged += (sender, args) =>
            {
                var newText = string.Join(string.Empty, args.Text);
                var text = StringHelper.CheckIfDecimal(newText);
                if (!string.Equals(text, newText))
                {
                    editAmount.Text = string.Empty;
                    editAmount.Append(text);
                }
                decimal.TryParse(text, out _amount);
                _header.MainText = Money.ToString();
            };

            _editCurrency = FindViewById<EditText>(Resource.Id.text_currency);
            _editCurrency.Text = $"{_account.Money.Currency.Name} ({_account.Money.Currency.Code})";
            _editCurrency.Click += (sender, args) => StartActivityForResult(new Intent(this, typeof(CurrencyPickerActivity)), RequestCodeCurrency);

            var switchView = FindViewById<Switch>(Resource.Id.switch_enable_account);
            switchView.Checked = _account.IsEnabled;
            switchView.CheckedChange += (sender, args) => _enabled = args.IsChecked;

            var deleteButton = FindViewById<Button>(Resource.Id.button_delete);
            deleteButton.Click += (sender, args) =>
            {
                Delete();
                Finish();
            };

            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 480.DpToPx());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RequestCodeCurrency && resultCode == Result.Ok)
            {
                _currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
                _editCurrency.Text = $"{_currency.Name} ({_currency.Code})";
                _header.MainText = Money.ToString();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Save
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Save)))
            {
                Save();
                Finish();
            }
            else
            {
                Finish();
            }
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, Resources.GetString(Resource.String.Save))
                .SetIcon(Resource.Drawable.ic_action_done).SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        private async void Save()
        {
            _account.Money = Money;
            _account.LastUpdate = DateTime.Now;
            _account.Name = NameOrDefault;
            _account.IsEnabled = _enabled;

            await UiUtils.Edit.Update(_account);
        }

        private async void Delete()
        {
            await UiUtils.Edit.Delete(_account);
        }
    }
}