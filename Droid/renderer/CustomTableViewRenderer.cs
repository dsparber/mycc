using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MyCryptos.renderer;

[assembly: ExportRenderer(typeof(TableView), typeof(CustomTableViewRenderer))]
namespace MyCryptos.renderer
{
    public class CustomTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var listView = Control as global::Android.Widget.ListView;
            listView.DividerHeight = 0;
            listView.SetHeaderDividersEnabled(false);
        }

        protected override TableViewModelRenderer GetModelRenderer(global::Android.Widget.ListView listView, TableView view)
        {
            return new CustomTableViewModelRenderer(this.Context, listView, view);
        }
    }
}