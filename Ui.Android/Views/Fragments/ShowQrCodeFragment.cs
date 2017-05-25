using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class ShowQrCodeFragment : Fragment
    {
        private const string SourceIdKey = "source-id";
        private const string OnlyAddressKey = "only-address";

        public static ShowQrCodeFragment Create(int sourceId, bool onlyAddress)
        {
            var fragement = new ShowQrCodeFragment();

            var args = new Bundle();
            args.PutInt(SourceIdKey, sourceId);
            args.PutBoolean(OnlyAddressKey, onlyAddress);
            fragement.Arguments = args;

            return fragement;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_show_qr_code, container, false);

            var sourceId = Arguments.GetInt(SourceIdKey);
            var onlyAddress = Arguments.GetBoolean(OnlyAddressKey);

            var source = AccountStorage.Instance.Repositories.OfType<AddressAccountRepository>().FirstOrDefault(r => r.Id == sourceId);
            if (source == null) return view;

            var imageBarcode = view.FindViewById<ImageView>(Resource.Id.imageBarcode);

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

            var qrContent = onlyAddress ? source.Address : $"{source.Currency.Code.ToLower()}:{source.Address}?label={source.Name}";
            var barcode = barcodeWriter.Write(qrContent);
            imageBarcode.SetImageBitmap(barcode);

            view.FindViewById<TextView>(Resource.Id.text_address).Text = $"{Resources.GetString(Resource.String.Address)}:\n{source.Address.Substring(0, source.Address.Length / 2)}\u200B{source.Address.Substring(source.Address.Length / 2)}";

            return view;
        }
    }
}