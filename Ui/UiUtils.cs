using MyCC.Ui.Edit;
using MyCC.Ui.Prepare;
using MyCC.Ui.Update;

namespace MyCC.Ui
{
    public static class UiUtils
    {
        public static readonly IUpdateUtils Update = new UpdateUtils();
        public static readonly IEditUtils Edit = new EditUtils();
        public static readonly IPrepareUtil Prepare = new PrepareUtil();
    }
}