using Android.Content;
using Android.Views;
using Android.Widget;
using MyCC.Forms.Android.renderer;
using MyCC.Forms.constants;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TableViewModelRenderer), typeof(CustomTableViewModelRenderer))]
namespace MyCC.Forms.Android.renderer
{
    public class CustomTableViewModelRenderer : TableViewModelRenderer
    {
        public CustomTableViewModelRenderer(Context Context, global::Android.Widget.ListView ListView, TableView View)
            : base(Context, ListView, View)
        { }
        public override global::Android.Views.View GetView(int position, global::Android.Views.View convertView, ViewGroup parent)
        {
            var androidView = base.GetView(position, convertView, parent);

            var element = GetCellForPosition(position);

            if (element.GetType() == typeof(TextCell))
            {
                var text = ((((androidView as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView);
                var divider = (androidView as LinearLayout).GetChildAt(1);

                text.SetTextColor(AppConstants.ThemeColor.ToAndroid());
                divider.SetBackgroundColor(Color.FromHex("c7d7d4").ToAndroid());
                text.SetPadding(30, 10, 0, 0);
            }

            return androidView;
        }
    }
}