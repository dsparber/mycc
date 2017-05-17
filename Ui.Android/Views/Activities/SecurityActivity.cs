﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]

    public class SecurityActivity : MyccActivity
    {
        private View _buttonEnablePin;
        private View _buttonDisablePin;
        private View _buttonChangePin;
        private View _fingerprintView;
        private View _useShakeView;
        private Switch _fingerprintSwitch;
        private Switch _useShakeSwitch;
        private HeaderFragment _header;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_security_actions);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _buttonEnablePin = FindViewById(Resource.Id.button_enable_pin);
            _buttonDisablePin = FindViewById(Resource.Id.button_disable_pin);
            _buttonChangePin = FindViewById(Resource.Id.button_change_pin);
            _fingerprintView = FindViewById(Resource.Id.layout_use_fingerprint);
            _useShakeView = FindViewById(Resource.Id.layout_use_shake_gesture);
            _fingerprintSwitch = FindViewById<Switch>(Resource.Id.switch_use_fingerprint);
            _useShakeSwitch = FindViewById<Switch>(Resource.Id.switch_use_shake_gesture);
            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);

            _fingerprintSwitch.CheckedChange += (sender, args) =>
            {
                ApplicationSettings.IsFingerprintEnabled = args.IsChecked;
                SetHeaderText();
            };
            _useShakeSwitch.CheckedChange += (sender, args) => ApplicationSettings.LockByShaking = args.IsChecked;

            _buttonEnablePin.Click += (sender, args) => OpenPinActivity(true, false);
            _buttonDisablePin.Click += (sender, args) => OpenPinActivity(false, true);
            _buttonChangePin.Click += (sender, args) => OpenPinActivity(true, true);
        }

        protected override void OnResume()
        {
            base.OnResume();

            _buttonEnablePin.Visibility = !ApplicationSettings.IsPinSet ? ViewStates.Visible : ViewStates.Gone;
            _buttonDisablePin.Visibility = ApplicationSettings.IsPinSet ? ViewStates.Visible : ViewStates.Gone;
            _buttonChangePin.Visibility = ApplicationSettings.IsPinSet ? ViewStates.Visible : ViewStates.Gone;
            _fingerprintView.Visibility = FingerprintHelper.IsFingerprintAvailable && ApplicationSettings.IsPinSet ? ViewStates.Visible : ViewStates.Gone;
            _useShakeView.Visibility = ApplicationSettings.IsPinSet ? ViewStates.Visible : ViewStates.Gone;
            _fingerprintSwitch.Checked = ApplicationSettings.IsFingerprintEnabled;
            _useShakeSwitch.Checked = ApplicationSettings.LockByShaking;

            SetHeaderText();
        }

        private void SetHeaderText()
        {
            _header.InfoText = Resources.GetString(ApplicationSettings.IsPinSet && ApplicationSettings.IsFingerprintEnabled ? Resource.String.FingerprintActive : ApplicationSettings.IsPinSet ? Resource.String.PinActive : Resource.String.NotConfigured);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        private void OpenPinActivity(bool newPin, bool oldPin)
        {
            var intent = new Intent(this, typeof(PinActivity));
            intent.PutExtra(PinActivity.ExtraEnterOldPin, oldPin);
            intent.PutExtra(PinActivity.ExtraEnterNewPin, newPin);
            StartActivity(intent);
        }
    }
}