using MyCC.Forms.Android.renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TableView), typeof(CustomTableViewRenderer))]
namespace MyCC.Forms.Android.renderer
{
    public class CustomTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var listView = Control;
            listView.DividerHeight = 0;
            listView.SetHeaderDividersEnabled(false);
        }

        protected override TableViewModelRenderer GetModelRenderer(global::Android.Widget.ListView listView, TableView view)
        {
            return new CustomTableViewModelRenderer(Context, listView, view);
        }
    }
}