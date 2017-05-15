using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currencies.Model;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class CurrencyListAdapter : ArrayAdapter<Currency>
    {
        public CurrencyListAdapter(Context context, List<Currency> items) : base(context, 0, items)
        { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_currency, parent, false);
            }

            convertView.FindViewById<TextView>(Resource.Id.text_code).Text = item.Code;
            convertView.FindViewById<TextView>(Resource.Id.text_name).Text = item.Name;

            return convertView;
        }
    }
}