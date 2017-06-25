using MyCC.Ui.Edit;
using MyCC.Ui.Get;
using MyCC.Ui.Get.Implementations;
using MyCC.Ui.Prepare;
using MyCC.Ui.Update;

namespace MyCC.Ui
{
    public static class UiUtils
    {
        public static readonly IGetUtils Get = new GetUtils();
        public static readonly IUpdateUtils Update = new UpdateUtils();
        public static readonly IEditUtils Edit = new EditUtils();
        public static readonly IPrepareUtils Prepare = new PrepareUtils();

        internal static readonly ICachedData AssetsRefresh = (ICachedData)Get.Assets;
        internal static readonly ICachedData RatesRefresh = (ICachedData)Get.Rates;
    }
}