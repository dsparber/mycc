using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
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
        public const string ExtraQrText = "QrText";

        private ZXingScannerFragment _scanFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_scan_qr_code);

            var view = LayoutInflater.FromContext(this).Inflate(Resource.Layout.overlay_scan_qr_code, null);
            var button = view.FindViewById<FloatingActionButton>(Resource.Id.button_flash);

            _scanFragment = new ZXingScannerFragment
            {
                CustomOverlayView = view,
                UseCustomOverlayView = true
            };

            button.Click += (sender, args) =>
            {
                button.SetImageResource(_scanFragment.IsTorchOn ? Resource.Drawable.ic_action_flash_off : Resource.Drawable.ic_action_flash_on);
                _scanFragment.ToggleTorch();
            };

            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragment_container, _scanFragment, "ZXINGFRAGMENT")
                .Commit();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (PermissionsHandler.NeedsPermissionRequest(this))
                PermissionsHandler.RequestPermissionsAsync(this);
            else
                StartScanning();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void StartScanning()
        {
            _scanFragment.StartScanning(result =>
            {
                if (string.IsNullOrEmpty(result?.Text)) return;

                var resultData = new Intent();
                _scanFragment.StopScanning();
                resultData.PutExtra(ExtraQrText, result.Text);
                SetResult(Result.Ok, resultData);
                Finish();
            },
            new MobileBarcodeScanningOptions
            {
                AutoRotate = true,
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
            });
        }
    }
}