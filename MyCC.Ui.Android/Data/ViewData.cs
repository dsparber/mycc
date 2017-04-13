using Android.Content;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data
{
    public class ViewData
    {
        public static RatesViewData Rates => Instance._ratesViewData;
        private readonly RatesViewData _ratesViewData;

        private ViewData(Context context)
        {
            _ratesViewData = new RatesViewData(context);

            Messaging.Update.Rates.Subscribe(this, _ratesViewData.UpdateRateItems);

            Messaging.Request.Rates.Subscribe(this, TaskHelper.UpdateRates);
        }


        public static ViewData Instance;

        public static void Init(Context context)
        {
            Instance = Instance ?? new ViewData(context);
        }

    }
}