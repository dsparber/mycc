using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Label = "@string/Editing", Theme = "@style/MyCC")]

    public class EditSourceActivity : MyccActivity
    {
        private const int RequestCodeCurrency = 1;

        public const string KeyRepositoryId = "repositoryId";
        private OnlineAccountRepository _repository;
        private HeaderFragment _header;
        private EditText _editCurrency;

        private int _repositoryId;
        private string _name;
        private string _address;
        private Currency _currency;
        private bool _enabled;

        private string NameOrDefault => string.IsNullOrWhiteSpace(_name) ? Resources.GetString(Resource.String.Unnamed) : _name;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            savedInstanceState = savedInstanceState ?? new Bundle();
            SetContentView(Resource.Layout.activity_edit_source);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _repositoryId = savedInstanceState.GetInt(KeyRepositoryId, Intent?.GetIntExtra(KeyRepositoryId, -1) ?? -1);
            _repository = AccountStorage.Instance.Repositories.Find(r => r.Id == _repositoryId) as OnlineAccountRepository;

            if (_repositoryId == -1 || _repository == null) throw new NullReferenceException("The repository id needs to be specified and passed with the intent!");

            _name = _repository.Name;
            _enabled = _repository.Elements.All(a => a.IsEnabled);

            if (_repository is AddressAccountRepository)
            {
                var addresRepo = (AddressAccountRepository)_repository;
                _currency = addresRepo.Currency;
                _address = addresRepo.Address;
            }


            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.MainText = _name;
            _header.InfoText = $"{Resources.GetString(Resource.String.Source)}: {_repository.Description}";

            var editName = FindViewById<EditText>(Resource.Id.text_name);
            editName.Text = _name;
            editName.AfterTextChanged += (sender, args) =>
            {
                _name = editName.Text;
                _header.MainText = NameOrDefault;
            };

            var editAddress = FindViewById<EditText>(Resource.Id.text_address);
            editAddress.Text = _address;
            editAddress.TextChanged += (sender, args) => _address = string.Concat(args.Text);

            _editCurrency = FindViewById<EditText>(Resource.Id.text_currency);
            _editCurrency.Text = $"{_currency?.Name} ({_currency?.Code})";
            _editCurrency.Click += (sender, args) =>
            {
                var intent = new Intent(this, typeof(CurrencyPickerActivity));
                intent.PutExtra(CurrencyPickerActivity.ExtraOnlyAddressCurrencies, true);
                StartActivityForResult(intent, RequestCodeCurrency);
            };

            var switchView = FindViewById<Switch>(Resource.Id.switch_enable_account);
            switchView.Checked = _enabled;
            switchView.CheckedChange += (sender, args) => _enabled = args.IsChecked;

            var deleteButton = FindViewById<Button>(Resource.Id.button_delete);
            deleteButton.Click += (sender, args) =>
            {
                UiUtils.Edit.Delete(_repository);
                Finish();
            };

            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 480.DpToPx());

            if (_repository is AddressAccountRepository && !(_repository is BlockchainXpubAccountRepository)) return;

            FindViewById(Resource.Id.view_currency).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.view_address).Visibility = ViewStates.Gone;

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RequestCodeCurrency && resultCode == Result.Ok)
            {
                _currency = data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency).Find();
                _editCurrency.Text = $"{_currency.Name} ({_currency.Code})";
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(KeyRepositoryId, _repositoryId);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Save
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Save)))
            {
                Save();
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
            try
            {
                var dialog = this.GetLoadingDialog(null, Resource.String.Testing);
                dialog.Show();

                var enabledStates = _repository.Elements.ToDictionary(a => a.Id, a => _enabled);
                await UiUtils.Edit.Update(_repository, _address, _currency.Id, NameOrDefault, enabledStates,
                    () => dialog.Dismiss());

                dialog.Dismiss();
                Finish();
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }
    }
}