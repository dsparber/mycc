using MyCC.Ui;
using MyCC.Ui.Messages;

namespace MyCC.Forms.View.Components.Header
{
    public class RatesOverviewHeader : HeaderView
    {
        public RatesOverviewHeader(string currencyId) : base(true)
        {
            Data = UiUtils.Get.Rates.HeaderFor(currencyId);
            Messaging.Update.Rates.Subscribe(this, () => Data = UiUtils.Get.Rates.HeaderFor(currencyId));
        }
    }
}
