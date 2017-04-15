using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Ui.Android.Views.Activities;

namespace MyCC.Ui.Android.Views.Fragments.AddSource
{
    public class AddBittrexFragment : AddSourceFragment.Repository
    {
        private string _key;
        private string _secret;
        private EditText _nameEntry;

        private string NameOrDefault => string.IsNullOrWhiteSpace(AddSourceActivity.Name) ? Resources.GetString(Resource.String.Unnamed) : AddSourceActivity.Name;
        private AddSourceActivity AddSourceActivity => (AddSourceActivity)Activity;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_add_bittrex, container, false);

            var keyText = view.FindViewById<EditText>(Resource.Id.text_key);
            keyText.TextChanged += (sender, args) => _key = string.Join(string.Empty, args.Text);

            var secretText = view.FindViewById<EditText>(Resource.Id.text_secret);
            secretText.TextChanged += (sender, args) => _secret = string.Join(string.Empty, args.Text);

            _nameEntry = view.FindViewById<EditText>(Resource.Id.text_name);
            _nameEntry.Text = AddSourceActivity.Name;
            _nameEntry.AfterTextChanged += (sender, args) =>
            {
                if (!_nameEntry.HasFocus) return;

                var name = _nameEntry.Text;
                AddSourceActivity.Name = name;
            };

            return view;
        }

        public override bool EntryComplete => !string.IsNullOrWhiteSpace(_secret) && !string.IsNullOrWhiteSpace(_key);

        public override OnlineAccountRepository GetRepository()
        {
            return EntryComplete ? new BittrexAccountRepository(default(int), NameOrDefault, _key.Replace("\n", string.Empty).Trim(), _secret.Replace("\n", string.Empty).Trim()) : null;
        }

        public override void OnResume()
        {
            base.OnResume();
            if (_nameEntry == null) return;

            _nameEntry.Text = AddSourceActivity.Name;
        }
    }
}