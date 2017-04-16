using Android.Content;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data.Get
{
    public class ViewData
    {
        public static RatesViewData Rates => _instance._ratesViewData;
        public static AssetsViewData Assets => _instance._assetsViewData;
        public static AssetsGraphViewData AssetsGraph => _instance._assetsGraphViewData;

        private readonly RatesViewData _ratesViewData;
        private readonly AssetsViewData _assetsViewData;
        private readonly AssetsGraphViewData _assetsGraphViewData;

        private ViewData(Context context)
        {
            _ratesViewData = new RatesViewData(context);
            _assetsViewData = new AssetsViewData(context);
            _assetsGraphViewData = new AssetsGraphViewData(context);

            Messaging.Update.Rates.Subscribe(this, _ratesViewData.UpdateRateItems);
            Messaging.Update.Assets.Subscribe(this, () => { _assetsViewData.UpdateRateItems(); _assetsGraphViewData.UpdateItems(); });

            Messaging.Request.Rates.Subscribe(this, TaskHelper.UpdateRates);
            Messaging.Request.Assets.Subscribe(this, TaskHelper.UpdateAssets);
            Messaging.Request.MissingRates.Subscribe(this, () => TaskHelper.FetchMissingRates());
        }


        private static ViewData _instance;

        public static void Init(Context context)
        {
            _instance = _instance ?? new ViewData(context);
        }

    }
}