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
using MyCC.Core.Currencies.Model;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Messages;
using MyCC.Ui.Android.Views.Fragments;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Label = "@string/Editing", Theme = "@style/MyCC")]

    public class EditSourceActivity : MyccActivity
    {
        private const int RequestCodeCurrency = 1;

        public const string ExtraRepositoryId = "repositoryId";
        private OnlineAccountRepository _repository;
        private HeaderFragment _header;
        private EditText _editCurrency;

        private string _name;
        private string _address;
        private Currency _currency;
        private bool _enabled;

        private string NameOrDefault => string.IsNullOrWhiteSpace(_name) ? Resources.GetString(Resource.String.Unnamed) : _name;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_edit_source);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var repositoryId = Intent?.GetIntExtra(ExtraRepositoryId, -1) ?? -1;
            _repository = AccountStorage.Instance.Repositories.Find(r => r.Id == repositoryId) as OnlineAccountRepository;

            if (repositoryId == -1 || _repository == null) throw new NullReferenceException("The account id needs to be specified and passed with the intent!");

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
                Delete();
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
                _currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
                _editCurrency.Text = $"{_currency.Name} ({_currency.Code})";
            }
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
            var dialog = this.GetLoadingDialog(null, Resource.String.Testing);
            dialog.Show();

            // Test if data is valid
            var addressRepo = _repository as AddressAccountRepository;
            if (addressRepo != null && (!addressRepo.Address.Equals(_address) || !addressRepo.Currency.Equals(_currency)))
            {
                var testRepo = AddressAccountRepository.CreateAddressAccountRepository(addressRepo.Name, _currency, _address ?? string.Empty);

                if (testRepo == null || !await testRepo.Test())
                {
                    dialog.Dismiss();
                    this.ShowInfoDialog(Resource.String.Error, Resource.String.FetchingNoSuccessText);
                    return;
                }
                if (!addressRepo.Currency.Equals(_currency))
                {
                    await AccountStorage.Instance.Remove(_repository);
                    await testRepo.FetchOnline();
                    await AccountStorage.Instance.Add(testRepo);
                }
                testRepo.Id = _repository.Id;
                _repository = testRepo;
                await _repository.FetchOnline();
            }

            // Apply name and enabled status
            _repository.Name = NameOrDefault;
            foreach (var a in _repository.Elements)
            {
                a.Name = _repository.Name;
                a.IsEnabled = _enabled;
            }

            // Save changes
            await AccountStorage.Instance.Update(_repository);
            foreach (var a in _repository.Elements.ToList())
            {
                await AccountStorage.Update(a);
            }

            Messaging.Update.Assets.Send();
            dialog.Dismiss();

            Finish();
        }

        private async void Delete()
        {
            await AccountStorage.Instance.Remove(_repository);
            Messaging.Update.Assets.Send();
        }
    }
}