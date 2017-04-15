using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Data.Get;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class AssetsListAdapter : ArrayAdapter<AssetItem>
    {
        public AssetsListAdapter(Context context, IList<AssetItem> items) : base(context, 0, items)
        { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_asset, parent, false);
            }

            convertView.FindViewById<TextView>(Resource.Id.text_currency).Text = item.CurrencyCode;
            convertView.FindViewById<TextView>(Resource.Id.text_amount).Text = item.FormattedValue;
            convertView.FindViewById<TextView>(Resource.Id.text_reference).Text = item.FormattedReferenceValue;

            return convertView;
        }
    }
}