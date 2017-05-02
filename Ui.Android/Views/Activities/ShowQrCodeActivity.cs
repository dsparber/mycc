using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]

    public class ShowQrCodeActivity : MyccActivity
    {
        private ImageView _imageBarcode;

        public const string ExtraSourceId = "SourceId";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_show_qr_code);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = Resources.GetString(Resource.String.QrCode);

            _imageBarcode = FindViewById<ImageView>(Resource.Id.imageBarcode);

            var size = new[] { Resources.DisplayMetrics.HeightPixels, Resources.DisplayMetrics.WidthPixels }.Min();

            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = (int)(size * 0.67),
                    Height = (int)(size * 0.67),
                    Margin = 1
                }
            };

            var id = Intent?.GetIntExtra(ExtraSourceId, -1) ?? -1;
            var source = AccountStorage.Instance.Repositories.OfType<AddressAccountRepository>().FirstOrDefault(r => r.Id == id);

            if (source == null) throw new NullReferenceException("A source id needs to be specified and passed with the intent!");

            var barcode = barcodeWriter.Write($"{source.Currency.Code.ToLower()}:{source.Address}?label={source.Name}");

            _imageBarcode.SetImageBitmap(barcode);


            var header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(header, activityRootView.Height > 480.DpToPx());

            FindViewById<TextView>(Resource.Id.text_address).Text = $"{Resources.GetString(Resource.String.Address)}:\n{source.Address.Substring(0, source.Address.Length / 2)}\u200B{source.Address.Substring(source.Address.Length / 2)}";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }
    }
}