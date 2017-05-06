using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]

    public class PinActivity : MyccActivity
    {
        public const string ExtraEnterOldPin = "EnterOldPin";
        public const string ExtraEnterNewPin = "EnterNewPin";


        private string _oldPin;
        private string _newPin;
        private string _repeatPin;

        private bool _changePin;


        private EditText _newPinEntry;
        private EditText _oldPinEntry;
        private EditText _repeatPinEntry;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_pin);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var oldPinRequired = Intent?.GetBooleanExtra(ExtraEnterOldPin, false) ?? false;
            var newPinRequired = Intent?.GetBooleanExtra(ExtraEnterNewPin, false) ?? false;
            _changePin = oldPinRequired && newPinRequired;

            var title = Resources.GetString(_changePin ? Resource.String.ChangePin : newPinRequired ? Resource.String.EnablePin : Resource.String.DisablePin);
            SupportActionBar.Title = Resources.GetString(Resource.String.Pin);

            _oldPinEntry = FindViewById<EditText>(Resource.Id.edit_old_pin);
            _newPinEntry = FindViewById<EditText>(Resource.Id.edit_new_pin);
            _repeatPinEntry = FindViewById<EditText>(Resource.Id.edit_repeat_pin);

            var header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            header.InfoText = title;

            _oldPinEntry.Visibility = oldPinRequired ? ViewStates.Visible : ViewStates.Gone;
            _newPinEntry.Visibility = newPinRequired ? ViewStates.Visible : ViewStates.Gone;
            _repeatPinEntry.Visibility = newPinRequired ? ViewStates.Visible : ViewStates.Gone;

            _oldPinEntry.AfterTextChanged += OldPinChanged;
            _newPinEntry.AfterTextChanged += NewPinChanged;
            _repeatPinEntry.AfterTextChanged += NewPinChanged;

            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(header, activityRootView.Height > 480.DpToPx());

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        private void DisplayError(TextView editText, int resourceId)
        {
            editText.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animation.shake));
            editText.Text = string.Empty;
            editText.Error = Resources.GetString(resourceId);
            Task.Run(() => Task.Delay(3000).ContinueWith(t => RunOnUiThread(() => editText.Error = null)));
        }

        private void NewPinChanged(object sender, EventArgs args)
        {
            _newPin = _newPinEntry.Text;
            _repeatPin = _repeatPinEntry.Text;

            if (string.IsNullOrWhiteSpace(_newPin) || string.IsNullOrWhiteSpace(_repeatPin) || _newPin.Length < 4 || _repeatPin.Length < 4) return;

            if (_newPin.Equals(_repeatPin))
            {
                ApplicationSettings.Pin = _newPin;
                Finish();
            }
            else
            {
                DisplayError(_repeatPinEntry, Resource.String.NewPinsDontMatch);
            }

        }

        private void OldPinChanged(object sender, EventArgs args)
        {
            _oldPin = _oldPinEntry.Text;
            if (string.IsNullOrWhiteSpace(_oldPin) || _oldPin.Length != ApplicationSettings.PinLength) return;

            if (ApplicationSettings.IsPinValid(_oldPin))
            {
                if (_changePin)
                {
                    _newPinEntry.RequestFocusFromTouch();
                }
                else
                {
                    ApplicationSettings.Pin = null;
                    Finish();
                }
            }
            else
            {
                DisplayError(_oldPinEntry, Resource.String.OldPinWrong);
            }
        }
    }
}