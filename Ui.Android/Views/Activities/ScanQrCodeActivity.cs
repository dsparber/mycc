using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Android;
using Result = Android.App.Result;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class ScanQrCodeActivity : MyccActivity
    {
        private ZXingScannerFragment _scanFragment;

        public const string ExtraQrText = "QrText";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_scan_qr_code);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = Resources.GetString(Resource.String.ScanQrCode);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }
        protected override void OnResume()
        {
            base.OnResume();

            var needsPermissionRequest = PermissionsHandler.NeedsPermissionRequest(this);

            if (needsPermissionRequest)
                PermissionsHandler.RequestPermissionsAsync(this);

            if (_scanFragment == null)
            {
                var view = LayoutInflater.FromContext(this).Inflate(Resource.Layout.overlay_scan_qr_code, null);
                var button = view.FindViewById<FloatingActionButton>(Resource.Id.button_flash);

                _scanFragment = new ZXingScannerFragment
                {
                    UseCustomOverlayView = true,
                    CustomOverlayView = view
                };

                button.Click += (sender, args) =>
                {
                    button.SetImageResource(_scanFragment.IsTorchOn ? Resource.Drawable.ic_action_flash_off : Resource.Drawable.ic_action_flash_on);
                    _scanFragment.ToggleTorch();
                };

                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragment_container, _scanFragment).Commit();
            }

            if (!needsPermissionRequest)
                Scan();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnPause()
        {
            _scanFragment?.StopScanning();
            base.OnPause();
        }

        private void Scan()
        {
            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = true,
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
            };

            _scanFragment.StartScanning(result =>
            RunOnUiThread(() =>
            {
                if (string.IsNullOrEmpty(result?.Text)) return;

                var resultData = new Intent();
                _scanFragment.StopScanning();
                resultData.PutExtra(ExtraQrText, result.Text);
                SetResult(Result.Ok, resultData);
                Finish();
            }), options);
        }
    }
}